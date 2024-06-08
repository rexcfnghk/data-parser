// For more information see https://aka.ms/fsharp-console-apps
open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite

let [<Literal>] specPath = "./specs"
let [<Literal>] dataPath = "./data"
let [<Literal>] outputPath = "./output"

let specs = readAllSpecFiles specPath

printfn $"%A{specs}"

match specs with
| Error e -> raise (invalidOp $"{e}")
| Ok specs ->
    specs
    |> readDataFiles dataPath
    |> Array.iter (function Error e -> raise (invalidOp $"{e}") | Ok result -> writeOutputFile outputPath result)
    
printfn "Output complete. Press Enter to exit."
ignore <| Console.ReadLine()