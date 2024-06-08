module DataParser.Console.FileWrite

open System.Text.Json
open System.IO
open DataParser.Console.DataFiles
open DataParser.Console.Core

let writeOutputFile folderPath (fileMap : DataFileParseResult seq) =
    let serializeAndWrite (DataFileName (FormatName format, rawDate)) (jsonElements: JsonObject seq) =
        let serializeElement (stream: FileStream) (jsonObject : JsonObject) =
            let serialized = JsonSerializer.Serialize jsonObject
            let bytes = System.Text.Encoding.UTF8.GetBytes $"{serialized}\n"
            stream.Write bytes
        
        ignore <| Directory.CreateDirectory folderPath
        use fs = File.Open (folderPath + $"/{format}_{rawDate}.ndjson", FileMode.Create)
        Seq.iter (serializeElement fs) jsonElements
    
    fileMap
    |> Seq.iter (DataFileParseResult.iter serializeAndWrite)
    