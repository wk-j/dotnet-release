module DotNetRelease.GitHub

open Octokit
open System.IO
open Serilog

let logger = LoggerConfiguration().WriteTo.Console().CreateLogger()

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

let createRelease (client: GitHubClient) owner repository release = 
    let newRelease  = 
        client.Repository.Release.Create(owner, repository, release) 
        |> Async.AwaitTask
        |> Async.RunSynchronously
    (newRelease)


[<CompiledName("UploadAsset")>]
let uploadAsset (client: GitHubClient) release file = 
    let info = FileInfo(file)
    logger.Information("is asset exist - {0} - {1}", file, File.Exists file)

    let releaseAsset = 
        let str = new MemoryStream()
        let bytes = File.ReadAllBytes(file)
        str.Write(bytes, 0, bytes.Length)
        str.Seek(0L, SeekOrigin.Begin) |> ignore

        let extension = (Path.GetExtension file).ToUpper()

        let asset = ReleaseAssetUpload()
        asset.FileName <- info.Name
        asset.RawData <- str
        asset.ContentType <-
            match extension with
            | ".ZIP" -> "application/zip"
            | ".MSI" | ".EXE" | ".DLL" | ".NUPKG" -> "application/octet-stream"
            | "_" -> "application/octet-stream"
            | _ -> "application/octet-stream"

        client.Repository.Release.UploadAsset(release, asset)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    releaseAsset

[<CompiledName("CreateRelease")>]
let createNewRelease (options: ReleaseOptions) = 
    logger.Information("create new release - {0}", options.Tag)

    let client = GitHubClient(ProductHeaderValue("my-cool-app"))
    client.Credentials <- Credentials(options.Token)

    let release = NewRelease(options.Tag)
    release.Name <- options.Name
    release.Body <- options.Body
    release.Prerelease <- false
    release.Draft <- false

    let newRelease = createRelease client (options.Owner) (options.Repository) release
    logger.Information("new release result - {0}", newRelease.Url)

    let lastest = 
        client.Repository.Release.Get(options.Owner, options.Repository, newRelease.Id) 
        |> Async.AwaitTask
        |> Async.RunSynchronously

    logger.Information("latest release - {0}", lastest.Url)

    let assets = 
        options.Assets |> List.map (uploadAsset client lastest) 

    for asset in assets do
        logger.Information("asset - {0}", asset.BrowserDownloadUrl)

let rec private parseCommandLineRec args options = 
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

[<CompiledName("ParseCommandLineOptions")>]
let parseCommandLineOptions args = 

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

    parseCommandLineRec args defaultOptions