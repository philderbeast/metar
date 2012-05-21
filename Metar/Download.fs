namespace MeridianArc.Metar

open System
open System.IO
open System.Net

module internal _Download =
    let reqHTTP uri = WebRequest.Create(uri : Uri) :?> HttpWebRequest

    let req (cccc : string) =
        let path = "http://weather.noaa.gov/pub/data/observations/metar/stations/" + cccc.ToUpper() + ".TXT"
        let req = reqHTTP(new Uri(path))
        let resp = req.GetResponse()
        use stream = resp.GetResponseStream()
        use sr = new StreamReader(stream, Text.Encoding.UTF8)
        sr.ReadToEnd()
        
open _Download

type Download() =
    static member MetarFor(cccc) = req cccc 