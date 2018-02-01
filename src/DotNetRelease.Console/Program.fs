// Learn more about F# at http://fsharp.org

open DotNetRelease.GitHub

[<EntryPoint>]
let main argv =
    let rs = parseCommandLineOptions (argv |> Array.toList) 
    printfn "%A" rs
    0 // return an integer exit code


