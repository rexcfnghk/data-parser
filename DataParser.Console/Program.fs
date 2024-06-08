// For more information see https://aka.ms/fsharp-console-apps
open DataParser.Console.FileRead
open DataParser.Console.FileWrite
open DataParser.Console.Core

let specs = readAllSpecFiles "./specs"

printfn $"%A{specs}"

match specs with
| Error e -> raise (invalidOp $"{e}")
| Ok specs ->
    specs
    |> flip readDataFiles "./data"
    |> Array.iter (function Error e -> raise (invalidOp $"{e}") | Ok result -> writeOutputFile "./output" result)
