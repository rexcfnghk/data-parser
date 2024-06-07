module DataParser.Console.FormatFiles

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
        | Ok x -> Ok <| FormatLine (regexMatch.Groups.[1].Value, System.Int32.Parse(regexMatch.Groups.[2].Value), x)
        | Error e -> Error e
    else Error <| UnexpectedFormatLine line

