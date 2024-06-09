open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite
open DataParser.Console.Result

let [<Literal>] specPath = "./specs"
let [<Literal>] dataPath = "./data"
let [<Literal>] outputPath = "./output"

let okHandler = writeOutputFile outputPath

let errorHandler = List.iter (eprintfn "Error occurred during processing: %A")

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
