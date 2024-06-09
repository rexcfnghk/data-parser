module DataParser.Console.FormatFiles

open System
open System.Collections.Generic
open System.Text.RegularExpressions
open Core

type JsonDataType = JString | JBool | JInt

type FormatLine = FormatLine of columnName: string * width: int * dataType : JsonDataType

let parseJsonDataType line = function
    | "TEXT" -> Ok JsonDataType.JString
    | "BOOLEAN" -> Ok JsonDataType.JBool
    | "INTEGER" -> Ok JsonDataType.JInt
    | _ -> Error [UnexpectedFormatLine line]

let parseFormatLine (regex: Regex) line =
    let regexMatch = regex.Match line
    if regexMatch.Success
    then
        let jsonDataType = parseJsonDataType line regexMatch.Groups["type"].Value
        match jsonDataType with
        | Ok x -> Ok <| FormatLine (regexMatch.Groups["name"].Value, Int32.Parse(regexMatch.Groups["width"].Value), x)
        | Error e -> Error e
    else Error [UnexpectedFormatLine line]

let parseFormatLineHeader (line: string) =
    let headerLookup (dict : IDictionary<string, string>) v =
        match dict.TryGetValue v with
        | true, s -> Ok s
        | false, _ -> Error [UnexpectedFormatHeader line]
        
    let buildRegex (regexStrings: string seq) =
        let joined = String.Join(',', regexStrings)
        Regex $"^{joined}$"
    
    let headerRegexLookup = dict [ ("\"column name\"", "(?<name>.+)"); ("width", "(?<width>\d+)"); ("datatype", "(?<type>.+)") ]
    let lines = line.Split(',')
    let regexes = seqTraverseResult (headerLookup headerRegexLookup) lines
    Result.map buildRegex regexes
    
let parseFormatFile (file: string) =
    let lines = file.Split('\n', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)
    try
        let formatRegex = parseFormatLineHeader lines[0]
        let formatFileLines =
            lines
            |> Array.skip 1
            |> Array.toList
        
        if formatFileLines = []
        then Error [UnparsableFormatFile file]
        else formatFileLines |> listTraverseResult (fun s -> Result.bind (flip parseFormatLine s) formatRegex)
        
    with
        | :? IndexOutOfRangeException -> Error [UnparsableFormatFile file]
        