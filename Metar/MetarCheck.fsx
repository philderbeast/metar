#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsecCS.dll"
#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsec.dll"
#r @"bin\Debug\AirportCodes.dll"
#r @"C:\dev\metar\metar\packages\FsCheck.0.7.1\lib\net35\FsCheck.dll"

#load "Parser.fs"
#load "Airport.fs"
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
open BlockScope.Metar.Parser

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
out (toStrDateTime dateTime) dateAKL;;

out (toStrCode icao) "SGES";;
out (toStrCode icao) metarCHC;;

out (toStrObsTime obsTime) "210500Z";;
out (toStrObsTime (icao >>. ws >>. obsTime)) metarCHC;;

let variable:Parser<_> =
    pipe3 (regex @"\d{3,3}") (pstring "V") (regex @"\d{3,3}")
        (fun x _ y -> Variable(int x, int y))

out (toStrWindDirection variable) "310V290";; 

let steady:Parser<_> =
    regex @"\d{3,3}" |>> (fun s -> Bearing(int s))

out (toStrWindDirection steady) "060";; 

out (toStrWindDirection ((attempt variable) <|> steady)) "060";; 
out (toStrWindDirection ((attempt variable) <|> steady)) "310V290";; 

let kts:Parser<_> = regex @"\d+" .>> pstring "KT" |>> (fun x -> InKnots(int x))
let mps:Parser<_> = regex @"\d+" .>> pstring "MPS" |>> (fun x -> InMPS(int x))
out (toStrWindSpeed kts) "08KT";;
out (toStrWindSpeed mps) "03MPS";;
out (toStrWindSpeed ((attempt kts) <|> mps)) "08KT";;
out (toStrWindSpeed ((attempt kts) <|> mps)) "03MPS";;

out (toStrWind pWind) "310V29003MPS";;
out (toStrWind pWind) "00000KT";;
out (toStrWind pWind) "06008KT";;

out (toStrInt vis) "9999";;
