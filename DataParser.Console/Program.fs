open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite
open ResultMap

[<Literal>] 
let specFolderPath = "./specs"

[<Literal>]
let dataFolderPath = "./data"

[<Literal>]
let outputFolderPath = "./output"

let okHandler f = writeOutputFile outputFolderPath

let errorHandler key errors = eprintfn $"Error occurred during processing data file: {key}. Error is : %+A{errors}"

printfn "Reading spec files..."
let specs = ResultMap (readAllSpecFiles specFolderPath)

printfn "Retrieving data files..."
let dataFiles = readDataFiles dataFolderPath

printfn "Parsing data files..."
let parsedDateFileFormats = getDataFileFormats specs dataFiles
    
let dataFileParsedResults =
    ResultMap.bindResult parseDataFile parsedDateFileFormats

printfn "Writing to output..."
ResultMap.biIter okHandler errorHandler dataFileParsedResults

printfn "Processing complete. Press Enter to exit."
ignore <| Console.ReadLine()
