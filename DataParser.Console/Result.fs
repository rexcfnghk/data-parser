module DataParser.Console.Result

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

let biFoldMap f g = function
    | Ok x -> ignore <| f x
    | Error e -> ignore <| g e

type ResultBuilder() =
    member _.Bind(x, f) = Result.bind f x
    member _.Return x = Ok x
    member _.ReturnFrom x = x
    member _.Zero () = Ok ()
    member _.BindReturn(x, f) = Result.map f x
    member _.MergeSources(x, y) =
        match x, y with
        | Ok x, Ok y -> Ok (x, y)
        | Error e1, Error e2 -> Error (e1 @ e2)
        | Error e, _ -> Error e
        | _, Error e -> Error e
    
let result = ResultBuilder ()
