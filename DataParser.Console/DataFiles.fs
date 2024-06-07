module DataParser.Console.DataFiles

open System.Globalization
open System.IO
open System.Linq
open System.Text
open DataParser.Console.Core
open System.Text.RegularExpressions
open DataParser.Console.FormatFiles

let [<Literal>] trueByteLiteral = 49uy
let [<Literal>] falseByteLiteral = 48uy

type DataFileName = DataFileName of fileFormat: FileFormat * rawDate : string

let lookupFileFormat availableFormats given =
    if Set.contains given availableFormats
    then Ok given
    else Error <| FileFormatNotFound (availableFormats, given)
    
let collectFileFormats = Set.ofList

let dataFileNameRegex =
    Regex(@"^(.+)_(\d\d\d\d-\d\d-\d\d)$", RegexOptions.Compiled ||| RegexOptions.Singleline ||| RegexOptions.CultureInvariant)

let parseDataFileName s =
    let regexMatch = dataFileNameRegex.Match s
    if regexMatch.Success
    then Ok <| DataFileName (FileFormat regexMatch.Groups[1].Value, regexMatch.Groups[2].Value)
    else Error <| DataFileNameFormatError s
    
let parseDataFileLine (formatLines : FormatLine list) (dataFileLine : string) : Map<string, obj> =
    let parseDataType dataType (s: byte array) : Result<obj, Error> =
        match dataType with
        | JBool -> if s = [|trueByteLiteral|] then Ok true elif s = [|falseByteLiteral|] then Ok false else Error (UnparsableValue s) 
        | JInt -> match System.Int32.TryParse (s, CultureInfo.InvariantCulture) with true, i -> Ok i | false, _ -> Error (UnparsableValue s)
        | JString -> Ok <| let result = Encoding.UTF8.GetString s in result.Trim()
        
    let folder (stream : MemoryStream, lineMaxCount, map) (FormatLine (columnName, width, dataType)) =
        let byteArray = Array.create width 0uy
        stream.ReadExactly(byteArray, 0, width)
        let result = parseDataType dataType byteArray
        stream, lineMaxCount, Map.add columnName result map
    
    let bytes : byte array = Encoding.UTF8.GetBytes dataFileLine
    use s = new MemoryStream(bytes)
    let lineMaxCount = List.sumBy (fun (FormatLine (_, width, _)) -> width) formatLines
    let _, _, map = List.fold folder (s, lineMaxCount, Map.empty) formatLines
    
    map
    |> Map.filter (fun _ -> Result.isOk)
    |> Map.map (fun _ (Ok v) -> v)