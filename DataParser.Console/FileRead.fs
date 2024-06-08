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
    
let readDataFiles folderPath (fileFormatLookup: Map<FormatName, FormatLine list>) =
    let getDataFileFormat output (filePath: string) =
        let fileName = Path.GetFileNameWithoutExtension filePath
        let fileFormatSet = fileFormatLookup.Keys |> Set.ofSeq
        result {
            let! dataFileName as DataFileName (fileFormat, _) = parseDataFileName fileName
            let! formatName = lookupFormatName fileFormatSet fileFormat
            let formatLines = fileFormatLookup[formatName]
            let parsed =
                File.ReadLines filePath
                |> Seq.map (parseDataFileLine formatLines)
                
            return! Map.add <!> Ok dataFileName <*> Ok parsed <*> output    
        }
        
    let mapJsonMapToDataFileParseResult (map: Map<DataFileName, JsonObject seq>) =
        Map.fold (fun s k v -> Seq.append s (Seq.singleton { dataFileName = k; jsonElements = v })) Seq.empty map

    Directory.GetFiles(folderPath, "*.txt")
    |> Array.fold getDataFileFormat (Ok Map.empty)
    |> Result.map mapJsonMapToDataFileParseResult
