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
    
let (<!>) f x = Result.map f x
    
let rec traverseList f list =
    let cons head tail = head :: tail
    match list with
    | [] -> Ok []
    | x :: xs -> cons <!> (f x) <*> (traverseList f xs)
    
let inline sequenceList x  = traverseList id x

let traverseMap f map =
    let folder s k v = Map.add <!> Ok k <*> f v <*> s
    Map.fold folder (Ok Map.empty) map
    
let inline sequenceMap x = traverseMap id x

let rec traverseSeq f seq =
    let folder s x = Seq.append <!> f x <*> s 
    Seq.fold folder (Ok Seq.empty) seq
    
let inline sequenceSeq x = traverseSeq id x

let flip f x y = f y x
