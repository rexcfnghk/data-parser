module DataParser.Tests.Main

open System
open System.Collections.Generic
open System.Globalization
open System.Text.RegularExpressions
open DataParser.Console.Core
open DataParser.Console.DataFiles
open DataParser.Console.FormatFiles
open DataParser.Tests.Helpers
open Hedgehog
open Hedgehog.Xunit
open Swensen.Unquote

[<Property>]
let ``DataFileNames has structural equality`` (ff: FormatName) (s: string) =
    DataFileName (ff, s) =! DataFileName (ff, s)
    
[<Property>]
let ``Unfound format returns false when element does not exist in set`` (xs: Set<FormatName>) (x: FormatName) =
    // Arrange
    let sut = Set.remove x xs
    
    // Act Assert
    test <@ not <| Set.contains x sut @>
    
[<Property>]
let ``lookupFormatName returns FormatNotFound error when element does not exist in set`` (xs: Set<FormatName>) (x: FormatName) =
    // Arrange
    let sut = Set.remove x xs
    let expected = Error <| FileFormatNotFound (sut, x)
    
    // Act Assert
    lookupFormatName sut x =! expected
    
[<Property>]
let ``lookupFormatName returns Ok when element exists in set`` (xs: Set<FormatName>) (x: FormatName) =
    // Arrange
    let sut = Set.add x xs
    let expected = Ok x
    
    // Act Assert
    lookupFormatName sut x =! expected
    
[<Property>]
let ``FormatLines are compared with structural equality`` (columnName: string) (width: int) (dataType: JsonDataType) =
    FormatLine (columnName, width, dataType) =! FormatLine (columnName, width, dataType)
    
[<Property>]
let ``collectFormatNames should return a set with every file format in the input list`` (xs: FormatName list) =
    let resultSet = Set.ofList xs
    
    test <@ Set.isSubset resultSet (collectFormatNames xs) @>
    
[<Property>]
let ``parseDataFileName should return error when file name does not conform to expected format`` (s: string) =
    let expected = Error <| DataFileNameFormatError s
    
    parseDataFileName s =! expected
    
[<Property>]
let ``parseDataFileName should return ok with expected fields when file conform to expected format`` (s: int) (d: DateTime) =
    let rawDate = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
    let fileName = $"{s}_{rawDate}"
    let FormatName = FormatName $"{s}"
    let expected = Ok <| DataFileName (FormatName, rawDate)
    
    parseDataFileName fileName =! expected
    
[<Property>]
let ``parseFormatLine returns error when line does not conform to expected format`` (s: string) =
    let expected = Error <| UnexpectedFormatLine s
    let regex = Regex("^\n$")
    
    parseFormatLine regex s =! expected
    
[<Xunit.Theory>]
[<Xunit.InlineData("name", 10, "TEXT")>]
[<Xunit.InlineData("valid", 1, "BOOLEAN")>]
[<Xunit.InlineData("count", 3, "INTEGER")>]
let ``parseFormatLine returns expected FormatLine when line conforms to regex`` (columnName, width, dataType) =
    let line = $"{columnName},{width},{dataType}"
    let jsonType = forceParseJsonType line dataType
    let expected = Ok <| FormatLine (columnName, width, jsonType)
    let regex = Regex("^(?<name>.+),(?<width>\d+),(?<type>.+)$")
    
    parseFormatLine regex line =! expected
    
[<Xunit.Theory>]
[<Xunit.InlineData("\"column name\",width,datatype", "^(?<name>.+),(?<width>\d+),(?<type>.+)$")>]
[<Xunit.InlineData("width,\"column name\",datatype", "^(?<width>\d+),(?<name>.+),(?<type>.+)$")>]
[<Xunit.InlineData("datatype,width,\"column name\"", "^(?<type>.+),(?<width>\d+),(?<name>.+)$")>] 
let ``parseFormatLineHeader returns expected regex when line conforms to regex`` (header, expectedRegex) =
    let expected = Regex(expectedRegex)

    $"{parseFormatLineHeader header}" =! $"{expected}"
    
[<Xunit.Theory>]
[<Xunit.InlineData("\"column name\",width,datatype\nname,10,TEXT\n")>]
[<Xunit.InlineData("width,\"column name\",datatype\n10,name,TEXT\n")>]
let ``parseFormatFile returns expected FormatLines when there is one line`` formatFile =
    let expected = Ok [ FormatLine ("name", 10, JsonDataType.JString) ]
    
    parseFormatFile formatFile =! expected
    
[<Xunit.Theory>]
[<Xunit.InlineData("\"column name\",width,datatype\nname,10,TEXT\nvalid,1,BOOLEAN\n")>]
[<Xunit.InlineData("width,\"column name\",datatype\n10,name,TEXT\n1,valid,BOOLEAN\n")>]
let ``parseFormatFile returns expected FormatLines when there is two lines`` formatFile =
    let expected = Ok [ FormatLine ("name", 10, JsonDataType.JString); FormatLine ("valid", 1, JsonDataType.JBool) ]
    
    parseFormatFile formatFile =! expected
    
[<Xunit.Fact>]
let ``Given a format and a dataFileLine of Diabetes, parseDataFileLine returns an expected map`` () =
    let dataFileLine = "Diabetes  1 1\n"
    let formatLines = [
        FormatLine ("name", 10, JsonDataType.JString)
        FormatLine ("valid", 1, JsonDataType.JBool)
        FormatLine("count", 3, JsonDataType.JInt)
    ]
    let expected : Result<JsonObject, Error> = Ok <| Map.ofList [ ("name", "Diabetes"); ("valid", true); ("count", 1) ]
    
    parseDataFileLine formatLines dataFileLine =! expected
    
[<Xunit.Fact>]
let ``Given a format and a dataFileLine of Asthma, parseDataFileLine returns an expected map`` () =
    let dataFileLine = "Asthma    0-14\n"
    let formatLines = [
        FormatLine ("name", 10, JsonDataType.JString)
        FormatLine ("valid", 1, JsonDataType.JBool)
        FormatLine("count", 3, JsonDataType.JInt)
    ]
    let expected : Result<JsonObject, Error> = Ok <| Map.ofList [ ("name", "Asthma"); ("valid", false); ("count", -14) ]
    
    parseDataFileLine formatLines dataFileLine =! expected
    
[<Xunit.Fact>]
let ``Given a format and a dataFileLine of Stroke, parseDataFileLine returns an expected map`` () =
    let dataFileLine = "Stroke    1122\n"
    let formatLines = [
        FormatLine ("name", 10, JsonDataType.JString)
        FormatLine ("valid", 1, JsonDataType.JBool)
        FormatLine("count", 3, JsonDataType.JInt)
    ]

    let expected : Result<JsonObject, Error> = Ok <| Map.ofList [ ("name", "Stroke"); ("valid", true); ("count", 122) ]
    
    parseDataFileLine formatLines dataFileLine =! expected

[<Xunit.Fact>]
let ``parseFormatFile should return error when file is an empty string`` () =
    let file = ""
    
    parseFormatFile file =! Error (UnparsableFormatFile file)