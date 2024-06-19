module DataParser.Tests.ResultMapTests

open Hedgehog.Xunit
open Swensen.Unquote
open ResultMap

[<Property>]
let ``map obeys functor identity`` (x: int) (y: string) =
    let sut = ResultMap <| Map.ofList [ (x, Ok y) ]
    
    ResultMap.map id sut =! sut

[<Property>]
let ``map obeys functor composition`` (x: string) (y: int) =
    let sut = ResultMap <| Map.ofList [ (x, Ok y) ]
    let f, g = ((+) 1, (*) 2)
    
    ResultMap.map (g << f) sut =! (ResultMap.map f sut |> ResultMap.map g) 
