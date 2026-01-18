open System
open DataParser.Console.FreeIOOps
open DataParser.Console.FreeIOInterpreter
open DataParser.Console.FileRead
open ResultMap

[<Literal>] 
let SpecFolderPath = "./specs"

[<Literal>]
let DataFolderPath = "./data"

[<Literal>]
let OutputFolderPath = "./output"

let makeProgram specFolder dataFolder outputFolder =
    io {
        do! logInfo "Reading spec files..."
        let! specs = readSpecs specFolder

        let dataFileInfos = getDataFileInfos dataFolder

        do! logInfo "Parsing data files..."
        let dataFileFormats = getDataFileFormats specs dataFileInfos

        // iterate over entries and parse/write
        let (ResultMap m) = dataFileFormats
        for KeyValue(filePath, entry) in m do
            match entry with
            | Ok dataFileFormat ->
                let! parseResult = DataParser.Console.FreeIOOps.parseDataFile dataFileFormat
                match parseResult with
                | Ok parseRes -> do! writeOutput outputFolder parseRes
                | Error errs -> do! logError (sprintf "Error occurred during processing data file: %A. Errors are : %A" filePath errs)
            | Error errs ->
                do! logError (sprintf "Error occurred during processing data file: %A. Errors are : %A" filePath errs)

        do! logInfo "Processing complete."
    }

// run the program (when executed as an app)
interpret (makeProgram SpecFolderPath DataFolderPath OutputFolderPath)
|> Task.runSynchronously

printfn "Processing complete. Press Enter to exit."
ignore <| Console.ReadLine()
