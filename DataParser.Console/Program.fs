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

printfn "Reading spec files..."
let specs = readAllSpecFiles SpecFolderPath

let dataFiles = readDataFiles DataFolderPath

printfn "Parsing data files..."
let parsedDateFileFormats = getDataFileFormats specs dataFiles
    
let dataFileParsedResults =
    ResultMap.bindResult parseDataFile parsedDateFileFormats

printfn "Writing to output folder..."
ResultMap.biIter okHandler errorHandler dataFileParsedResults

printfn "Processing complete. Press Enter to exit."
ignore <| Console.ReadLine()
