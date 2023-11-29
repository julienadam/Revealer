module SlidePrinter

open PuppeteerSharp
open System
open System.IO
open System.Threading.Tasks

let ensureBrowser () = task {
    use browserFetcher = new BrowserFetcher()
    return! browserFetcher.DownloadAsync()
}

let getPrintingOptions header footer = 
    let inline styleHeaderAndFooter text = sprintf "<span style=\"font-size: 12px; padding-left: 5px \">%s</span>" text
    let options = new PdfOptions()
    options.Landscape <- true
    let marginOptions = new Media.MarginOptions()
    marginOptions.Bottom <- "1cm"
    marginOptions.Top <- "1cm"
    marginOptions.Left <- "1cm"
    marginOptions.Right <- "1cm"
    options.MarginOptions <- marginOptions
    options.Format <- Media.PaperFormat.A4
    options.PrintBackground <- true
    options.Tagged <- true
    match header, footer with
    | None, None -> ()
    | _ -> 
        options.DisplayHeaderFooter <- true
        options.HeaderTemplate <- styleHeaderAndFooter (header |> Option.defaultValue "")
        options.FooterTemplate <- styleHeaderAndFooter (footer |> Option.defaultValue "")
        ()
    options

let generateSinglePdf url outputFile header footer = task {
    use! browser = Puppeteer.LaunchAsync(new LaunchOptions(Headless = true))
    use! page = browser.NewPageAsync()
    // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.
    let! _ = page.GoToAsync(url)
    // Wait for fonts to be loaded. Omitting this might result in no text rendered in pdf.
    let! _ = page.EvaluateExpressionHandleAsync("document.fonts.ready") 

    // Wait for 3 seconds for the whole page to load. Not very precise but I can't find a way to know when
    // everything is finished redering at the moment
    let! _ = Task.Delay(TimeSpan.FromSeconds(3))
    return! page.PdfAsync(outputFile, getPrintingOptions header footer)
}

let generateAllPdfs inputFolder outputFolder theme highlightTheme port header footer = task {
    let! _ = ensureBrowser ()

    let tasks = 
        Directory.GetFiles(inputFolder, "*.md") 
        |> Array.map (fun markdownFile ->
            printfn "Processing markdown file : %s" (markdownFile |> pastelSys System.ConsoleColor.DarkCyan)
            let path = Path.GetFileNameWithoutExtension(markdownFile);
            let url = sprintf "http://localhost:%i/%s.html?theme=%s&highlight-theme=%s&print-pdf" port path theme highlightTheme
            generateSinglePdf url (Path.Combine(outputFolder, sprintf "%s.pdf" path)) header footer
        )
        |> Array.map (fun t -> t :> Task)
    
    Task.WaitAll(tasks)
}