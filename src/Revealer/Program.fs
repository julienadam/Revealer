open Argu
open System.IO

type CliArguments =
    | Output of path:string
    | Input of path:string
    | Serve
    | Port of port:int
    | Auto_Open
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Input _ -> "Input directory containing the markdow files. Defaults to current directory"
            | Output _ -> "Output directory containing the static HTML files. Creates the directory if not found. Defaults to current directory."
            | Auto_Open -> "Automatically open the website or generated index file in a browser"
            | Serve -> "Opens a web server and serves the HTML generated from the input"
            | Port _ -> "HTTP port on which to open the web server. Defaults to 8083"

[<EntryPoint>]
let main _ = 
    let parser = ArgumentParser.Create<CliArguments>()
    let result = parser.ParseCommandLine()

    let inputFolder = result.GetResult(Input, defaultValue = System.Environment.CurrentDirectory)
    let isServe = result.Contains(Serve)

    if Directory.Exists(inputFolder) = false then
        sprintf "Input directory %s does not exist" inputFolder |> printError
        parser.PrintUsage() |> ignore
        -1
    else
        let autoOpen = result.Contains(Auto_Open)

        if isServe = false then
            let outputFolder = result.GetResult(Output, defaultValue = System.Environment.CurrentDirectory)

            if Directory.Exists(outputFolder) = false then
                Directory.CreateDirectory(outputFolder) |> ignore

            StaticSiteGenerator.generateStaticSite inputFolder outputFolder
            if autoOpen then
                Path.Combine(outputFolder, "index.html") |> openUrlInBrowser
        else
            let port = result.GetResult(Port, defaultValue = 8083)
            let appTask = WebApp.startAsync inputFolder port
            if autoOpen then
                sprintf "http://localhost:%i/index.html" port |> openUrlInBrowser
            appTask.Wait();

        0