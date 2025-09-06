namespace DataParser.Console

open DataParser.Console.DataFiles

type DataFileParseResult =
    { DataFilePath : FilePath
      DataFileName : DataFileName
      JsonElements : seq<JsonObject> }
