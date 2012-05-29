#load "Stations.fs"

open System

Environment.CurrentDirectory <- @"C:\temp"
BlockScope.Metar._Stations.cacheFile()