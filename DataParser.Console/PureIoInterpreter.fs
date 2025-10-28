namespace DataParser.Console

open System
open ResultMap
open DataParser.Console.DataFiles
open DataParser.Console.FileRead

module PureIoInterpreter =

    type Action =
        | ReadSpecs of folder: string
        | ParseDataFile of filePath: string
        | WriteOutput of folder: string * DataFileParseResult
        | LogError of string
        | LogInfo of string

    /// Interpret a FreeIO program purely by invoking provided handlers and recording actions.
    let rec interpretPure (onReadSpecs: string -> ResultMap<FormatName, FormatLine list, Error>)
                          (onParse: DataFileFormat -> Result<DataFileParseResult, Error list>)
                          (program: FreeIO<'a>) : ('a * Action list) =
        match program with
        | Pure x -> (x, [])
        | Free (ReadSpecs(folder, next)) ->
            let specs = onReadSpecs folder
            let (res, actions) = interpretPure onReadSpecs onParse (next specs)
            (res, ReadSpecs folder :: actions)
        | Free (ParseDataFile(df, next)) ->
            // choose to call the parse handler and record which file was parsed
            let parseRes = onParse df
            let (res, actions) = interpretPure onReadSpecs onParse (next parseRes)
            let filePathStr = match df.FilePath with FilePath s -> s
            (res, ParseDataFile filePathStr :: actions)
        | Free (WriteOutput(folder, result, next)) ->
            let (res, actions) = interpretPure onReadSpecs onParse (next())
            (res, WriteOutput(folder, result) :: actions)
        | Free (LogError(msg, next)) ->
            let (res, actions) = interpretPure onReadSpecs onParse (next())
            (res, LogError msg :: actions)
        | Free (LogInfo(msg, next)) ->
            let (res, actions) = interpretPure onReadSpecs onParse (next())
            (res, LogInfo msg :: actions)
