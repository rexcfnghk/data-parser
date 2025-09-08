[<AutoOpen>]
module TaskBuilder

open System.Threading.Tasks

type TaskBuilder() =
    member _.MergeSources (x: Task<'a>, y: Task<'b>) = task {
        let! _ = Task.WhenAll(x :> Task, y :> Task)
        return x.Result, y.Result
    }

    member _.Bind(x, f) = task {
        let! result = x
        return! f result
    }

    // Bind overload to support awaiting a non-generic Task (do! someTask)
    member _.Bind(x: Task, f: unit -> Task<'T>) : Task<'T> =
        let tcs = new TaskCompletionSource<'T>()
        x.ContinueWith(fun (t: Task) ->
            if t.IsFaulted then tcs.SetException(t.Exception.InnerExceptions)
            elif t.IsCanceled then tcs.SetCanceled()
            else
                try
                    let next = f()
                    next.ContinueWith(fun (n: Task<'T>) ->
                        if n.IsFaulted then tcs.SetException(n.Exception.InnerExceptions)
                        elif n.IsCanceled then tcs.SetCanceled()
                        else tcs.SetResult(n.Result)
                    ) |> ignore
                with ex -> tcs.SetException(ex)
        ) |> ignore
        tcs.Task

    // Return helpers so the computation expression can produce tasks directly
    member _.Return(x: 'T) = Task.FromResult x
    member _.ReturnFrom(x: Task<'T>) = x
    member _.ReturnFrom(x: Task) = x
    member _.Zero() = Task.FromResult ()

let task = TaskBuilder()
