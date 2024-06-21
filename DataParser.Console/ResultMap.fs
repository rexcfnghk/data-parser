namespace ResultMap

type ResultMap<'TKey, 'TOkValue, 'TErrorValue when 'TKey : comparison> =
    ResultMap of Map<'TKey, Result<'TOkValue, 'TErrorValue list>>
    
module ResultMap =

    let map f (ResultMap x) =
        x
        |> Map.map (fun _ -> Result.map f)
        |> ResultMap
    
    let bindResult f (ResultMap x) =
        x
        |> Map.map (fun _ -> Result.bind f)
        |> ResultMap
        
    let keys (ResultMap x) = Set.ofSeq (Map.keys x)
    
    let tryFind key (ResultMap m) =
        Map.tryFind key m
    
    let biIter f g (ResultMap x) =
        let go k = function Ok v -> f k v | Error e -> g k e
        Map.iter go x
