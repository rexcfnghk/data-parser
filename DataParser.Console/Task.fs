[<RequireQualifiedAccess>]
module Task

open System.Threading.Tasks

let map = (<!>)

let toUnit (x: Task) = task {
    do! x
    return ()
}

let fromUnit (x: unit) = Task.FromResult x :> Task

let liftA3 f x y z = f <!> x <*> y <*> z

let singleton x = task { return x }
