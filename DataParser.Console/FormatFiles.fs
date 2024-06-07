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
        let jsonDataType = parseJsonDataType line regexMatch.Groups.[3].Value
        match jsonDataType with
        | Ok x -> Ok <| FormatLine (regexMatch.Groups.[1].Value, Int32.Parse(regexMatch.Groups.[2].Value), x)
        | Error e -> Error e
    else Error <| UnexpectedFormatLine line

let parseFormatLineHeader (line: string) =
    let headerRegexLookup = dict [ ("\"column name\"", "(.+)"); ("width", "(\d+)"); ("datatype", "(.+)") ]
    let lines = line.Split(',')
    let regexes = Array.map (fun x -> headerRegexLookup.[x]) lines
    let regex = String.Join(',', regexes)
    Regex $"^{regex}$"
    
let (<*>) f x =
    match f, x with
    | Ok f, Ok x -> Ok (f x)
    | Error e, _ -> Error e
    | _, Error e -> Error e
    
let rec traverse f list =
    let cons head tail = head :: tail
    match list with
    | [] -> Ok []
    | x :: xs -> Ok cons <*> (f x) <*> (traverse f xs)
    
let parseFormatFile (file: string) =
    let lines = file.Split('\n', StringSplitOptions.TrimEntries ||| StringSplitOptions.RemoveEmptyEntries)
    let formatRegex = parseFormatLineHeader lines.[0]
    lines
    |> Array.skip 1
    |> Array.toList
    |> List.map (parseFormatLine formatRegex)
    |> traverse id