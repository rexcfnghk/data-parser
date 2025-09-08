namespace Microsoft.FSharp.Control

open System.Threading.Tasks

type TaskBuilder () =
    member _.MergeSources (x: Task<'a>, y: Task<'b>) = task {
        let! _ = Task.WhenAll(x :> Task, y :> Task)
        return x.Result, y.Result
    }

    member _.Bind (p: Task<'a>, k: 'a -> Task<'b>) : Task<'b> = task {
        let! v = p
        return! k v
    }

    member _.Return (v: 'a) : Task<'a> = Task.FromResult v


module TaskBuilder =
    let task = TaskBuilder()
