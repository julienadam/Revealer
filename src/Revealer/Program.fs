open Argu
open System.IO
open Microsoft.Extensions.Logging

type CliArguments =
    | Output of path:string
    | Input of path:string
    | Serve
    | Print
    | Port of port:int
    | Auto_Open
    | Theme of string
    | HighLight_Theme of string
    | Log_level of LogLevel
    | Print_Header of string
    | Print_Footer of string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Input _ -> "Input directory containing the markdow files. Defaults to current directory"
            | Output _ -> "Output directory containing the static HTML files. Creates the directory if not found. Defaults to current directory."
            | Auto_Open -> "Automatically open the website or generated index file in a browser"
            | Serve -> "Opens a web server and serves the HTML generated from the input"
            | Port _ -> "HTTP port on which to open the web server. Defaults to 8083"
            | Theme _ -> "Reveal.JS theme to use for the slides when exporting to static HTML or printing. Ignored in web app."
            | HighLight_Theme _ -> "Highlight.JS theme to use for syntax highlighted code blocks when exporting to static HTML or printing. Ignored in web app."
            | Print -> "Prints all slide decks to PDF files. Only work in combination with --serve. Files are written to the output folder."
            | Print_Header _ -> "Header contents when printing"
            | Print_Footer _ -> "Footer contents when printing"
            | Log_level _ -> "Sets the log level. Defaults to Error."
            
let ensureDirectoryExists outputFolder = 
    if Directory.Exists(outputFolder) = false then
        Directory.CreateDirectory(outputFolder) |> ignore
    outputFolder

[<EntryPoint>]
let main _ = 
    let parser = ArgumentParser.Create<CliArguments>()
    let args = parser.ParseCommandLine()

    let inputFolder = args.GetResult(Input, defaultValue = System.Environment.CurrentDirectory)
    let isServe = args.Contains(Serve)

    if Directory.Exists(inputFolder) = false then
        sprintf "Input directory %s does not exist" inputFolder |> printError
        parser.PrintUsage() |> ignore
        -1
    else
        let autoOpen = args.Contains(Auto_Open)

        if isServe = false then
            let outputFolder = 
                args.GetResult(Output, defaultValue = System.Environment.CurrentDirectory)
                |> ensureDirectoryExists

            let theme = args.TryGetResult(Theme)
            let highlightTheme = args.TryGetResult(HighLight_Theme)

            printfn "Starting to generate static HTML"
            printfn "\tInput          : %s" (inputFolder |> pastelSys System.ConsoleColor.DarkCyan)
            printfn "\tOutput         : %s" (outputFolder |> pastelSys System.ConsoleColor.DarkCyan)
            if theme.IsSome then
                printfn "\Theme           : %s" (theme.Value |> pastelSys System.ConsoleColor.DarkCyan)
            if highlightTheme.IsSome then
                printfn "\Highlight Theme : %s" (highlightTheme.Value |> pastelSys System.ConsoleColor.DarkCyan)

            StaticSiteGenerator.generateStaticSite inputFolder outputFolder theme highlightTheme
            if autoOpen then
                let url = Path.Combine(outputFolder, "index.html")
                printfn "Auto-opening %s in browser" (url |> pastelSys System.ConsoleColor.DarkCyan)
                openUrlInBrowser url
        else
            let port = args.GetResult(Port, defaultValue = 8083)
            printfn "Starting server on port %s" (port.ToString() |> pastelSys System.ConsoleColor.DarkCyan)
            let logLevel = args.GetResult(Log_level, defaultValue = LogLevel.Warning)
            printfn "Logging set to %s" (logLevel.ToString() |> pastelSys System.ConsoleColor.DarkCyan)
            let appTask = WebApp.startAsync inputFolder port logLevel
            let print = args.Contains(Print);
            if print then
                let theme = args.TryGetResult(Theme) |> Option.defaultValue "white"
                let highlightTheme = args.TryGetResult(HighLight_Theme) |> Option.defaultValue "github"
                let outputFolder = 
                    args.GetResult(Output, defaultValue = System.Environment.CurrentDirectory)
                    |> ensureDirectoryExists
                let header = args.TryGetResult(Print_Header)
                let footer = args.TryGetResult(Print_Footer)
                
                printfn "Starting to print slides"
                printfn "\tInput           : %s" (inputFolder |> pastelSys System.ConsoleColor.DarkCyan)
                printfn "\tOutput          : %s" (outputFolder |> pastelSys System.ConsoleColor.DarkCyan)
                printfn "\tTheme           : %s" (theme |> pastelSys System.ConsoleColor.DarkCyan)
                printfn "\tHighlight Theme : %s" (highlightTheme |> pastelSys System.ConsoleColor.DarkCyan)

                let generatePdfsTask = SlidePrinter.generateAllPdfs inputFolder outputFolder theme highlightTheme port header footer
                generatePdfsTask.Wait()
            else
                if autoOpen then
                    let url = sprintf "http://localhost:%i/index.html" port
                    printfn "Auto-opening %s in browser" (url |> pastelSys System.ConsoleColor.DarkCyan)
                    openUrlInBrowser url
                appTask.Wait();
        0