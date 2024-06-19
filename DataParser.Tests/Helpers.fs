module DataParser.Tests.Helpers

open DataParser.Console.FormatFiles

let forceParseJsonType line s =
    match parseJsonDataType line s with
    | Ok x -> x
    | Error _ -> raise (invalidArg (nameof s) "Invalid json data type cannot be parsed")
