open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite
open ResultMap

[<Literal>] 
let SpecFolderPath = "./specs"

[<Literal>]
let DataFolderPath = "./data"

[<Literal>]
let OutputFolderPath = "./output"

let okHandler _ = writeOutputFile OutputFolderPath

let errorHandler filePath errors =
    eprintfn $"Error occurred during processing data file: {filePath}. Errors are : %+A{errors}"

let consolidateResults (ResultMap dataFileFormats) =
    let folder acc k v =
        match v with
        | Ok dataFileFormat ->
            task {
                let! parseResult = parseDataFile dataFileFormat
                match parseResult with
                | Ok result ->
                    return! Task.liftA3 Map.add (Task.singleton k) (Task.singleton (Ok result)) acc
                | Error e ->
                    return! Task.liftA3 Map.add (Task.singleton k) (Task.singleton (Error e)) acc
            }
        | Error e -> 
            task {
                return! Task.liftA3 Map.add (Task.singleton k) (Task.singleton (Error e)) acc
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

        printfn "Writing to output folder..."
        ResultMap.biIter okHandler errorHandler consolidatedResults

        printfn "Processing complete. Press Enter to exit."
        ignore <| Console.ReadLine()
    }

t.GetAwaiter().GetResult()
