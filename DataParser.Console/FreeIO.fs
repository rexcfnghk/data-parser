namespace DataParser.Console

open ResultMap
open DataParser.Console.DataFiles
open DataParser.Console.Core
open DataParser.Console.FormatFiles

/// IO algebra for the application (free monad instructions)
type IO<'next> =
    | ReadSpecs of folder:string * (ResultMap<FormatName, FormatLine list, Error> -> 'next)
    | ParseDataFile of dataFile:DataFileFormat * (Result<DataFileParseResult, Error list> -> 'next)
    | WriteOutput of folder:string * DataFileParseResult * (unit -> 'next)
    | LogError of string * (unit -> 'next)
    | LogInfo of string * (unit -> 'next)

/// Free monad over IO
type FreeIO<'a> =
    | Pure of 'a
    | Free of IO<FreeIO<'a>>

module FreeIOOps =
    let rec bind (f:'a -> FreeIO<'b>) (m:FreeIO<'a>) : FreeIO<'b> =
        match m with
        | Pure x -> f x
        | Free op ->
            match op with
            | ReadSpecs(path, next) -> Free(ReadSpecs(path, next >> bind f))
            | ParseDataFile(df, next) -> Free(ParseDataFile(df, next >> bind f))
            | WriteOutput(folder, r, next) -> Free(WriteOutput(folder, r, next >> bind f))
            | LogError(msg, next) -> Free(LogError(msg, next >> bind f))
            | LogInfo(msg, next) -> Free(LogInfo(msg, next >> bind f))

    let map f m = bind (f >> Pure) m

    let liftF (op: IO<'a>) : FreeIO<'a> =
        match op with
        | ReadSpecs(p, k) -> Free(ReadSpecs(p, k >> Pure))
        | ParseDataFile(d, k) -> Free(ParseDataFile(d, k >> Pure))
        | WriteOutput(fol, r, k) -> Free(WriteOutput(fol, r, k >> Pure))
        | LogError(msg, k) -> Free(LogError(msg, k >> Pure))
        | LogInfo(msg, k) -> Free(LogInfo(msg, k >> Pure))

    // smart constructors
    let readSpecs folder = Free(ReadSpecs(folder, Pure))
    let parseDataFile df = Free(ParseDataFile(df, Pure))
    let writeOutput folder result = Free(WriteOutput(folder, result, Pure))
    let logError msg = Free(LogError(msg, Pure))
    let logInfo msg = Free(LogInfo(msg, Pure))

    type IOBuilder() =
        member _.Bind(m, f) = bind f m
        member _.Return(x) = Pure x
        member _.ReturnFrom(x:FreeIO<'a>) = x
        member _.Zero() = Pure ()
        member _.Delay(f: unit -> FreeIO<'a>) = f()
        member _.For(seq: seq<'a>, body: 'a -> FreeIO<unit>) : FreeIO<unit> =
            // fold left to sequence effects in order
            Seq.fold (fun acc v -> bind (fun _ -> body v) acc) (Pure ()) seq
        member _.Combine(comp, cont) =
            // sequence two computations: run comp then cont
            bind (fun _ -> cont) comp

    let io = IOBuilder()
