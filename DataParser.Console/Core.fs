module DataParser.Console.Core

type FormatName = FormatName of string

type FileNameWithoutExtension = FileNameWithoutExtension of string

type Error =
    | FileFormatNotFound of availableFormats: Set<FormatName> * givenFormat : FormatName
    | DataFileNameFormatError of fileName: FileNameWithoutExtension
    | UnexpectedFormatHeader of string
    | UnexpectedFormatLine of string
    | UnparsableValue of string
    | DataFileLineLengthShorterThanSpec of string
    | UnparsableFormatFile of fileContent: string
    
let flip f x y = f y x
