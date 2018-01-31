module Tests

open Xunit
open Program

[<Fact>]
let ``Get list of assets``() = 
    let args = ["--asset"; "A.zip"; "--asset"; "B.zip"]
    let rs = parseCommandLineRec args defaultOptions

    Assert.Equal(2, rs.Assets.Length)
    Assert.True(rs.Assets |> List.contains "A.zip")
    Assert.True(rs.Assets |> List.contains "B.zip")