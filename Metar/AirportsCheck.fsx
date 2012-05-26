#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsecCS.dll"
#r @"C:\dev\metar\metar\packages\FParsec.0.9.2.0\lib\net40\FParsec.dll"
#r @"bin\Debug\AirportCodes.dll"
#r @"C:\dev\metar\metar\packages\FsCheck.0.7.1\lib\net35\FsCheck.dll"
#load "Parse.fs"
#load "Airports.fs"

open System
open FParsec
open FParsec.Primitives
open FParsec.CharParsers
open FsCheck
open Prop
open FsCheck.Checks
open FsCheck.Fluent
open _Airports
open System.Text.RegularExpressions

let out p str =
    match run p str with
    | Success(result, _, _)   -> result
    | Failure(errorMsg, _, _) -> errorMsg

let rec illegal (chars:char list) =
    match chars with
    | [] -> false
    | a::b -> a = '"' || a = '\r' || a = '\n' || illegal b

let safeToQuote (s:string) =
    match s with
        | null | "" -> false
        | _ ->
            let splits = s.Split([|'"';'\r';'\n'|])
            match splits.Length with
                | 1 -> 
//                    printf "PASS: %A\r\n" s
                    true
                | _ ->
//                    printf "FAIL: %A\r\n" s
                    false

[for x in "
" -> x];; 
[for x in "" -> x] |> illegal
safeToQuote "";;
safeToQuote "\r";;
safeToQuote "\r\n";;
safeToQuote "
";;
safeToQuote "
";;
safeToQuote "abc";;

let enQuote s = "\"" + s + "\""
let deQuote (s:string) = s.Trim('"')
let prop_QuotedStr (s : string) =
    let filter = safeToQuote(s)
    let property = out quotedStr (enQuote s) = s
    filter ==> property
Check.Quick prop_QuotedStr

let toStr p = p |>> (fun x -> match x with | ICAO(s) | IATA(s) -> s) 

out (toStr icao) (enQuote "SGES");;
out (toStr iata) (enQuote "SGE");;

// Generate 4 letter strings of uppercaps that are double quoted.
let genUpper length =
    (Gen.listOfLength(length) (Gen.elements ['A'..'Z']))
    |> Gen.map (fun (chars:char list) -> enQuote (new String(List.toArray chars)))

genUpper 4 |> Helpers.sample1;;

let arbUpper count = genUpper count |> Arb.fromGen
let prop_Icao (s:string) = Prop.forAll (arbUpper 4) (fun x -> out (toStr icao) x = deQuote x) 
let prop_Iata (s:string) = Prop.forAll (arbUpper 3) (fun x -> out (toStr iata) x = deQuote x) 

Check.Quick prop_Icao
Check.Quick prop_Iata

Arb.Default.Float().Generator |> Gen.suchThat (fun x -> -90 <= x && x <= 90)

let genLatitude = Gen.choose(-90, 90) |>