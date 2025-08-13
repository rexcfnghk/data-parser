[<AutoOpen>]
module TaskBuilder

open System.Threading.Tasks

type TaskBuilder() =
    member _.MergeSources (x, y) = task {
        let! _ = Task.WhenAll(x, y)
        return x.Result, y.Result
    }

    member _.Bind(x, f) = task {
        let! result = x
        return! f result
    }

    member _.Return x = task { return x }

    member _.Zero() = task { return () }


let task = TaskBuilder()
