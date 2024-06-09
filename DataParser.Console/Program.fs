open System
open DataParser.Console.FileRead
open DataParser.Console.FileWrite
open DataParser.Console.Result

let [<Literal>] specPath = "./specs"
let [<Literal>] dataPath = "./data"
let [<Literal>] outputPath = "./output"

let successHandler =
    printfn "Writing to output files..."
    writeOutputFile outputPath

let failureHandler = List.iter (eprintfn "Error occurred during processing %A")

let program = result {
    printfn "Reading spec files..."
    let! specs = readAllSpecFiles specPath
    
    printfn "Parsing data files..."
    let dataFiles = readDataFiles dataPath specs
    
    Seq.iter (biFoldMap successHandler failureHandler) dataFiles
}



match program with
| Error e -> raise (invalidOp $"Error occurred during processing: {e}")
| Ok _ ->
    printfn "Output complete. Press Enter to exit."
    ignore <| Console.ReadLine()
