open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite
open DataParser.Console.Result

let [<Literal>] specPath = "./specs"
let [<Literal>] dataPath = "./data"
let [<Literal>] outputPath = "./output"

let program = result {
    printfn "Reading spec files..."
    let! specs = readAllSpecFiles specPath
    
    printfn "Parsing data files..."
    let! dataFiles = readDataFiles dataPath specs
    
    printfn "Writing to output files..."
    writeOutputFile outputPath dataFiles
}

match program with
| Error e -> eprintfn $"Error occurred during processing: {e}"
| Ok _ ->
    printfn "Output complete. Press Enter to exit."
    ignore <| Console.ReadLine()
