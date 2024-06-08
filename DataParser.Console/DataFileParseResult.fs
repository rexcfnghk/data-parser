namespace DataParser.Console

open DataParser.Console.DataFiles

type DataFileParseResult =
    { dataFileName : DataFileName
      jsonElements : JsonObject seq }
    
module DataFileParseResult =
    
    let iter f x = ignore (f x.dataFileName x.jsonElements)
