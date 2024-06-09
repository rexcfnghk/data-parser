namespace DataParser.Console

open DataParser.Console.DataFiles

type DataFileParseResult =
    { DataFileName : DataFileName
      JsonElements : JsonObject seq }
    
module DataFileParseResult =
    
    let iter f x = ignore (f x.DataFileName x.JsonElements)
