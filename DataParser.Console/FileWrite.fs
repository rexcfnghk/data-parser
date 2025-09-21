module DataParser.Console.FileWrite

open System
open System.Text.Json
open System.Threading.Tasks
open System.IO
open DataParser.Console.DataFiles

let newLineUtf8 = System.Text.Encoding.UTF8.GetBytes Environment.NewLine

let writeOutputFileAsync folderPath (fileMap : DataFileParseResult) =
    let serializeElement (JsonObject jsonObject) =
        let serialized = JsonSerializer.SerializeToUtf8Bytes jsonObject
        Array.concat [| serialized; newLineUtf8 |]
        
    let writeBytesAsync (stream: Stream) (bytes: byte array) =
        bytes
        |> stream.WriteAsync
        |> _.AsTask()
    
    let createOutputFilePath  =
       (+) folderPath << sprintf "/%s" << formatOutputFileName
        
    ignore <| Directory.CreateDirectory folderPath
    let filePath = createOutputFilePath fileMap.DataFileName
    use fs = File.Open (filePath, FileMode.Create)
    let tasks = Seq.map (writeBytesAsync fs << serializeElement) fileMap.JsonElements
    Task.WhenAll tasks
