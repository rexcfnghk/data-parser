open System
open System.Threading.Tasks
open DataParser.Console.FileRead
open DataParser.Console.FileWrite
open ResultMap

[<Literal>] 
let SpecFolderPath = "./specs"

[<Literal>]
let DataFolderPath = "./data"

[<Literal>]
let OutputFolderPath = "./output"

let okHandler _ = writeOutputFileAsync OutputFolderPath

let errorHandler filePath errors =
    eprintfn $"Error occurred during processing data file: {filePath}. Errors are : %+A{errors}"

let consolidateResults (ResultMap dataFileFormats) =
    let folder acc k = function
        | Ok dataFileFormat ->
            task {
                let! parseResult = parseDataFile dataFileFormat
                return! Map.add <!> Task.singleton k <*> Task.singleton parseResult <*> acc
            }
        | Error e -> 
            task {
                return! Map.add <!> Task.singleton k <*> Task.singleton (Error e) <*> acc
            }
    
    Map.fold folder (Task.singleton Map.empty) dataFileFormats
    |> Task.map ResultMap

printfn "Reading spec files..."

let t =
    task {
        let! specs = readAllSpecFiles SpecFolderPath

        let dataFileInfos = getDataFileInfos DataFolderPath

        printfn "Parsing data files..."
        let dataFileFormats = getDataFileFormats specs dataFileInfos

        let! consolidatedResults = consolidateResults dataFileFormats

        let result = ResultMap.either okHandler ((<<) Task.fromUnit << errorHandler) consolidatedResults

        printfn "Writing to output folder..."
        do! Task.WhenAll(Map.values result)

        printfn "Processing complete. Press Enter to exit."
        ignore <| Console.ReadLine()
    }

t.GetAwaiter().GetResult()
