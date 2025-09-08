module Map

open System.Threading.Tasks

let traverseTask (f: 'b -> Task<'c>) =
    Map.fold (fun acc k v -> task { 
        let! t = f v 
        and! acc' = acc  
        return Map.add k t acc' }) (task { return Map.empty })
