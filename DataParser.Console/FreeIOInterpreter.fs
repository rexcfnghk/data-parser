namespace DataParser.Console

open System.Threading.Tasks
open DataParser.Console.FileRead

module FreeIOInterpreter =

    let rec interpret = function
        | Pure x -> Task.FromResult x
            | Free (ReadSpecs(folder, next)) -> task {
                  let! specs = readAllSpecFilesAsync folder
                  return! interpret (next specs)
            }
            | Free (ParseDataFile(df, next)) -> task {
                  let! r = parseDataFile df
                  return! interpret (next r)
            }
            | Free (WriteOutput(folder, result, next)) -> task {
                  do! DataParser.Console.FileWrite.writeOutputFileAsync folder result
                  return! interpret (next())
            }
            | Free (LogError(msg, next)) -> task {
                  eprintfn "%s" msg
                  return! interpret (next())
            }
            | Free (LogInfo(msg, next)) -> task {
                  printfn "%s" msg
                  return! interpret (next())
            }
