﻿module DataParser.Console.FileRead

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
    |> ResultMap
    
let parseDataFile dataFile =
    let (FilePath filePath) = dataFile.FilePath
    let dataFileLines = File.ReadLines filePath
    result {
        let! parsedJsonObjects =
            Result.traverseSeq (parseDataFileLine dataFile.FormatLines) dataFileLines
        return { DataFileName = dataFile.Name; JsonElements = parsedJsonObjects }
    }
    
let readDataFiles folderPath =
    seq {
        for file in Directory.GetFiles(folderPath, "*.txt") do
            (FilePath file, FileNameWithoutExtension <| Path.GetFileNameWithoutExtension file)
    }
    
let getDataFileFormats resultMap =
    let getDataFileFormat (filePath, fileName) formatLines =           
        result {
            let! dataFileName = parseDataFileName fileName
            return { Name = dataFileName; FormatLines = formatLines; FilePath = filePath }
        }
    
    let folder state (filePath, fileName)  =
        match parseDataFileName fileName with
        | Error e -> Map.add filePath (Error e) state
        | Ok (DataFileName (formatName, _)) ->
            let dataFileFormatResult = 
                match ResultMap.tryFind formatName resultMap with
                | Some formatLines ->
                    Result.bind (getDataFileFormat (filePath, fileName)) formatLines
                | None -> Error [ FileFormatNotFound (ResultMap.keys resultMap, formatName) ]
            Map.add filePath dataFileFormatResult state
            
    ResultMap << Seq.fold folder Map.empty
