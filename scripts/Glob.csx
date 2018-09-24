#! "netcoreapp2.1"
#r "nuget:Glob,0.4.0"

using System;
using Glob = Glob.Glob;
using Glob;

var match = new Glob(".publsh/*.npkg");
var files = new DirectoryInfo("./").GlobFiles("**/*.nupkg");


foreach (var item in files) {
    Console.WriteLine(item.FullName);
}
