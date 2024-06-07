module DataParser.Console.Core

open System.Text.RegularExpressions

type FileFormat = FileFormat of string

type DataFileName = DataFileName of fileFormat: FileFormat * rawDate : string

type Error =
    | FileFormatNotFound of availableFormats: Set<FileFormat> * givenFormat : FileFormat
    | DataFileNameFormatError of fileName: string
    | UnexpectedFormatLine of string
    
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
    
