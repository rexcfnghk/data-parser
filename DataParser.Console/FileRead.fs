module DataParser.Console.FileRead

open DataParser.Console.Core
open DataParser.Console.FormatFiles
open System.IO

let readAllSpecFiles folderPath =
    Directory.GetFiles(folderPath, "*.csv")
    |> Array.map (fun x -> FormatName (Path.GetFileNameWithoutExtension x), parseFormatFile (File.ReadAllText x))
    |> Map.ofArray

