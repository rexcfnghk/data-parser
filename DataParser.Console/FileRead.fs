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
    |> sequenceMap
    
let readDataFiles (fileFormatLookup: Map<FormatName, FormatLine list>) folderPath =
    let getDataFileFormat output (filePath: string) =
        let fileName = Path.GetFileNameWithoutExtension filePath
        let fileFormatSet = fileFormatLookup.Keys |> Set.ofSeq
        result {
            let! (dataFileName as DataFileName (fileFormat, _)) = parseDataFileName fileName
            let! formatName = lookupFormatName fileFormatSet fileFormat
            let formatLines = fileFormatLookup[formatName]
            return
                File.ReadLines filePath
                // TODO: Fix
                |> Seq.map (parseDataFileLine formatLines)
                |> flip (Map.add dataFileName) output
        }

    Directory.GetFiles(folderPath, "*.txt")
    |> Array.map (getDataFileFormat Map.empty)

