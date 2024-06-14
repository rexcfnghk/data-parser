module DataParser.Console.FileRead

open DataParser.Console.Core
open DataParser.Console.FormatFiles
open DataParser.Console.DataFiles
open System.IO

let readAllSpecFiles folderPath =
    let createFormatFileTuple (filePath: string) =
        FormatName (Path.GetFileNameWithoutExtension filePath),
        parseFormatFile (File.ReadAllText filePath)
    
    Directory.GetFiles(folderPath, "*.csv")
    |> Array.map createFormatFileTuple
    |> Map.ofArray
    
let parseDataFile dataFile =
    let dataFileLines = File.ReadLines dataFile.FilePath
    result {
        let! parsedJsonObjects =
            Result.traverseSeq (parseDataFileLine dataFile.FormatLines) dataFileLines
        return { DataFileName = dataFile.Name; JsonElements = parsedJsonObjects }
    }
    
let getDataFileFormat formatLines (filePath, fileName) =           
    result {
        let! dataFileName = parseDataFileName fileName
        return { Name = dataFileName; FormatLines = formatLines; FilePath = filePath }
    }
    
let getDataFiles folderPath =
    seq {
        for file in Directory.GetFiles(folderPath, "*.txt") do
            (file, Path.GetFileNameWithoutExtension file)
    }
    
let mapDataFilePath specs (filePath, fileName) =
    let getSuccessfulKeys =
        Map.filter (fun _ -> Result.isOk)
        >> Map.keys
    
    result {
        let! DataFileName (dataFileName, _) = parseDataFileName fileName
        let! dataFileFormat =
            match Map.tryFind dataFileName specs with
            | Some formatLines -> Result.bind (flip getDataFileFormat (filePath, fileName)) formatLines
            | None -> Error [FileFormatNotFound (getSuccessfulKeys specs, dataFileName)]
        return! parseDataFile dataFileFormat
    }
