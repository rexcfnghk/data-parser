[<RequireQualifiedAccess>]
module Task

open System.Threading.Tasks

let runSynchronously (task: Task) = task.GetAwaiter().GetResult()

let map = (<!>)

let toUnit (x: Task) = task { do! x }

let fromUnit x = x :> Task

let liftA3 f x y z = f <!> x <*> y <*> z

let singleton x = task { return x }
