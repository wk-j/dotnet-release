// Learn more about F# at http://fsharp.org

open DotNetRelease.GitHub

[<EntryPoint>]
let main argv =
    let options = parseCommandLineOptions (argv |> Array.toList) 

    createNewRelease options
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore

    0 // return an integer exit code


