open Octokit
open System.IO

type ReleaseOptions = 
    { Token: string
      Owner: string
      Repository: string
      Body: string
      Tag: string
      Name: string
      Assets: string list
      Errors: string list
     }

let defaultOptions = { 
    Token = ""
    Owner = ""
    Repository = ""
    Body = ""
    Tag = ""
    Name = ""
    Assets = []
    Errors = []
}

module GitHub = 
    let createReleaseo options = 
        let client = GitHubClient(ProductHeaderValue("my-cool-app"))
        client.Credentials <- Credentials(options.Token)

        let release = NewRelease(options.Tag)
        release.Name <- options.Name
        release.Body <- options.Body
        release.Prerelease <- false
        release.Draft <- true

        let newRelease  = 
            client.Repository.Release.Create(options.Owner, options.Repository, release) 
            |> Async.AwaitTask
            |> Async.RunSynchronously

        let uploadAsset release file = 
            let info = FileInfo(file: string)
            let raw = File.OpenRead(file)
            let asset = ReleaseAssetUpload()
            asset.FileName <- info.Name
            asset.RawData <- raw

            client.Repository.Release.UploadAsset(release, asset)
            |> Async.AwaitTask
            |> Async.RunSynchronously

        options.Assets |> List.map (uploadAsset newRelease)

let rec parseCommandLineRec args options = 
    let appendError error = 
        { options with Errors = error :: options.Errors }

    match args with
    | [] -> options

    | "--asset" :: xs ->
        match xs with
        | asset :: xss ->
            parseCommandLineRec xss { options with Assets = asset :: options.Assets  }
        | _ ->
            appendError "Option --asset needs a value"
            |> parseCommandLineRec xs

    | "--body" :: xs ->
        match xs with
        | note :: xss  ->
            parseCommandLineRec xss { options with Body = note }
        | _ ->
            appendError "Option --body needs a value"
            |> parseCommandLineRec xs
            
    | "--name" :: xs ->
        match xs with
        | title :: xss ->
            parseCommandLineRec xss { options with Name = title }
        | _ ->
            appendError "Option --name needs a value"
            |> parseCommandLineRec xs

    | "--repository" :: xs ->
        match xs with
        | repo :: xss ->
            parseCommandLineRec xss { options with Repository = repo }
        | _ ->
            appendError "Option --repository needs a value"
            |> parseCommandLineRec xs 

    | "--token" :: xs ->
        match xs with
        | token::xss ->
            parseCommandLineRec xss { options with Token = token }
        | _ ->
            appendError "Option --token needs a value"
            |> parseCommandLineRec xs

    | "--owner" :: xs ->
        match xs with
        | user :: xss ->
            parseCommandLineRec xss { options with Owner = user }
        | _ ->
            appendError "Option --owner needs a value"
            |> parseCommandLineRec xs

    | "--tag" :: xs ->
        match xs with
        | tag :: xss ->
            parseCommandLineRec xss { options with Tag = tag }
        | _ ->
            appendError "Option --tag needs a value"
            |> parseCommandLineRec xs

    | x :: xs ->
        appendError (sprintf "Option %s is unrecognized " x)
        |> parseCommandLineRec xs

[<EntryPoint>]
let main argv =
    let rs = parseCommandLineRec (argv |> Array.toList) defaultOptions
    printfn "%A" rs
    0 // return an integer exit code
