﻿[<RequireQualifiedAccess>]
module Result

    let (<*>) f x =
        match f, x with
        | Ok f, Ok x -> Ok (f x)
        | Error e1, Error e2 -> Error (e1 @ e2)
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
        let folder k v s = Map.add <!> Ok k <*> f v <*> s
        Map.foldBack folder map (Ok Map.empty)
        
    let inline sequenceMap x = traverseMap id x

    let traverseSeq f seq =
        let folder x s = Seq.append <!> (Result.map Seq.singleton) (f x) <*> s
        Seq.foldBack folder seq (Ok Seq.empty)
        
    let inline sequenceSeq x = traverseSeq id x

    let biFoldMap_ f g = function
        | Ok x -> ignore <| f x
        | Error e -> ignore <| g e
