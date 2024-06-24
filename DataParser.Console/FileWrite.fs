module DataParser.Console.FileWrite

open System.Text.Json
open System.IO
open DataParser.Console.DataFiles

let writeOutputFile folderPath (fileMap : DataFileParseResult) =
    let serializeElement (JsonObject jsonObject) =
        let serialized = JsonSerializer.Serialize jsonObject
        System.Text.Encoding.UTF8.GetBytes $"{serialized}\n"
        
    let writeBytes (stream: Stream) (bytes: byte array) = stream.Write bytes
    
    let createOutputFilePath  =
       (+) folderPath << sprintf "/%s" << formatOutputFileName
        
    ignore <| Directory.CreateDirectory folderPath
    let filePath = createOutputFilePath fileMap.DataFileName
    use fs = File.Open (filePath, FileMode.Create)
    Seq.iter (writeBytes fs << serializeElement) fileMap.JsonElements
