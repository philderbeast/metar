#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsecCS.dll"
#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsec.dll"
#r @"bin\Debug\AirportCodes.dll"

#load "Airport.fs"
#load "Parser.fs"
#load "OpenFlights.fs"

open System
open FParsec
open FParsec.Primitives
open FParsec.CharParsers
open BlockScope.Parser
open BlockScope.PrimitiveParsers
open BlockScope.OpenFlights

let test p str =
    match run p str with
    | Success(result, _, _)   -> printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg

test quote "\""
test quotedListOfChar "\"GKA\""
test quotedStr "\"GKA\""

test (sepBy quotedStr (str ",")) "\"Goroka\",\"Goroka\",\"Papua New Guinea\""
test listOfStrings "\"Goroka\",\"Goroka\",\"Papua New Guinea\""

test iata "\"GKA\""
test icao "\"AYGA\""
test codes "\"GKA\",\"AYGA\""

test ((str ",") >>. codes) ",\"GKA\",\"AYGA\""
test thenCodes ",\"GKA\",\"AYGA\""

test countries "\"Goroka\",\"Goroka\",\"Papua New Guinea\""
test (countries .>>. thenCodes) "\"Goroka\",\"Goroka\",\"Papua New Guinea\",\"GKA\",\"AYGA\""

test latlong "-6.081689,145.391881"

test line "1,\"Goroka\",\"Goroka\",\"Papua New Guinea\",\"GKA\",\"AYGA\",-6.081689,145.391881,5282,10,\"U\""
test lines "1,\"Goroka\",\"Goroka\",\"Papua New Guinea\",\"GKA\",\"AYGA\",-6.081689,145.391881,5282,10,\"U\"
2,\"Madang\",\"Madang\",\"Papua New Guinea\",\"MAG\",\"AYMD\",-5.207083,145.7887,20,10,\"U\"
3,\"Mount Hagen\",\"Mount Hagen\",\"Papua New Guinea\",\"HGU\",\"AYMH\",-5.826789,144.295861,5388,10,\"U\""
