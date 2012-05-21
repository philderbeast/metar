open System
open System.IO
open System.Net
open System.Text
open Microsoft.FSharp.Control.WebExtensions

let reqFTP(uri : Uri) : FtpWebRequest =
    let req = WebRequest.Create(uri)
    let asFtp = req :?> FtpWebRequest
    asFtp.Credentials <- new NetworkCredential("anonymous", "anonymous@meridianarc.com")
    asFtp.UsePassive <- true
    asFtp

let req = reqFTP(new Uri("ftp://tgftp.nws.noaa.gov/data/observations/metar/stations/NZCH.TXT"))

let resp = req.GetResponse()
let stream = resp.GetResponseStream()
let sr = new StreamReader(stream, Text.Encoding.UTF8)
let contents = sr.ReadToEnd()
sr.Close()
stream.Close()

let reqHTTP uri = WebRequest.Create(uri : Uri) :?> HttpWebRequest

let req = reqHTTP(new Uri("http://weather.noaa.gov/pub/data/observations/metar/stations/NZAA.TXT"))

let resp = req.GetResponse()
let stream = resp.GetResponseStream()
let sr = new StreamReader(stream, Text.Encoding.UTF8)
let contents = sr.ReadToEnd()
sr.Close()
stream.Close()