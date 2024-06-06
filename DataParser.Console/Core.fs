module DataParser.Console.Core

open System.Text.RegularExpressions

type FileFormat = FileFormat of string

type DataFileName = DataFileName of fileFormat: FileFormat * rawDate : string

type Error =
    | FileFormatNotFound of availableFormats: Set<FileFormat> * givenFormat : FileFormat
    | DataFileNameFormatError of fileName: string
    | UnexpectedFormatLine of string

type JsonDataType = JString | JBool | JInt

type FormatLine = FormatLine of columnName: string * width: int * dataType : JsonDataType

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
    then Ok <| DataFileName (FileFormat regexMatch.Groups.[1].Value, regexMatch.Groups.[2].Value)
    else Error <| DataFileNameFormatError s
    
let parseFormatLine (regex: Regex) line =
    let parseJsonDataType = function
        | "TEXT" -> Ok JsonDataType.JString
        | "BOOLEAN" -> Ok JsonDataType.JBool
        | "INTEGER" -> Ok JsonDataType.JInt
        | _ -> Error <| UnexpectedFormatLine line
    
    let regexMatch = regex.Match line
    if regexMatch.Success
    then
        let jsonDataType = parseJsonDataType regexMatch.Groups.[3].Value
        match jsonDataType with
        | Ok x -> Ok <| FormatLine (regexMatch.Groups.[1].Value, System.Int32.Parse(regexMatch.Groups.[2].Value), x)
        | Error e -> Error e
    else Error <| UnexpectedFormatLine line