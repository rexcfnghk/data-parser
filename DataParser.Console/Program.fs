// For more information see https://aka.ms/fsharp-console-apps
open DataParser.Console.FileRead

let specs = readAllSpecFiles "./specs"

printfn $"%A{specs}"