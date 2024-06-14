open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite
open DataParser.Console.DataFiles
open DataParser.Console.Core

[<Literal>] 
let specPath = "./specs"

[<Literal>]
let dataPath = "./data"

[<Literal>]
let outputPath = "./output"

let okHandler = writeOutputFile outputPath

let errorHandler = List.iter (eprintfn "Error occurred during processing: %+A")

let program = result {
    printfn "Reading spec files..."
    let specs = readAllSpecFiles specPath
    
    printfn "Retrieving data files..."
    let dataFiles = getDataFiles dataPath
    
    printfn "Parsing data files..."
    let dataFileParsedResults = Seq.map (mapDataFilePath specs) dataFiles
    
    printfn "Writing to output..."
    Seq.iter (Result.biFoldMap okHandler errorHandler) dataFileParsedResults
    
    return ()
}

match program with
| Error e -> errorHandler e
| Ok _ -> ()

printfn "Processing complete. Press Enter to exit."
ignore <| Console.ReadLine()
