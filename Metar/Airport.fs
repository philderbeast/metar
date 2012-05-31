namespace BlockScope.Metar

type AirportCode =
    | ICAO of string
    | IATA of string

type Location = { Name: string; City: string; Country: string }

type LatLong = { Lat: float; Long: float }

type Airport = {
    Location: Location;
    Icao: AirportCode;
    Iata: AirportCode;
    Coords: LatLong}

