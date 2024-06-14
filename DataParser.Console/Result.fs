[<RequireQualifiedAccess>]
module Result

    let (<*>) f x =
        match f, x with
        | Ok f, Ok x -> Ok (f x)
        | Error e1, Error e2 -> Error (e1 @ e2)
        | Error e, _ -> Error e
        | _, Error e -> Error e
        
    let (<!>) = Result.map
        
    let rec traverseList f list =
        let cons head tail = head :: tail
        match list with
        | [] -> Ok []
        | x :: xs -> cons <!> f x <*> traverseList f xs
        
    let sequenceList x  = traverseList id x

    let traverseMap f map =
        let folder k v s = Map.add <!> Ok k <*> f v <*> s
        Map.foldBack folder map (Ok Map.empty)
        
    let sequenceMap x = traverseMap id x 

    let traverseSeq f seq =
        let folder x s = Seq.append <!> Result.map Seq.singleton (f x) <*> s
        Seq.foldBack folder seq (Ok Seq.empty)
        
    let sequenceSeq x = traverseSeq id x

    let biFoldMap f g = function
        | Ok x -> f x
        | Error e -> g e
