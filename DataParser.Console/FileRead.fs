module DataParser.Console.FileRead

open DataParser.Console.Result
open DataParser.Console.Core
open DataParser.Console.FormatFiles
open DataParser.Console.DataFiles
open System.IO

type DataFile =
    { filePath: string
      name: DataFileName
      formatLines: FormatLine list }

let readAllSpecFiles folderPath =
    Directory.GetFiles(folderPath, "*.csv")
    |> Array.map (fun x -> FormatName (Path.GetFileNameWithoutExtension x), parseFormatFile (File.ReadAllText x))
    |> Map.ofArray
    |> mapSequenceResult
    
let getDataFile (fileFormatLookup: Map<FormatName, FormatLine list>) (filePath, fileName) =
    let fileFormatSet = fileFormatLookup.Keys |> Set.ofSeq
    result {
        let! dataFileName as DataFileName (fileFormat, _) = parseDataFileName fileName
        let! formatName = lookupFormatName fileFormatSet fileFormat
        return { name = dataFileName; formatLines = fileFormatLookup[formatName]; filePath = filePath }
    }
    
let parseDataFile dataFile =
    File.ReadLines dataFile.filePath
    |> seqTraverseResult (parseDataFileLine dataFile.formatLines)
    |> Result.map (fun jsonObject -> { dataFileName = dataFile.name; jsonElements = jsonObject })
    
let readDataFiles folderPath (fileFormatLookup: Map<FormatName, FormatLine list>) =
    Directory.GetFiles(folderPath, "*.txt")
    |> Seq.map (fun file -> file, Path.GetFileNameWithoutExtension file)
    |> seqTraverseResult (getDataFile fileFormatLookup)
    |> Result.bind (seqTraverseResult parseDataFile)
