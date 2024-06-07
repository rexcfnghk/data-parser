// For more information see https://aka.ms/fsharp-console-apps
open DataParser.Console.FileRead
open DataParser.Console.Core

let specs = readAllSpecFiles "./specs"

printfn $"%A{specs}"

let r =
    specs.Keys
    |> Set.ofSeq
    |> flip readDataFiles "./data"

printfn $"%A{r}"
