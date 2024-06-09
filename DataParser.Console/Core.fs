module DataParser.Console.Core

type FormatName = FormatName of string

type Error =
    | FileFormatNotFound of availableFormats: Set<FormatName> * givenFormat : FormatName
    | DataFileNameFormatError of fileName: string
    | UnexpectedFormatHeader of string
    | UnexpectedFormatLine of string
    | UnparsableValue of obj
    | UnparsableFormatFile of fileContent: string
    
let (<*>) f x =
    match f, x with
    | Ok f, Ok x -> Ok (f x)
    | Error e1, Error e2 -> Error (e1 @ e2)
    | Error e, _ -> Error e
    | _, Error e -> Error e
    
let (<!>) f x = Result.map f x
    
let rec listTraverseResult f list =
    let cons head tail = head :: tail
    match list with
    | [] -> Ok []
    | x :: xs -> cons <!> (f x) <*> (listTraverseResult f xs)
    
let inline listSequenceResult x  = listTraverseResult id x

let mapTraverseResult f map =
    let folder k v s = Map.add <!> Ok k <*> f v <*> s
    Map.foldBack folder map (Ok Map.empty)
    
let inline mapSequenceResult x = mapTraverseResult id x

let seqTraverseResult f seq =
    let folder x s = Seq.append <!> (Result.map Seq.singleton) (f x) <*> s
    Seq.foldBack folder seq (Ok Seq.empty)
    
let inline seqSequenceResult x = seqTraverseResult id x

let flip f x y = f y x
