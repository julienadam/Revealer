open Argu
open System.IO
open System.Diagnostics

type CliArguments =
    | Output of path:string
    | Input of path:string
    | Auto_Open
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Input _ -> "Input directory containing the markdow files"
            | Output _ -> "Output directory containing the static HTML files"
            | Auto_Open -> "Automatically open the website or generated index file in a browser"

let parser = ArgumentParser.Create<CliArguments>()

let result = parser.ParseCommandLine()
let inputFolder = result.GetResult(Input, defaultValue = System.Environment.CurrentDirectory)
let outputFolder = result.GetResult(Output, defaultValue = System.Environment.CurrentDirectory)
let autoOpen = result.Contains(Auto_Open)

if Directory.Exists(inputFolder) = false then
    sprintf "Input directory %s does not exist" inputFolder |> printError
    parser.PrintUsage() |> ignore
    System.Environment.Exit(-1)

if Directory.Exists(outputFolder) = false then
    Directory.CreateDirectory(outputFolder) |> ignore

StaticSiteGenerator.generateStaticSite inputFolder outputFolder
if autoOpen then
    let psi = ProcessStartInfo()
    psi.FileName <- Path.Combine(outputFolder, "index.html")
    psi.UseShellExecute <- true
    Process.Start(psi) |> ignore