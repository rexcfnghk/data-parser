// For more information see https://aka.ms/fsharp-console-apps
open DataParser.Console.FileRead
open DataParser.Console.Core

let specs = readAllSpecFiles "./specs"

Map.values specs
|> Seq.iter (fun _ -> ignore (Result.mapError (fun e -> raise <| invalidOp $"{e}")))

printfn $"%A{specs}"