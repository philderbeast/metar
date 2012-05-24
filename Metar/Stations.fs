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


(*
SOURCE: http://weather.noaa.gov/tg/site.shtml

Keyed by Index (Block and Station) Number
-----------------------------------------
All stations that have an index number assigned by the WMO are available in this file.
The ASCII file contains the following fields in the order specified, delimited by semi-colons.
Fields that are empty will be coded as no characters (adjacent delimiters) except where
otherwise noted.
Block Number	2 digits representing the WMO-assigned block.
Station Number 	 3 digits representing the WMO-assigned station. 
ICAO Location
Indicator 	 4 alphanumeric characters, not all stations in this file have an assigned location indicator. The value "----" is used for stations that do not have an assigned location indicator. 
Place Name 	 Common name of station location. 
State 	 2 character abbreviation (included for stations located in the United States only). 
Country Name 	 Country name is ISO short English form. 
WMO Region 	 digits 1 through 6 representing the corresponding WMO region, 7 stands for the WMO Antarctic region. 
Station Latitude 	 DD-MM-SSH where DD is degrees, MM is minutes, SS is seconds and H is N for northern hemisphere or S for southern hemisphere. The seconds value is omitted for those stations where the seconds value is unknown. 
Station Longitude 	 DDD-MM-SSH where DDD is degrees, MM is minutes, SS is seconds and H is E for eastern hemisphere or W for western hemisphere. The seconds value is omitted for those stations where the seconds value is unknown. 
Upper Air Latitude 	 DD-MM-SSH where DD is degrees, MM is minutes, SS is seconds and H is N for northern hemisphere or S for southern hemisphere. The seconds value is omitted for those stations where the seconds value is unknown. 
Upper Air Longitude 	 DDD-MM-SSH where DDD is degrees, MM is minutes, SS is seconds and H is E for eastern hemisphere or W for western hemisphere. The seconds value is omitted for those stations where the seconds value is unknown. 
Station Elevation (Ha) 	 The station elevation in meters. Value is omitted if unknown. 
Upper Air Elevation (Hp) 	 The upper air elevation in meters. Value is omitted if unknown. 
RBSN indicator 	P if station is defined by the WMO as belonging to the Regional Basic Synoptic Network, omitted otherwise. 

Download the Station Information Text File keyed by index number:
http://weather.noaa.gov/data/nsd_bbsss.txt
http://weather.noaa.gov/data/nsd_bbsss.gz
http://weather.noaa.gov/data/nsd_bbsss.zip

Keyed by Location Indicator
---------------------------
All stations that have a location indicator assigned by The ICAO are available in this file.
The ASCII file contains The following fields in The order specified, delimited by semicolons.
Fields that are empty will be coded as no characters (adjacent delimiters) except where
otherwise noted.
ICAO Location
Indicator 	 4 alphanumeric characters. 
Block Number	 2 digits representing The WMO-assigned block. Not all stations in The file have an assigned block number. The value "--" is used for station without an assigned number. 
Station Number 	 3 digits representing The WMO-assigned station. Not all stations in The file have an assigned station number. The value "---" is used for station without an assigned number. 
Place Name 	 Common name of station location. 
State 	 2 character abbreviation (included for stations located in The United States only). 
Country Name 	 Country name is ISO short English form. 
WMO Region 	 Digits 1 through 6 representing The corresponding WMO region, 7 stands for The WMO Antarctic region. 
Station Latitude 	 DD-MM-SSH where DD is degrees, MM is minutes, SS is seconds and H is N for northern hemisphere or S for southern hemisphere. The seconds value is omitted for those stations where The seconds value is unknown. 
Station Longitude 	 DDD-MM-SSH where DDD is degrees, MM is minutes, SS is seconds and H is E for eastern hemisphere or W for western hemisphere. The seconds value is omitted for those stations where The seconds value is unknown. 
Upper Air Latitude 	 DD-MM-SSH where DD is degrees, MM is minutes, SS is seconds and H is N for northern hemisphere or S for southern hemisphere. The seconds value is omitted for those stations where The seconds value is unknown. 
Upper Air Longitude 	 DDD-MM-SSH where DDD is degrees, MM is minutes, SS is seconds and H is E for eastern hemisphere or W for western hemisphere. The seconds value is omitted for those stations where The seconds value is unknown. 
Station Elevation (Ha)	The station elevation in meters. Value is omitted if unknown.
Upper Air Elevation (Hp) 	 The upper air elevation in meters. Value is omitted if unknown. 
RBSN indicator 	P if station is defined by The WMO as belonging to The Regional Basic Synoptic Network, omitted otherwise.

Download The Station Information Text File keyed by location indicator:
http://weather.noaa.gov/data/nsd_cccc.txt
http://weather.noaa.gov/data/nsd_cccc.gz
http://weather.noaa.gov/data/nsd_cccc.zip
*)
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