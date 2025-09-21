[<AutoOpen>]
module TaskOperators

open System.Threading.Tasks

let (<!>) f x = task {
    let! result = x
    return f result
}

let (<*>) (f: Task<'a -> 'b>) (x: Task<'a>) = task {
    let tasks = [|f :> Task; x :> Task|]
    let! _ = Task.WhenAll(tasks)
    return f.Result x.Result
}
