﻿namespace BlockScope

module PrimitiveParsers =
    open Parser
    open FParsec
    open BlockScope.Metar

    let data = AirportCodes.Codes.airports
    let upperCaps : Parser<_> = regex "[A-Z]"
    let icao = quoted (regex "[A-Z]{4,4}") |>> (fun s -> ICAO(s)) <?> "ICAO 4 letter code in uppercaps"
    let iata = quoted (regex "[A-Z]{3,3}") |>> (fun s -> IATA(s)) <?> "IATA 3 letter code in uppercaps"
    let codes = pipe3 iata (str ",") icao (fun iata _ icao -> (iata, icao))
    let latlong = pipe3 pfloat (str ",") pfloat (fun lat _ long -> { Lat = lat; Long = long})
    let listOfStrings = (sepBy quotedStr (str ","))
    let thenCodes = (str ",") >>. codes
    let countries =
        let each = quotedStr .>> (str ",")
        pipe3 each each quotedStr (fun a b c -> {Name = a; City = b; Country = c})

module OpenFlights =
    open PrimitiveParsers
    open Parser
    open FParsec
    open BlockScope.Metar

    let line =
        let index = pint32 .>> (str ",")
        let thenCodes = (str ",") >>. codes
        let thenLatLong = (str ",") >>. latlong .>> skipRestOfLine false 
        pipe4 index countries thenCodes thenLatLong
            (fun _ c (c3, c4) l -> { Location = c; Iata = c3; Icao = c4; Coords = l})
    let lines =
        sepBy line newline

(*
1,"Goroka","Goroka","Papua New Guinea","GKA","AYGA",-6.081689,145.391881,5282,10,"U"
2,"Madang","Madang","Papua New Guinea","MAG","AYMD",-5.207083,145.7887,20,10,"U"
3,"Mount Hagen","Mount Hagen","Papua New Guinea","HGU","AYMH",-5.826789,144.295861,5388,10,"U"

This
Airport ID	Unique OpenFlights identifier for this airport. 
Name	Name of airport. May or may not contain the City name.
City	Main city served by airport. May be spelled differently from Name.
Country	Country or territory where airport is located.
IATA/FAA	3-letter FAA code, for airports located in Country "United States of America".
 3-letter IATA code, for all other airports.
 Blank if not assigned.
ICAO	4-letter ICAO code.
 Blank if not assigned.
Latitude	Decimal degrees, usually to six significant digits. Negative is South, positive is North.
Longitude	Decimal degrees, usually to six significant digits. Negative is West, positive is East.
Altitude	In feet.
Timezone	Hours offset from UTC. Fractional hours are expressed as decimals, eg. India is 5.5.
DST	Daylight savings time. One of E (Europe), A (US/Canada), S (South America), O (Australia), Z (New Zealand), N (None) or U (Unknown). See also: Help: Time
SOURCE: http://openflights.org/data.html
*)

