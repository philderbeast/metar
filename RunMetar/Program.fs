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

Console.ReadKey() |> ignore