module DataParser.Tests.Helpers

open DataParser.Console.Core

let parseJsonType s =
    match s with
    | "TEXT" -> JsonDataType.JString
    | "BOOLEAN" -> JsonDataType.JBool
    | "INTEGER" -> JsonDataType.JInt
    | _ -> raise (invalidArg (nameof s) "Unable to parse JSON data type")