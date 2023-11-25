module StaticSiteGenerator

open FSharp.Formatting.Markdown
open Giraffe.ViewEngine
open System.IO

open MarkdownToReveal
open System.IO.Compression

let generateStaticSite inputFolder outputFolder =

    let markdownFiles = Directory.GetFiles(inputFolder, "*.md")

    let parseAndRender markdownContents =
        let parsed = Markdown.Parse(markdownContents)
        let options = DeckConfiguration.parseConfigurationFromDocument parsed.Paragraphs
        let pageTitle = options.TryFind("title") |> Option.defaultValue "Revealer"
        let theme = options.TryFind("theme") |> Option.defaultValue "black"
        printfn "\tTheme : %s" (theme |> pastelSys System.ConsoleColor.DarkGreen)
        printfn "\tTitle  : %s" (pageTitle |> pastelSys System.ConsoleColor.DarkGreen)

        parsed
        |> buildSectionsAndSlides
        |> renderRevealHtml pageTitle theme

    for markdownFile in markdownFiles do
        printfn "Processing markdown file : %s" (markdownFile |> pastelSys System.ConsoleColor.DarkCyan)
        let source = File.ReadAllText(markdownFile)
        let rendered = parseAndRender source
        let filename = Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(markdownFile), ".html"))
        System.IO.File.WriteAllBytes(filename, rendered |> RenderView.AsBytes.htmlDocument)

    printfn "Extracting Reveal JS distribution files"
    RevealJsFiles.getRevealZip().ExtractToDirectory(outputFolder, true)