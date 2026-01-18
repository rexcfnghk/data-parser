module DataParser.Tests.ProgramTests

open System
open System.IO
open Swensen.Unquote
open DataParser.Console
open ResultMap

open Xunit

[<Fact>]
let ``program should request specs, parse data and request write`` () =
    // arrange: create a temp workspace with specs and data
    let baseDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
    Directory.CreateDirectory(baseDir) |> ignore
    let specsDir = Path.Combine(baseDir, "specs")
    let dataDir = Path.Combine(baseDir, "data")
    let outDir = Path.Combine(baseDir, "output")
    Directory.CreateDirectory(specsDir) |> ignore
    Directory.CreateDirectory(dataDir) |> ignore
    Directory.CreateDirectory(outDir) |> ignore

    // write a simple spec file
    let specContent = "\"column name\",width,datatype\nname,10,TEXT\n"
    File.WriteAllText(Path.Combine(specsDir, "person.csv"), specContent)

    // write a simple data file with a name of 10 chars
    let dataLine = "ABCDEFGHIJ\n"
    File.WriteAllText(Path.Combine(dataDir, "person_2020-01-01.txt"), dataLine)

    // handlers that the pure interpreter will use
    let readHandler (folder: string) : ResultMap<FormatName, FormatLine list, Error> =
        // read all csv files and parse them using existing parser
        let items =
            Directory.GetFiles(folder, "*.csv")
            |> Array.map (fun fp ->
                let formatName = FormatName (Path.GetFileNameWithoutExtension fp)
                let text = File.ReadAllText(fp)
                let parsed = DataParser.Console.FormatFiles.parseFormatFile text
                formatName, parsed)
        ResultMap << Map.ofArray <| items

    let parseHandler (df: DataFileFormat) : Result<DataFileParseResult, Error list> =
        // reuse the real parse path by calling the task-based parser synchronously
        DataParser.Console.FileRead.parseDataFile df |> fun t -> t.GetAwaiter().GetResult()

    // act: build the program with our temp dirs and run the pure interpreter
    let program = Program.makeProgram specsDir dataDir outDir
    let ((), actions) = PureIoInterpreter.interpretPure readHandler parseHandler program

    // assert: actions include ReadSpecs, ParseDataFile, WriteOutput and final LogInfo
    let hasReadSpecs = actions |> List.exists (function PureIoInterpreter.ReadSpecs _ -> true | _ -> false)
    let hasParse = actions |> List.exists (function PureIoInterpreter.ParseDataFile _ -> true | _ -> false)
    let hasWrite = actions |> List.exists (function PureIoInterpreter.WriteOutput(_, _) -> true | _ -> false)
    let hasFinalLog = actions |> List.exists (function PureIoInterpreter.LogInfo m -> m = "Processing complete." | _ -> false)

    test <@ hasReadSpecs = true && hasParse = true && hasWrite = true && hasFinalLog = true @>
