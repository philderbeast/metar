open System
open MeridianArc.Metar
open Parse
open FParsec
open FParsec.Primitives
open FParsec.CharParsers
open _Airports

//Console.WriteLine("Date line, then metar line ...")
//let line1 = datePart(nzch)
//let line2 = metarPart(nzch)
//Console.WriteLine(line1)
//Console.WriteLine(line2)

//Console.WriteLine("Parse date ...")
//Console.WriteLine(parseDate(line1))

//test date @"2012/05/21"
//test time @"05:00"
//
//test dateTime @"2012/05/21 05:00"
//test dateTime @"2012/05/21 05:00
//"

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


Console.ReadKey() |> ignore