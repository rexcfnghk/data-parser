module DataParser.Console.FileWrite

open System.Text.Json
open System.IO
open DataParser.Console.DataFiles
open DataParser.Console.Core

let writeOutputFile folderPath (fileMap : Map<DataFileName,Map<string,obj> seq>) =
    let serializeAndWrite (DataFileName (FormatName format, rawDate)) (jsonElements: Map<string, obj> seq) =
        let serializeElement (stream: FileStream) (jsonObject : Map<string, obj>) =
            let serialized = JsonSerializer.Serialize jsonObject
            let bytes = System.Text.Encoding.UTF8.GetBytes $"{serialized}\n"
            stream.Write bytes
        
        ignore <| Directory.CreateDirectory folderPath
        use fs = File.Open (folderPath + $"/{format}_{rawDate}.ndjson", FileMode.Create)
        Seq.iter (serializeElement fs) jsonElements
    
    fileMap
    |> Map.iter serializeAndWrite
    
    

