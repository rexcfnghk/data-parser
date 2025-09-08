[<RequireQualifiedAccess>]
module Task

open System.Threading.Tasks

let map f x = task {
    let! result = x
    return f result
}

let bind f x = task {
    let! result = x
    return! f result
}

let toUnit (x: Task) = task {
    do! x
    return ()
}

let (<!>) = map

let (<*>) (f: Task<'a -> 'b>) (x: Task<'a>) = task {
    let tasks = [|f :> Task; x :> Task|]
    let! _ = Task.WhenAll(tasks)
    return f.Result x.Result
}

let liftA3 f x y z = f <!> x <*> y <*> z

let singleton x = task { return x }
