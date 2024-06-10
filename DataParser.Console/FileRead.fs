module DataParser.Console.FileRead

open DataParser.Console.Core
open DataParser.Console.FormatFiles
open DataParser.Console.DataFiles
open System.IO

let readAllSpecFiles folderPath =
    Directory.GetFiles(folderPath, "*.csv")
    |> Array.map (fun x -> FormatName (Path.GetFileNameWithoutExtension x), parseFormatFile (File.ReadAllText x))
    |> Map.ofArray
    |> Result.sequenceMap
    
let parseDataFile dataFile =
    File.ReadLines dataFile.FilePath
    |> Result.traverseSeq (parseDataFileLine dataFile.FormatLines)
    |> Result.map (fun jsonObject -> { DataFileName = dataFile.Name; JsonElements = jsonObject })
    
let parseDataFiles folderPath (fileFormatLookup: Map<FormatName, FormatLine list>) =
    seq {
        for file in Directory.GetFiles(folderPath, "*.txt") do
            let f = file, Path.GetFileNameWithoutExtension file
            
            result {
                let! dataFileFormat = getDataFileFormat fileFormatLookup f
                return! parseDataFile dataFileFormat
            }
    }
