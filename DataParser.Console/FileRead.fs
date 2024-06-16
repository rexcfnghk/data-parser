module DataParser.Console.FileRead

open DataParser.Console.Core
open DataParser.Console.FormatFiles
open DataParser.Console.DataFiles
open System.IO
open ResultMap

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
    
let readDataFiles folderPath =
    seq {
        for file in Directory.GetFiles(folderPath, "*.txt") do
            (file, Path.GetFileNameWithoutExtension file)
    }
    
let getDataFileFormats resultMap =
    let folder state (filePath, fileName)  =
        match parseDataFileName fileName with
        | Error e -> Map.add filePath (Error e) state
        | Ok (DataFileName (formatName, _)) ->
            let dataFileFormatResult = 
                match ResultMap.tryFind formatName resultMap with
                | Some formatLines ->
                    Result.bind (flip getDataFileFormat (filePath, fileName)) formatLines
                | None -> Error [FileFormatNotFound (ResultMap.keys resultMap, formatName)]
            Map.add filePath dataFileFormatResult state
            
    ResultMap << Seq.fold folder Map.empty
    