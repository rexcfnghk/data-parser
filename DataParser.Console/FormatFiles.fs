module DataParser.Console.FormatFiles

open System
open System.Text.RegularExpressions
open Core

type JsonDataType = JString | JBool | JInt

type FormatLine = FormatLine of columnName: string * width: int * dataType : JsonDataType

let parseJsonDataType line = function
    | "TEXT" -> Ok JsonDataType.JString
    | "BOOLEAN" -> Ok JsonDataType.JBool
    | "INTEGER" -> Ok JsonDataType.JInt
    | _ -> Error <| UnexpectedFormatLine line

let parseFormatLine (regex: Regex) line =
    let regexMatch = regex.Match line
    if regexMatch.Success
    then
        let jsonDataType = parseJsonDataType line regexMatch.Groups["type"].Value
        match jsonDataType with
        | Ok x -> Ok <| FormatLine (regexMatch.Groups["name"].Value, Int32.Parse(regexMatch.Groups["width"].Value), x)
        | Error e -> Error e
    else Error <| UnexpectedFormatLine line

let parseFormatLineHeader (line: string) =
    let headerRegexLookup = dict [ ("\"column name\"", "(?<name>.+)"); ("width", "(?<width>\d+)"); ("datatype", "(?<type>.+)") ]
    let lines = line.Split(',')
    let regexes = Array.map (fun x -> headerRegexLookup[x]) lines
    let regex = String.Join(',', regexes)
    Regex $"^{regex}$"
    
let parseFormatFile (file: string) =
    let lines = file.Split('\n', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)
    try
        let formatRegex = parseFormatLineHeader lines[0]
        lines
        |> Array.skip 1
        |> Array.toList
        |> List.map (parseFormatLine formatRegex)
        |> sequenceList
    with
        | :? IndexOutOfRangeException -> Error (UnparsableFormatFile file)
        