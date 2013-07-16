namespace BlockScope.Metar

module Parser =
    open FParsec
    open FParsec.Primitives
    open BlockScope.Parser
    open BlockScope.Metar

    let datePart (metar: string) = metar.Split [|'\n'|] |> Seq.head
    let metarPart (metar: string) = metar.Split [|'\n'|] |> Seq.skip 1 |> Seq.head

    let ws = spaces
    let date : Parser<_> =
        regex @"\d{4,4}\/\d{2,2}\/\d{2,2}"
        <?> "Date in yyyy/mm/dd format"

    let time : Parser<_> =
        regex @"\d{2,2}:\d{2,2}"
        <?> "Time in hh:mm format"

    type DateTime = DateTime of string * string
    let toStrDateTime p = p |>> (fun dt -> match dt with | DateTime(d, t) -> d + " " + t) 

    let dateTime =
        pipe3 date ws time (fun d _ t -> DateTime(d, t))

    let icao : Parser<_> = (regex "[A-Z]{4,4}") |>> (fun s -> ICAO(s)) <?> "ICAO 4 letter code in uppercaps"
    let toStrCode p = p |>> (fun code -> match code with | ICAO(s) | IATA(s) -> s)

    type DayAndHour = { Day:int; Hour:int } 
    let obsTime : Parser<_> =
        let dd = regex @"\d{2,2}"
        let hhhhZ = regex @"\d{4,4}Z"
        pipe2 dd hhhhZ (fun d t -> { Day = (int d); Hour = int (t.TrimEnd('Z')) })
        <?> "Time in ddhhhhZ format where dd is the day of the month"

    let toStrObsTime p = p |>> (fun dh -> match dh with | { Day = d; Hour = h } -> sprintf "%d %04dZ" d h)

    type WindDirection =
        | Bearing of int
        | Variable of int * int 
        with
        override this.ToString() =
            match this with
                | Bearing(from) -> sprintf "%3d" from
                | Variable(a, b) -> sprintf "%3dV%3d" a b

    let toStrWindDirection p = p |>> (fun (wd:WindDirection) -> wd.ToString())

    type WindSpeed =
        | InKnots of int
        | InMPS of int
        with
        override this.ToString() =
            match this with
                | InKnots(s) -> sprintf "%03dKT" s
                | InMPS(s) -> sprintf "%03dMPS" s

    let toStrWindSpeed p = p |>> (fun (ws:WindSpeed) -> ws.ToString())

    type Wind = { Direction:WindDirection; Speed:WindSpeed }
        with
        override this.ToString() =
            sprintf "%A" this

    let toStrWind p = p |>> (fun w -> w.ToString())

    let pWind : Parser<_> =
        let variable:Parser<_> =
            pipe3 (regex @"\d{3,3}") (pstring "V") (regex @"\d{3,3}")
                (fun x _ y -> Variable(int x, int y))
        let steady:Parser<_> =
            regex @"\d{3,3}" |>> (fun s -> Bearing(int s))
        let dir = (attempt variable) <|> steady
        let kts:Parser<_> = regex @"\d+" .>> pstring "KT" |>> (fun x -> InKnots(int x))
        let mps:Parser<_> = regex @"\d+" .>> pstring "MPS" |>> (fun x -> InMPS(int x))
        let speed = (attempt kts) <|> mps
        pipe2 dir speed (fun d s -> { Direction = d; Speed = s })
        <?> "Wind is direction then speed. Direction is ddd or dddVddd degrees and speed is in KTS or MPS"

    let vis:Parser<_> =
        regex @"\d{4,4}" |>> (fun v -> int v)

    let toStrInt p = p |>> (fun s -> sprintf "%4d" s)
