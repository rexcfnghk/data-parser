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
    |> Result.sequenceMap
    
let parseDataFile dataFile =
    let dataFileLines = File.ReadLines dataFile.FilePath
    result {
        let! parsedJsonObjects =
            Result.traverseSeq (parseDataFileLine dataFile.FormatLines) dataFileLines
        return { DataFileName = dataFile.Name; JsonElements = parsedJsonObjects }
    }
    
let parseDataFiles folderPath (fileFormatLookup: Map<FormatName, FormatLine list>) =
    let getDataFileFormat (fileFormatLookup: Map<FormatName, FormatLine list>) (filePath, fileName) =           
        result {
            let! dataFileName as DataFileName (fileFormat, _) = parseDataFileName fileName
            let! formatLines = tryLookupFormatLines fileFormatLookup fileFormat
            return { Name = dataFileName; FormatLines = formatLines; FilePath = filePath }
        }
    
    seq {
        for file in Directory.GetFiles(folderPath, "*.txt") do
            let f = file, Path.GetFileNameWithoutExtension file
            
            result {
                let! dataFileFormat = getDataFileFormat fileFormatLookup f
                return! parseDataFile dataFileFormat
            }
    }
