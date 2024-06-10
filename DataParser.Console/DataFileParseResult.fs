namespace DataParser.Console

open DataParser.Console.DataFiles

type DataFileParseResult =
    { DataFileName : DataFileName
      JsonElements : seq<JsonObject> }
