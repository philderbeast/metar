#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsecCS.dll"
#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsec.dll"
#r @"bin\Debug\AirportCodes.dll"
#r @"C:\dev\metar\metar\packages\FsCheck.0.7.1\lib\net35\FsCheck.dll"

#load "Parser.fs"
#load "Metar.fs"

open System
open FParsec
open FParsec.Primitives
open FParsec.CharParsers
open FsCheck
open Prop
open FsCheck.Checks
open FsCheck.Fluent
open System.Text.RegularExpressions
open BlockScope.Metar
open BlockScope.Parser

let nzaa = "2012/05/21 06:00
NZAA 210600Z 00000KT 9999 FEW020 14/11 Q1017 TEMPO 8000 SHRA
"

let nzch = "2012/05/21 05:00
NZCH 210500Z 06008KT 9999 SCT013 10/08 Q1020 NOSIG
"

let out p str =
    match run p str with
    | Success(result, _, _)   -> result
    | Failure(errorMsg, _, _) -> errorMsg

let dateAKL = datePart nzaa;;
let metarCHC = metarPart nzch;;

out date dateAKL;;
out time (dateAKL.Split() |> Seq.last);;
out (toStr dateTime) dateAKL;;

//out (toStr icao) (enQuote "SGES");;