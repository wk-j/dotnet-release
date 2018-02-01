module Tests

open Xunit
open DotNetRelease.GitHub

[<Fact>]
let ``Get list of assets``() = 
    let args = ["--asset"; "A.zip"; "--asset"; "B.zip"]
    let rs = parseCommandLineOptions args 

    Assert.Equal(2, rs.Assets.Length)
    Assert.True(rs.Assets |> List.contains "A.zip")
    Assert.True(rs.Assets |> List.contains "B.zip")

[<Fact>]
let ``Get list of erros``() = 
    let args = ["--foo"; "--asset"]
    let rs = parseCommandLineOptions args 

    Assert.Equal(2,rs.Errors.Length)