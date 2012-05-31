namespace BlockScope

module Parser =
    open FParsec
    open FParsec.Primitives

    type UserState = unit
    type Parser<'t> = Parser<'t, UserState>
    let str s : Parser<_> = pstring s
    let stringFromList l = new string(l |> List.toArray) 
    let quote = str "\""
    let quoted p = between quote quote p
    let quotedListOfChar = quoted (many1 (noneOf "\""))
    let quotedStr = quotedListOfChar |>> stringFromList