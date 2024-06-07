module DataParser.Console.Core

type FormatName = FormatName of string

type Error =
    | FileFormatNotFound of availableFormats: Set<FormatName> * givenFormat : FormatName
    | DataFileNameFormatError of fileName: string
    | UnexpectedFormatLine of string
    | UnparsableValue of obj
    
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
    
let inline sequence x  = traverse id x

let flip f x y = f y x
