﻿open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite
open DataParser.Console.Result

[<Literal>] 
let specPath = "./specs"

[<Literal>]
let dataPath = "./data"

[<Literal>]
let outputPath = "./output"

let okHandler = writeOutputFile outputPath

let errorHandler = List.iter (eprintfn "Error occurred during processing: %+A")

result {
    printfn "Reading spec files..."
    let! specs = readAllSpecFiles specPath
    
    printfn "Parsing data files..."
    let dataFiles = parseDataFiles dataPath specs
    
    printfn "Writing to output..."
    Seq.iter (biFoldMap_ okHandler errorHandler) dataFiles
} |> ignore

printfn "Processing complete. Press Enter to exit."
ignore <| Console.ReadLine()
