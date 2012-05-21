open System
open MeridianArc.Metar
open Parse
open FParsec
open FParsec.Primitives
open FParsec.CharParsers

//Console.WriteLine("Date line, then metar line ...")
//let line1 = datePart(nzch)
//let line2 = metarPart(nzch)
//Console.WriteLine(line1)
//Console.WriteLine(line2)

//Console.WriteLine("Parse date ...")
//Console.WriteLine(parseDate(line1))

// 4.2 Parsing a single float
test pfloat "1.25"
test pfloat "1.25E 3"
test numberList @"[ 1 ,
                          2 ] ";;

// 4.3 Parsing a float between brackets
test floatBetweenBrackets "[1.0]";;
test floatBetweenBrackets "[]";;
test floatBetweenBrackets "[1.0";;
test floatBetweenBrackets "[1.0, 2.0]";;

// 4.4 Abstracting parsers
// No tests for this section

// 4.5 Parsing a list of floats
test (many floatBetweenBrackets) "";;
test (many floatBetweenBrackets) "[1.0]";;
test (many floatBetweenBrackets) "[2][3][4]";;
test (many floatBetweenBrackets) "[1][2.0E]";;
test (many1 floatBetweenBrackets) "(1)";;
test (many1 (floatBetweenBrackets <?> "float between brackets")) "(1)";;

test floatList "[]";;
test floatList "[1.0]";;
test floatList "[4,5,6]";;
test floatList "[1.0,]";;
test floatList "[1.0,2.0";; 

// 4.6 Handling whitespace
test floatBetweenBrackets "[1.0, 2.0]";;
test numberList @"[ 1 ,
                          2 ] ";;
test numberList @"[ 1,
                         2; 3]";;
test numberListFile " [1, 2, 3] [4]";;

// 4.7 Parsing string data
test (many (str "a" <|> str "b")) "abba";;
test (skipStringCI "<float>" >>. pfloat) "<FLOAT>1.0";;
// Use module prefix to avoid calling the builtin identifier parser.
test Parse.identifier "_";;
test Parse.identifier "_test1=";;
test Parse.identifier "1";;
test stringLiteral "\"abc\"";;
test stringLiteral "\"abc\\\"def\\\\ghi";;
test stringLiteral "\"abc\\def\"";;

// 4.8 Sequentially applying parsers
test product "3 * 5";;
test stringConstant "myString = \"stringValue\"";;
test (float_ws .>>. (str_ws "," >>. float_ws)) "123, 456";;

// 4.9 Parsing alternatives
test boolean "false";;
test boolean "true";;
test boolean "tru";;
test ((ws >>. str "a") <|> (ws >>. str "b")) " b";;
test (ws >>. (str "a" <|> str "b")) " b";;

//test (str_ws) @"2012/05/21 05:00"

Console.ReadKey() |> ignore