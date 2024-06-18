module DataParser.Console.DataFiles

open System
open System.Globalization
open System.IO
open System.Text
open System.Text.RegularExpressions
open DataParser.Console.Core
open DataParser.Console.FormatFiles

let [<Literal>] TrueByte = 49uy
let [<Literal>] FalseByte = 48uy

type DataFileName = DataFileName of fileFormat: FormatName * rawDate : string

type FilePath = FilePath of string

type JsonObject = JsonObject of Map<string, obj>

type DataFileFormat =
    { FilePath: FilePath
      Name: DataFileName
      FormatLines: FormatLine list }

let dataFileNameRegex =
    Regex(@"^(.+)_(\d\d\d\d-\d\d-\d\d)$", RegexOptions.Compiled ||| RegexOptions.Singleline ||| RegexOptions.CultureInvariant)
    
let tryLookupFormatLines availableFormats given =
    match Map.tryFind given availableFormats with
    | Some v -> Ok v
    | None -> Error <| [FileFormatNotFound (Set.ofSeq availableFormats.Keys, given)]

let parseDataFileName (fileNameWithoutExtension as FileNameWithoutExtension s) =
    let regexMatch = dataFileNameRegex.Match s
    if regexMatch.Success
    then Ok <| DataFileName (FormatName regexMatch.Groups[1].Value, regexMatch.Groups[2].Value)
    else Error [DataFileNameFormatError fileNameWithoutExtension]
    
let parseDataFileLine (formatLines : FormatLine list) (dataFileLine : string) : Result<JsonObject, Error list> =
    let parseDataType dataType (s: byte array) : Result<obj, Error list> =
        match dataType with
        | JBool ->
            match s with
            | [| TrueByte |] -> Ok true
            | [| FalseByte |] -> Ok false
            | _ ->
                let errorValue = Encoding.UTF8.GetString s
                Error [UnparsableValue errorValue] 
        | JInt ->
            match Int32.TryParse (s, CultureInfo.InvariantCulture) with
            | true, i -> Ok i
            | false, _ ->
                let errorValue = Encoding.UTF8.GetString s
                Error [UnparsableValue errorValue] 
        | JString ->
            let result = Encoding.UTF8.GetString s
            Ok <| result.Trim()
        
    let folder result (FormatLine (columnName, width, dataType)) =
        match result with
        | Error e -> Error e
        | Ok (stream : MemoryStream, map) ->
            let byteArray = Array.create width 0uy
            try
                stream.ReadExactly(byteArray, 0, width)
                let result = parseDataType dataType byteArray
                Ok (stream, Map.add columnName result map)
            with :? EndOfStreamException ->
                let line = Encoding.UTF8.GetString (stream.ToArray())
                Error [DataFileLineLengthShorterThanSpec line]
    
    let bytes : byte array = Encoding.UTF8.GetBytes dataFileLine
    use s = new MemoryStream(bytes)
    let initialState = Ok (s, Map.empty)
    
    result {
        let! _, jsonObjectMap = List.fold folder initialState formatLines
        let! y = Result.sequenceMap jsonObjectMap
        return JsonObject y
    }
