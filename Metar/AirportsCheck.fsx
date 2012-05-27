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
let prop_QuotedStr (s:string) =
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

let arbUpper length = genUpper length |> Arb.fromGen
let prop_Icao (s:string) = Prop.forAll (arbUpper 4) (fun x -> out (toStr icao) x = deQuote x) 
let prop_Iata (s:string) = Prop.forAll (arbUpper 3) (fun x -> out (toStr iata) x = deQuote x) 

Check.Quick prop_Icao
Check.Quick prop_Iata

// pfloat is a bit limited in the formatting it will parse, %g seems a good fit.
let encodeTupleAsString coords = match coords with | (x, y) -> sprintf "%g,%g" x y
let printLatLong coords = match coords with | LatLong(x, y) -> encodeTupleAsString (x, y)
let tupleLatLong coords = match coords with | LatLong(x, y) -> (x, y)
let genLatLongAsString = Gen.two(Arb.Default.Float().Generator) |> Gen.map encodeTupleAsString
let genLatLongAsTuple = Gen.two(Arb.Default.Float().Generator)
    
genLatLongAsString |> Helpers.sample1;;
run latlong (genLatLongAsString |> Helpers.sample1);;

let outLatLongAsString parser coords =
    match run parser coords with
        | Success(results, _, _) -> Some(printLatLong results)
        | _ -> None
let prop_LatLongAsString (s:string) =
    let property coords = 
        outLatLongAsString (latlong) coords = Some(coords)
    Prop.forAll (Arb.fromGen genLatLongAsString) property

let outLatLongAsTuple parser coords =
    match run parser coords with
        | Success(results, _, _) -> (printfn "%A" (tupleLatLong results)); Some(tupleLatLong results)
        | _ -> None

let a = nan;;
let b = nan;;
a = b;;
match a with | x when x = nan -> true | _ -> false;
let specialFloats f1 f2 =
    match f1, f2 with
        | x, y when (Double.IsNaN x) && (Double.IsNaN y) -> true
        | x, y when x = infinity && y = infinity -> true
        | x, y when x = -infinity && y = -infinity -> true
        | x, y -> x = y
specialFloats nan nan;;
specialFloats -infinity -infinity;;
specialFloats infinity infinity;;
specialFloats 1.0 2.0;;
specialFloats 0.3 0.3;;

let prop_LatLongAsTuple =
    // SOURCE: "Testing Erlang Data Types with Quviq QuickCheck", Arts, Castro & Hughes.
    // I added the extra matches for nan, -inf & +inf.
    let equiv f1 f2 =
        let absError = 1.0e-16
        let relError = 1.0e-10
        match f1, f2 with
            | x, y when (Double.IsNaN x) && (Double.IsNaN y) -> true
            | x, y when x = infinity && y = infinity -> true
            | x, y when x = -infinity && y = -infinity -> true
            | _ ->
            match abs(f1 - f2) with
                | x when x < absError -> true
                | x when abs(f1) > abs(f2) -> abs((f1 - f2)/f1) < relError
                | x when abs(f1) < abs(f2) -> abs((f1 - f2)/f2) < relError
                | _ -> false
    let property coords =
        let lhs = outLatLongAsTuple (latlong) (encodeTupleAsString coords)
        let rhs = ((printfn "%A" coords); Some(coords))
        let compare (lhs:(float * float) option) (rhs:(float * float) option) =
            match lhs, rhs with
                | Some(x1, y1), Some(x2, y2) -> equiv x1 x2 && equiv y1  y2
                | _ -> true
        compare lhs rhs
    Prop.forAll (Arb.fromGen genLatLongAsTuple) property

outLatLongAsString latlong (genLatLongAsString |> Helpers.sample1);;
Check.Quick prop_LatLongAsString;;

outLatLongAsTuple latlong (encodeTupleAsString (genLatLongAsTuple |> Helpers.sample1));;
Check.Verbose prop_LatLongAsTuple;;
