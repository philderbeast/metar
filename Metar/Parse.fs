module Parse

open FParsec
open FParsec.Primitives
open FParsec.CharParsers

// 4.2 Parsing a single float
// The definition of pfloat.
// val pfloat: Parser<float,'u>
let test p str =
    match run p str with
    | Success(result, _, _)   -> printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg

// 4.10 F#’s value restriction
// Put here so that the definitions can be used in earlier sections with the error.
//    error FS0030: Value restriction.
//    The value 'p' has been inferred to have generic type
//        val p : Parser<string,'_a>
//    Either make the arguments to 'p' explicit or,
//    if you do not intend for it to be generic, add a type annotation.
type UserState = unit
type Parser<'t> = Parser<'t, UserState>

// 4.3 Parsing a float between brackets
let str s : Parser<_> = pstring s
let floatList = str "[" >>. sepBy pfloat (str ",") .>> str "]"
// If not using the pipe forward operator. See section 4.4 for floatBetweenBrackets.
// let floatBetweenBrackets = str "[" >>. pfloat .>> str "]"

// 4.4 Abstracting parsers
let betweenStrings s1 s2 p = str s1 >>. p .>> str s2

let floatBetweenBrackets = pfloat |> betweenStrings "[" "]"
let floatBetweenDoubleBrackets = pfloat |> betweenStrings "[[" "]]"

// The between combinator is a builtin but this is its declaration.
// let between pBegin pEnd p  = pBegin >>. p .>> pEnd

// Now betweenStrings can also be implemented using the pipe forward operator
// let betweenStrings s1 s2 p = p |> between (str s1) (str s2)

// 4.5 Parsing a list of floats
// This is the builtin many combinator.
// val many: Parser<'a,'u> -> Parser<'a list,'u>

// 4.6 Handling whitespace
let ws = spaces
let str_ws s = pstring s .>> ws
let float_ws : Parser<_> = pfloat .>> ws
let numberList = str_ws "[" >>. sepBy float_ws (str_ws ",") .>> str_ws "]"
let numberListFile = ws >>. numberList .>> eof

// 4.7 Parsing string data
let identifier : Parser<_> =
    let isIdentifierFirstChar c = isLetter c || c = '_'
    let isIdentifierChar c = isLetter c || isDigit c || c = '_'

    many1Satisfy2L isIdentifierFirstChar isIdentifierChar "identifier"
    .>> ws // skips trailing whitepace

let stringLiteral : Parser<_> =
    let normalChar = satisfy (fun c -> c <> '\\' && c <> '"')
    let unescape c = match c with
                     | 'n' -> '\n'
                     | 'r' -> '\r'
                     | 't' -> '\t'
                     | c   -> c
    let escapedChar = pstring "\\" >>. (anyOf "\\nrt\"" |>> unescape)
    between (pstring "\"") (pstring "\"")
            (manyChars (normalChar <|> escapedChar))

// 4.8 Sequentially applying parsers
let product = pipe2 float_ws (str_ws "*" >>. float_ws)
                    (fun x y -> x * y)

type StringConstant = StringConstant of string * string

let stringConstant = pipe3 identifier (str_ws "=") stringLiteral
                           (fun id _ str -> StringConstant(id, str))

// 4.9 Parsing alternatives
let boolean : Parser<_> =
    (stringReturn "true"  true) <|> (stringReturn "false" false)