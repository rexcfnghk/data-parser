[<RequireQualifiedAccess>]
module Task

open System.Threading.Tasks

let map f x = task {
    let! result = x
    return f result
}

let (<!>) = map

let (<*>) (f: Task<'a -> 'b>) (x: Task<'a>) = task {
    let! _ = Task.WhenAll(f, x) :?> Task<unit>
    return f.Result x.Result
}

let liftA2 f x y = f <!> x <*> y

let traverseSeq f xs = 
    let cons x xs = x :: xs
    let (<%>) = liftA2 cons
    Seq.fold (fun acc x -> f x <%> acc) (task { return [] }) xs