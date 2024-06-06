module DataParser.Tests

open System
open System.Globalization
open DataParser.Console.Core
open Hedgehog
open Hedgehog.Xunit
open Swensen.Unquote

[<Property>]
let ``DataFileNames has structural equality`` (ff: FileFormat) (s: string) =
    DataFileName (ff, s) =! DataFileName (ff, s)
    
[<Property>]
let ``Unfound format returns false when element does not exist in set`` (xs: Set<FileFormat>) (x: FileFormat) =
    // Arrange
    let sut = Set.remove x xs
    
    // Act Assert
    test <@ not <| Set.contains x sut @>
    
[<Property>]
let ``lookupFileFormat returns FormatNotFound error when element does not exist in set`` (xs: Set<FileFormat>) (x: FileFormat) =
    // Arrange
    let sut = Set.remove x xs
    let expected = Error <| FileFormatNotFound (sut, x)
    
    // Act Assert
    lookupFileFormat sut x =! expected
    
[<Property>]
let ``lookupFileFormat returns Ok when element exists in set`` (xs: Set<FileFormat>) (x: FileFormat) =
    // Arrange
    let sut = Set.add x xs
    let expected = Ok x
    
    // Act Assert
    lookupFileFormat sut x =! expected
    
[<Property>]
let ``FormatLines are compared with structural equality`` (columnName: string) (width: int) (dataType: JsonDataType) =
    FormatLine (columnName, width, dataType) =! FormatLine (columnName, width, dataType)
    
[<Property>]
let ``collectFileFormats should return a set with every file format in the input list`` (xs: FileFormat list) =
    let resultSet = Set.ofList xs
    
    test <@ Set.isSubset resultSet (collectFileFormats xs) @>
    
[<Property>]
let ``parseDataFileName should return error when file name does not conform to expected format`` (s: string) =
    let expected = Error <| DataFileNameFormatError s
    
    parseDataFileName s =! expected
    
[<Property>]
let ``parseDataFileName should return ok with expected fields when file conform to expected format`` (s: int) (d: DateTime) =
    let rawDate = d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
    let fileName = $"{s}_{rawDate}"
    let fileFormat = FileFormat $"{s}"
    let expected = Ok <| DataFileName (fileFormat, rawDate)
    
    parseDataFileName fileName =! expected