namespace ResultMap

type ResultMap<'TKey, 'TOkValue, 'TErrorValue when 'TKey : comparison> =
    | ResultMap of Map<'TKey, Result<'TOkValue, 'TErrorValue list>>
   
    static member (+) (ResultMap x, ResultMap y) =
        Map.fold (fun s k v -> Map.add k v s) x y
        |> ResultMap
        
module ResultMap =

    let empty = ResultMap Map.empty

    let unResultMap (ResultMap m) = m

    let map f =
        ResultMap
        << Map.map (fun _ -> Result.map f)
        << unResultMap

    let traverseTask f (ResultMap m) = 
        let folder acc k v = task {
            let! acc' = acc
            match v with
            | Ok x -> 
                let! t = f x
                return Map.add k (Ok t) acc'
            | Error e -> 
                return Map.add k (Error e) acc'

        }
    
        Map.fold folder (task { return Map.empty }) m
        |> Task.map ResultMap
            
    let bindResult f =
        ResultMap
        << Map.map (fun _ -> Result.bind f)
        << unResultMap
        
    let keys (ResultMap x) = Set.ofSeq (Map.keys x)
    
    let tryFind key (ResultMap m) =
        Map.tryFind key m
    
    let biIter f g (ResultMap x) =
        let go k = function Ok v -> f k v | Error e -> g k e
        Map.iter go x
