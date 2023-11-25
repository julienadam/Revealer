open Argu
open System.IO

type CliArguments =
    | Output of path:string
    | Input of path:string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Input _ -> "input directory containing the markdow files"
            | Output _ -> "output directory containing the static HTML files"

let parser = ArgumentParser.Create<CliArguments>()

let result = parser.ParseCommandLine()
let inputFolder = result.GetResult(Input, defaultValue = System.Environment.CurrentDirectory)
let outputFolder = result.GetResult(Output, defaultValue = System.Environment.CurrentDirectory)

if Directory.Exists(inputFolder) = false then
    sprintf "Input directory %s does not exist" inputFolder |> printError
    parser.PrintUsage() |> ignore
    System.Environment.Exit(-1)

if Directory.Exists(outputFolder) = false then
    sprintf "Output directory %s does not exist" outputFolder |> printError
    parser.PrintUsage() |> ignore
    System.Environment.Exit(-2)

StaticSiteGenerator.generateStaticSite inputFolder outputFolder