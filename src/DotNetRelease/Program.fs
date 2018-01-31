// Learn more about F# at http://fsharp.org

type ReleaseOption = 
    { Token: string
      User: string
      Repository: string
      Note: string
      Tag: string
      Title: string
      Assets: string list
     }

let defaultOptions = { 
    Token = ""
    User =  ""
    Repository = ""
    Note = ""
    Tag = ""
    Title = ""
    Assets = []
}

let rec parseCommandLineRec args options = 
    match args with
    | [] -> 
        options
    | "--asset" :: xs ->
        match xs with
        | asset :: xss ->
            parseCommandLineRec xss { options with Assets = asset :: options.Assets  }
        | _ ->
            printfn "--asset needs a value"
            parseCommandLineRec xs options
    | "--note" :: xs ->
        match xs with
        | note :: xss  ->
            parseCommandLineRec xss { options with Note = note }
        | _ ->
            printfn "--note needs a value"
            parseCommandLineRec xs options
    | "--title" :: xs ->
        match xs with
        | title :: xss ->
            parseCommandLineRec xss { options with Title = title }
        | _ ->
            printfn "--title needs a value"
            parseCommandLineRec xs options
    | "--repository" :: xs ->
        match xs with
        | repo :: xss ->
            parseCommandLineRec xss { options with Repository = repo }
        | _ ->
            printfn "--repository needs a value"
            parseCommandLineRec xs options
    | "--token" :: xs ->
        match xs with
        | token::xss ->
            parseCommandLineRec xss { options with Token = token }
        | _ ->
            printfn "--token needs a value"
            parseCommandLineRec xs options
    | "--user" :: xs ->
        match xs with
        | user :: xss ->
            parseCommandLineRec xss { options with User = user }
        | _ ->
            printfn "--user needs a value"
            parseCommandLineRec xs options
    | "--tag" :: xs ->
        match xs with
        | tag :: xss ->
            parseCommandLineRec xss { options with Tag = tag }
        | _ ->
            printfn "--tag needs a value"
            parseCommandLineRec xs options
    | x :: xs ->
        printfn "Option %s is unrecognized " x
        parseCommandLineRec xs options

[<EntryPoint>]
let main argv =
    let rs = parseCommandLineRec ["--tag"; "0.1.0"; "--user"; "wk-j"] defaultOptions
    printfn "%A" rs
    0 // return an integer exit code
