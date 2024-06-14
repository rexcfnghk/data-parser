open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite

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
    let! specs = readAllSpecFiles specPath
    
    printfn "Parsing data files..."
    let dataFiles = parseDataFiles dataPath specs
    
    printfn "Writing to output..."
    Seq.iter (Result.biFoldMap okHandler errorHandler) dataFiles
}

match program with
| Error e -> errorHandler e
| Ok _ -> ()

printfn "Processing complete. Press Enter to exit."
ignore <| Console.ReadLine()
