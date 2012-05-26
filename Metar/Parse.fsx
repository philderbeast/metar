#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsecCS.dll"
#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsec.dll"
#r @"bin\Debug\AirportCodes.dll"
#load "Parse.fs"
#load "Airports.fs"
#load "Parse.fs"

open System
open Parse
open FParsec
open FParsec.Primitives
open FParsec.CharParsers
open _Airports

test _Airports.quote "\""
test _Airports.quotedListOfChar "\"GKA\""
test _Airports.quotedStr "\"GKA\""

test (sepBy _Airports.quotedStr (str ",")) "\"Goroka\",\"Goroka\",\"Papua New Guinea\""
test _Airports.listOfStrings "\"Goroka\",\"Goroka\",\"Papua New Guinea\""

test _Airports.iata "\"GKA\""
test _Airports.icao "\"AYGA\""
test _Airports.codes "\"GKA\",\"AYGA\""

test ((str ",") >>. _Airports.codes) ",\"GKA\",\"AYGA\""
test _Airports.thenCodes ",\"GKA\",\"AYGA\""

test _Airports.countries "\"Goroka\",\"Goroka\",\"Papua New Guinea\""
test (_Airports.countries .>>. thenCodes) "\"Goroka\",\"Goroka\",\"Papua New Guinea\",\"GKA\",\"AYGA\""

test _Airports.latlong "-6.081689,145.391881"

test line "1,\"Goroka\",\"Goroka\",\"Papua New Guinea\",\"GKA\",\"AYGA\",-6.081689,145.391881,5282,10,\"U\""
test lines "1,\"Goroka\",\"Goroka\",\"Papua New Guinea\",\"GKA\",\"AYGA\",-6.081689,145.391881,5282,10,\"U\"
2,\"Madang\",\"Madang\",\"Papua New Guinea\",\"MAG\",\"AYMD\",-5.207083,145.7887,20,10,\"U\"
3,\"Mount Hagen\",\"Mount Hagen\",\"Papua New Guinea\",\"HGU\",\"AYMH\",-5.826789,144.295861,5388,10,\"U\""
