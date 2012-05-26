module metar

open FParsec
open FParsec.Primitives
open FParsec.CharParsers
open Parse

let nzaa = "2012/05/21 06:00
NZAA 210600Z 00000KT 9999 FEW020 14/11 Q1017 TEMPO 8000 SHRA
"

let nzch = "2012/05/21 05:00
NZCH 210500Z 06008KT 9999 SCT013 10/08 Q1020 NOSIG
"

let datePart metar = (metar : string).Split [|'\n'|] |> Seq.head
let metarPart metar = (metar : string).Split [|'\n'|] |> Seq.skip 1 |> Seq.head

let date : Parser<_> =
    regex @"\d{4,4}\/\d{2,2}\/\d{2,2}"

let time : Parser<_> =
    regex @"\d{2,2}:\d{2,2}"

type DateTime = DateTime of string * string


let dateTime =
    pipe3 date ws time (fun d _ t -> DateTime(d, t))