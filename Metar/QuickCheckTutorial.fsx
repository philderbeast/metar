﻿#r @"C:\dev\metar\metar\packages\FsCheck.0.7.1\lib\net35\FsCheck.dll"

open System
open FsCheck
open FsCheck.Checks
open FsCheck.Fluent
open Prop

(*
Properties
 Properties are expressed as F# function definitions. Properties are universally quantified over their parameters, so
*)
let revRevIsOrig xs = List.rev(List.rev xs) = xs
(*
means that the equality holds for all lists xs.
Properties must not necessarily have monomorphic types.
'Polymorphic' properties, such as the one above will be tested by FsCheck as if the generic arguments are of type object; this means, that values of various simple types (bool, char, string,...) are generated. It may even be the case that one generated list contains more than one type, e.g. {['r', "1a", true]} would be a list that can be used to check the property above.
The generated values are based on the type however, so you may change this behavior simply by giving xs a different inferred or explicit type:
*)
let revRevIsOrigInt (xs:list<int>) = List.rev(List.rev xs) = xs
(*
is only checked with lists of int.
FsCheck can check properties of various forms - these forms are called testable, and are indicated in the API by a generic type called 'Testable. A 'Testable may be a function of any number of parameters that returns bool or unit. In the latter case, a test passes if it does not throw.

Conditional Properties

Properties may take the form
<condition> ==> <property>
For example, 
*)
let rec ordered xs = 
    match xs with
    | [] -> true
    | [x] -> true
    | x::y::ys ->  (x <= y) && ordered (y::ys)
let rec insert x xs = 
    match xs with
    | [] -> [x]
    | c::cs -> if x <= c then x::xs else c::(insert x cs)
 
let Insert (x:int) xs = ordered xs ==> ordered (insert x xs)
(*
Such a property holds if the property after ==> holds whenever the condition does.
Testing discards test cases which do not satisfy the condition. Test case generation continues until 100 cases which do satisfy the condition have been found, or until an overall limit on the number of test cases is reached (to avoid looping if the condition never holds). In this case a message such as

Arguments exhausted after 97 tests.

indicates that 97 test cases satisfying the condition were found, and that the property held in those 97 cases. 
Notice that in this case the generated values had to be restricted to int. This is because the generated values need to be comparable, but this is not reflected in the types. Therefore, without the explicit restriction, FsCheck could generate lists containing different types (subtypes of objects), and these are not mutually comparable.

Lazy Properties

Since F# has eager evaluation by default, the above property does more work than necessary: it evaluates the property at the right of the condition no matter what the outcome of the condition on the left. While only a performance consideration in the above example, this may limit the expressiveness of properties - consider:
*)
let Eager a = a <> 0 ==> (1/a = 1/a)
(*
> Check.Quick Eager;;
Falsifiable, after 5 tests (0 shrinks) (StdGen (1811663113,295281725)):
0
with exception:
System.DivideByZeroException: Attempted to divide by zero.
   at Properties.Eager(Int32 a) in C:\Users\Kurt\Projects\FsCheck\main\FsCheck.Documentation\Properties.fs:line 24
   at DocumentationGen.fsCheckDocGen@127-3.Invoke(Int32 a) in C:\Users\Kurt\Projects\FsCheck\main\FsCheck.Documentation\Program.fs:line 127
   at FsCheck.Testable.evaluate[a,b](FSharpFunc`2 body, a a) in C:\Users\Kurt\Projects\FsCheck\main\FsCheck\Property.fs:line 167

Lazy evaluation is needed here to make sure the propery is checked correctly:
*)
let Lazy a = a <> 0 ==> (lazy (1/a = 1/a))

(*
> Check.Quick Lazy;;
Ok, passed 100 tests.


Quantified Properties

Properties may take the form

forAll <arbitrary>  (fun <args> -> <property>)

For example,
*)
let orderedList = Arb.from<list<int>> |> Arb.mapFilter List.sort ordered
let InsertWithArb x = forAll orderedList (fun xs -> ordered(insert x xs))
(*
The first argument of forAll is an IArbitrary instance. Such an instance encapsulates a test data generator and a shrinker (more on the latter later). By supplying a custom generator, instead of using the default generator for that type, it is possible to control the distribution of test data. In the example, by supplying a custom generator for ordered lists, rather than filtering out test cases which are not ordered, we guarantee that 100 test cases can be generated without reaching the overall limit on test cases. Combinators for defining generators are described later.

Expecting exceptions

You may want to test that a function or method throws an exception under certain circumstances. The following combinator helps:

throws<'e :> exn,'a> Lazy<'a>

An example:
*)
let ExpectDivideByZero() = throws<DivideByZeroException,_> (lazy (raise <| DivideByZeroException()))

(*
> Check.Quick ExpectDivideByZero;;
Ok, passed 100 tests.


Timed Properties

Properties may take the form

within <timeout in ms> <Lazy<property>>

For example,
*)
let timesOut (a:int) = 
    lazy
        if a>10 then
            while true do System.Threading.Thread.Sleep(1000)
            true
        else 
            true
    |> within 2000

(*
> Check.Quick timesOut;;
Timeout of 2000 milliseconds exceeded, after 25 tests (0 shrinks) (StdGen (1825243113,295281725)):
15

The first argument is the maximal time the lazy property given may run. If it runs longer, FsCheck considers the test as failed. Otherwise, the outcome of the lazy property is the outcome of within. Note that, although within attempts to cancel the thread in which the property is executed, that may not succeed, and so the thread may actually continue to run until the process ends.

Observing Test Case Distribution

It is important to be aware of the distribution of test cases: if test data is not well distributed then conclusions drawn from the test results may be invalid. In particular, the ==> operator can skew the distribution of test data badly, since only test data which satisfies the given condition is used.
FsCheck provides several ways to observe the distribution of test data. Code for making observations is incorporated into the statement of properties, each time the property is actually tested the observation is made, and the collected observations are then summarized when testing is complete.

Counting Trivial Cases

A property may take the form

trivial <condition> <property>

For example,
*)
let insertTrivial (x:int) xs = 
    ordered xs ==> (ordered (insert x xs))
    |> trivial (List.length xs = 0)
(*
Test cases for which the condition is true are classified as trivial, and the proportion of trivial test cases in the total is reported. In this example, testing produces

> Check.Quick insertTrivial;;
Arguments exhausted after 71 tests (38% trivial).


Classifying Test Cases

A property may take the form
classify <condition> <string> <property>
For example,
*)
let insertClassify (x:int) xs = 
    ordered xs ==> (ordered (insert x xs))
    |> classify (ordered (x::xs)) "at-head"
    |> classify (ordered (xs @ [x])) "at-tail" 
(*
Test cases satisfying the condition are assigned the classification given, and the distribution of classifications is reported after testing. In this case the result is

> Check.Quick insertClassify;;
Arguments exhausted after 72 tests.
45% at-tail, at-head.
23% at-tail.
22% at-head.

Note that a test case may fall into more than one classification.

Collecting Data Values

A property may take the form

collect <expression> <property>

For example,
*)
let insertCollect (x:int) xs = 
    ordered xs ==> (ordered (insert x xs))
        |> collect (List.length xs)
(*
The argument of collect is evaluated in each test case, and the distribution of values is reported. The type of this argument is printed using sprintf "%A". In the example above, the output is

> Check.Quick insertCollect;;
Arguments exhausted after 62 tests.
45% 1.
41% 0.
9% 2.
1% 4.
1% 3.


Combining Observations

The observations described here may be combined in any way. All the observations of each test case are combined, and the distribution of these combinations is reported. For example, testing the property
*)
let insertCombined (x:int) xs = 
    ordered xs ==> (ordered (insert x xs))
        |> classify (ordered (x::xs)) "at-head"
        |> classify (ordered (xs @ [x])) "at-tail"
        |> collect (List.length xs)
(*
produces

> Check.Quick insertCombined;;
Arguments exhausted after 61 tests.
27% 0, at-tail, at-head.
22% 1, at-tail.
18% 1, at-head.
11% 1, at-tail, at-head.
8% 2.
3% 3, at-tail.
3% 2, at-tail.
3% 2, at-head.
1% 3, at-head.


And, Or and Labelling Subproperties

Properties may take the form

<property> .&. <property>


<property> .|. <property>

p1.&. p2 succeeds if both succeed, fails if one of the properties fails, and is rejected when both are rejected.
p1 .|. p2 succeeds if either property succeeds, fails if both properties fail, and is rejected when both are rejected.
The .&. combinator is most commonly used to write complex properties which share a generator. In that case, it might be difficult upon failure to know excatly which sub-property has caused the failure. That's why you can label sub-properties, and FsCheck shows the labels of the failed subproperties when it finds a counter-example. This takes the form:

<string> @| <property>


<property> |@ <string>

For example,
*)
let complex (m: int) (n: int) =
    let res = n + m
    (res >= m)    |@ "result > #1" .&.
    (res >= n)    |@ "result > #2" .&.
    (res < m + n) |@ "result not sum"
(*
produces:

> Check.Quick complex;;
Falsifiable, after 1 test (2 shrinks) (StdGen (1870773113,295281725)):
Label of failing property: result not sum
0
0

It's perfectly fine to apply more than one label to a property; FsCheck displays all the applicable labels. This is useful for displaying intermediate results, for example:
*)
let multiply (n: int, m: int) =
  let res = n*m
  sprintf "evidence = %i" res @| (
    "div1" @| (m <> 0 ==> lazy (res / m = n)),
    "div2" @| (n <> 0 ==> lazy (res / n = m)),
    "lt1"  @| (res > m),
    "lt2"  @| (res > n))

(*
> Check.Quick multiply;;
Falsifiable, after 1 test (2 shrinks) (StdGen (1871183113,295281725)):
Labels of failing property: evidence = 0, lt1
(0, 0)

Notice that the above property combines subproperties by tupling them. This works for tuples up to length 6. It also works for lists. In general form

(<property1>,<property2>,...,<property6>) means <property1> .&. <property2> .&.... .&.<property6>


[property1;property2,...,propertyN] means <property1> .&. <property2> .&.... .&.<propertyN>

The example written as a list:
*)
let multiplyAsList (n: int, m: int) =
  let res = n*m
  sprintf "evidence = %i" res @| [
    "div1" @| (m <> 0 ==> lazy (res / m = n));
    "div2" @| (n <> 0 ==> lazy (res / n = m));
    "lt1"  @| (res > m);
    "lt2"  @| (res > n)]
(*
Produces the same result.
*)
