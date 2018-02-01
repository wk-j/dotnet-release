// Learn more about F# at http://fsharp.org

open DotNetRelease.GitHub

[<EntryPoint>]
let main argv =
    let options = parseCommandLineOptions (argv |> Array.toList) 
    let result = createNewRelease options
    0 // return an integer exit code


