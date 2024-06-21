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

let errorHandler key errors =
    eprintfn $"Error occurred during processing data file: {key}. Error are : %+A{errors}"

printfn "Reading spec files..."
let specs = readAllSpecFiles SpecFolderPath

printfn "Retrieving data files..."
let dataFiles = readDataFiles DataFolderPath

printfn "Parsing data files..."
let parsedDateFileFormats = getDataFileFormats specs dataFiles
    
let dataFileParsedResults =
    ResultMap.bindResult parseDataFile parsedDateFileFormats

printfn "Writing to output..."
ResultMap.biIter okHandler errorHandler dataFileParsedResults

printfn "Processing complete. Press Enter to exit."
ignore <| Console.ReadLine()
