module DataParser.Console.FileRead

open DataParser.Console.Result
open DataParser.Console.Core
open DataParser.Console.FormatFiles
open DataParser.Console.DataFiles
open System.IO

let readAllSpecFiles folderPath =
    Directory.GetFiles(folderPath, "*.csv")
    |> Array.map (fun x -> FormatName (Path.GetFileNameWithoutExtension x), parseFormatFile (File.ReadAllText x))
    |> Map.ofArray
    |> mapSequenceResult
    
let parseDataFile dataFile =
    File.ReadLines dataFile.FilePath
    |> seqTraverseResult (parseDataFileLine dataFile.FormatLines)
    |> Result.map (fun jsonObject -> { DataFileName = dataFile.Name; JsonElements = jsonObject })
    
let readDataFiles folderPath (fileFormatLookup: Map<FormatName, FormatLine list>) =
    Directory.GetFiles(folderPath, "*.txt")
    |> Seq.map (fun file -> file, Path.GetFileNameWithoutExtension file)
    |> seqTraverseResult (getDataFileFormat fileFormatLookup)
    |> Result.bind (seqTraverseResult parseDataFile)
