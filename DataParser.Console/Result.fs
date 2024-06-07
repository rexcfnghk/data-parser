module DataParser.Console.Result

type ResultBuilder() =
    member _.Bind(x, f) = Result.bind f x
    member _.Return x = Ok x
    member _.ReturnFrom x = x
    member _.Zero () = Ok ()
    member _.BindReturn(x, f) = Result.map f x
    member _.MergeSources(x, y) =
        match x, y with
        | Ok x, Ok y -> Ok (x, y)
        | Error e, _ -> Error e
        | _, Error e -> Error e
    
let result = ResultBuilder ()
