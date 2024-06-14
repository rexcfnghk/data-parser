open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite

[<Literal>] 
let specFolderPath = "./specs"

[<Literal>]
let dataFolderPath = "./data"

[<Literal>]
let outputFolderPath = "./output"

let okHandler = writeOutputFile outputFolderPath

let errorHandler = List.iter (eprintfn "Error occurred during processing: %+A")

printfn "Reading spec files..."
let specs = readAllSpecFiles specFolderPath

printfn "Retrieving data files..."
let dataFiles = readDataFiles dataFolderPath

printfn "Parsing data files..."
let dataFileParsedResults = Seq.map (mapDataFilePath specs) dataFiles

printfn "Writing to output..."
Seq.iter (Result.biFoldMap okHandler errorHandler) dataFileParsedResults

printfn "Processing complete. Press Enter to exit."
ignore <| Console.ReadLine()
