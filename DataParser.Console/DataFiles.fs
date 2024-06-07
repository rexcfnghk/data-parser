module DataParser.Console.DataFiles

open DataParser.Console.Core
open System.Text.RegularExpressions

type DataFileName = DataFileName of fileFormat: FileFormat * rawDate : string


let lookupFileFormat availableFormats given =
    if Set.contains given availableFormats
    then Ok given
    else Error <| FileFormatNotFound (availableFormats, given)
    
let collectFileFormats = Set.ofList

let dataFileNameRegex =
    Regex(@"^(.+)_(\d\d\d\d-\d\d-\d\d)$", RegexOptions.Compiled ||| RegexOptions.Singleline ||| RegexOptions.CultureInvariant)

let parseDataFileName s =
    let regexMatch = dataFileNameRegex.Match s
    if regexMatch.Success
    then Ok <| DataFileName (FileFormat regexMatch.Groups[1].Value, regexMatch.Groups[2].Value)
    else Error <| DataFileNameFormatError s
    