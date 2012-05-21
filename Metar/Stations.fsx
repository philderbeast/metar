#load "Stations.fs"

open System

Environment.CurrentDirectory <- @"C:\temp"
MeridianArc.Metar._Stations.cacheFile()