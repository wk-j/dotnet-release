module DotNetRelease.GitHub

open Octokit
open System.IO
open Serilog
open FSharp.Control.Tasks
open System

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
    client.Repository.Release.Create(owner, repository, release) 

[<CompiledName("UploadAsset")>]
let uploadAsset (client: GitHubClient) release file = 
    let info = FileInfo(file)
    logger.Information("is asset exist - {0} - {1}", file, File.Exists file)

    let releaseAsset = 
        let bytes = File.ReadAllBytes(file)
        let extension = (Path.GetExtension file).ToUpper()
        let asset = ReleaseAssetUpload()
        asset.FileName <- info.Name
        asset.RawData <- new MemoryStream(bytes)
        asset.ContentType <-
            match extension with
            | ".ZIP" -> "application/zip"
            | ".MSI" | ".EXE" | ".DLL" | ".NUPKG" -> "application/octet-stream"
            | "_" -> "application/octet-stream"
            | _ -> "application/octet-stream"

        logger.Information("uploading asset - {0}", asset.FileName)
        let upload = client.Repository.Release.UploadAsset(release, asset)
        upload.Wait()
        upload.Result
    releaseAsset

[<CompiledName("CreateRelease")>]
let createNewRelease (options: ReleaseOptions) = 
    logger.Information("create new release - {0}", options.Tag)

    let client = GitHubClient(ProductHeaderValue("my-wk-app"))
    client.SetRequestTimeout(TimeSpan.FromMinutes(20.0))
    client.Credentials <- Credentials(options.Token)

    let release = NewRelease(options.Tag)
    release.Name <- options.Name
    release.Body <- options.Body
    release.Prerelease <- false
    release.Draft <- false

    task { 
        let! newRelease = createRelease client (options.Owner) (options.Repository) release
        logger.Information("new release result - {0}", newRelease.Url)

        let! lastest = client.Repository.Release.Get(options.Owner, options.Repository, newRelease.Id) 
        logger.Information("latest release - {0}", lastest.Url)

        for item in options.Assets do
            let rs = uploadAsset client lastest item
            logger.Information ("url - {0}", rs.BrowserDownloadUrl)
    }

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
        Owner = "wk-j"
        Repository = ""
        Body = ""
        Tag = ""
        Name = ""
        Assets = []
        Errors = []
    }

    parseCommandLineRec args defaultOptions