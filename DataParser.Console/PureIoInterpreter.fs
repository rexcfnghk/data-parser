namespace DataParser.Console

open ResultMap
open DataParser.Console.DataFiles

module PureIOInterpreter =
    open Core
    open FormatFiles

    type RecordedAction =
        | ReadSpecs of folder: string
        | ParseDataFile of filePath: string
        | WriteOutput of folder: string * DataFileParseResult
        | LogError of string
        | LogInfo of string

    /// Interpret a FreeIO program purely by invoking provided handlers and recording actions.
    let rec interpretPure (onReadSpecs: string -> ResultMap<FormatName, FormatLine list, Error>)
                          (onParse: DataFileFormat -> Result<DataFileParseResult, Error list>)
                          (program: FreeIO<'a>) : ('a * RecordedAction list) =
        match program with
        | Pure x -> (x, [])
        | Free op ->
            match op with
            | IO.ReadSpecs(folder, next) ->
                let specs = onReadSpecs folder
                let (res, actions) = interpretPure onReadSpecs onParse (next specs)
                (res, RecordedAction.ReadSpecs folder :: actions)
            | IO.ParseDataFile(df, next) ->
                // choose to call the parse handler and record which file was parsed
                let parseRes = onParse df
                let (res, actions) = interpretPure onReadSpecs onParse (next parseRes)
                let filePathStr = match df.FilePath with FilePath s -> s
                (res, RecordedAction.ParseDataFile filePathStr :: actions)
            | IO.WriteOutput(folder, result, next) ->
                let (res, actions) = interpretPure onReadSpecs onParse (next())
                (res, RecordedAction.WriteOutput(folder, result) :: actions)
            | IO.LogError(msg, next) ->
                let (res, actions) = interpretPure onReadSpecs onParse (next())
                (res, RecordedAction.LogError msg :: actions)
            | IO.LogInfo(msg, next) ->
                let (res, actions) = interpretPure onReadSpecs onParse (next())
                (res, RecordedAction.LogInfo msg :: actions)
