namespace MeridianArc.Metar

open System
open System.IO
open System.Net
open System.Text
open Microsoft.FSharp.Control.WebExtensions

module internal _Stations =
    let download(url : Uri) =
        async {
            let client = new WebClient()
            client.Encoding <- Encoding.GetEncoding("utf-8")
            let! html = client.AsyncDownloadString(url)
            return html 
        }

    [<Literal>]
    let stationsUri = "http://weather.noaa.gov/data/nsd_cccc.txt"
    let cacheFileName = "nsd_cccc.txt"

    let saveStations (contents : string) =
        let streamWriter = File.CreateText cacheFileName
        streamWriter.Write contents
        streamWriter.Close()

    let cacheFile() =
        let cached = File.Exists cacheFileName
        match cached with
            | true -> ()
            | false -> 
                let contents = download (new Uri(stationsUri))
                contents |> Async.RunSynchronously |> saveStations

open _Stations

type Stations() =
    do
        cacheFile()