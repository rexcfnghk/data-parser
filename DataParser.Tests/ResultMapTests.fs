module DataParser.Tests.ResultMapTests

open Hedgehog.Xunit
open Swensen.Unquote
open ResultMap

[<Property>]
let ``map obeys functor identity`` (x: int) (y: string) =
    let sut = ResultMap <| Map.ofList [ (x, Ok y) ]
    
    ResultMap.map id sut =! sut

[<Property>]
let ``map obeys functor composition`` (x: string) (y: int) (z: int) (a: int) =
    let sut = ResultMap <| Map.ofList [ (x, Ok y) ]
    let f, g = ((+) z, (*) a)
    
    ResultMap.map (g << f) sut =! (ResultMap.map f sut |> ResultMap.map g) 
