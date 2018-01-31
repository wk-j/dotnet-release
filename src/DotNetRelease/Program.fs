// Learn more about F# at http://fsharp.org

type ReleaseOption = 
    { Token: string
      User: string
      Repository: string
      Note: string
      Tag: string
      Title: string
      Assets: string list
      Errors: string list
     }

let defaultOptions = { 
    Token = ""
    User =  ""
    Repository = ""
    Note = ""
    Tag = ""
    Title = ""
    Assets = []
    Errors = []
}

let rec parseCommandLineRec args options = 
    let appendError error = 
        { options with Errors = error :: options.Errors }

    match args with
    | [] -> 
        options
    | "--asset" :: xs ->
        match xs with
        | asset :: xss ->
            parseCommandLineRec xss { options with Assets = asset :: options.Assets  }
        | _ ->
            appendError "--asset needs a value"
            |> parseCommandLineRec xs
    | "--note" :: xs ->
        match xs with
        | note :: xss  ->
            parseCommandLineRec xss { options with Note = note }
        | _ ->
            appendError "--note needs a value"
            |> parseCommandLineRec xs
    | "--title" :: xs ->
        match xs with
        | title :: xss ->
            parseCommandLineRec xss { options with Title = title }
        | _ ->
            appendError "--title needs a value"
            |> parseCommandLineRec xs
    | "--repository" :: xs ->
        match xs with
        | repo :: xss ->
            parseCommandLineRec xss { options with Repository = repo }
        | _ ->
            appendError "--repository needs a value"
            |> parseCommandLineRec xs 
    | "--token" :: xs ->
        match xs with
        | token::xss ->
            parseCommandLineRec xss { options with Token = token }
        | _ ->
            appendError "--token needs a value"
            |> parseCommandLineRec xs
    | "--user" :: xs ->
        match xs with
        | user :: xss ->
            parseCommandLineRec xss { options with User = user }
        | _ ->
            appendError "--user needs a value"
            |> parseCommandLineRec xs
    | "--tag" :: xs ->
        match xs with
        | tag :: xss ->
            parseCommandLineRec xss { options with Tag = tag }
        | _ ->
            appendError "--tag needs a value"
            |> parseCommandLineRec xs
    | x :: xs ->
        appendError (sprintf "Option %s is unrecognized " x)
        |> parseCommandLineRec xs

[<EntryPoint>]
let main argv =
    let rs = parseCommandLineRec ["--tag"; "0.1.0"; "--user"; "wk-j"] defaultOptions
    printfn "%A" rs
    0 // return an integer exit code
