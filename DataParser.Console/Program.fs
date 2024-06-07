// For more information see https://aka.ms/fsharp-console-apps
open DataParser.Console.FileRead
open DataParser.Console.Result

let specs = readAllSpecFiles "./specs"

printfn $"%A{specs}"

readDataFiles