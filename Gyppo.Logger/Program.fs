open Gyppo.Logger.Common
open Gyppo.Logger.Syntax

let getDirectory args =
    match args with
    | [|path|] -> path
    | _ -> ""

[<EntryPoint>]
let main argv =
    getDirectory argv
    |> getSourceFiles
    |> Seq.choose processSourceFile
    |> Seq.iter writeSourceFile
    0 // return an integer exit code