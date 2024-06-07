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
    
let readDataFiles fileFormatLookup folderPath =
    let getDataFileFormat (filePath: string) =
        let fileName = Path.GetFileNameWithoutExtension filePath
        result {
            let! (DataFileName (fileFormat, _)) = parseDataFileName fileName
            return! lookupFormatName fileFormatLookup fileFormat
        }

    Directory.GetFiles(folderPath, "*.txt")
    |> Array.toList
    |> traverse getDataFileFormat

