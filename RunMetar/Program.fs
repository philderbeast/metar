open System
open Parse
open FParsec
open FParsec.Primitives
open FParsec.CharParsers
open MeridianArc.Airports

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

let line = "1,\"Goroka\",\"Goroka\",\"Papua New Guinea\",\"GKA\",\"AYGA\",-6.081689,145.391881,5282,10,\"U\""
let parsed = run OpenFlights.line line
Console.ReadKey() |> ignore