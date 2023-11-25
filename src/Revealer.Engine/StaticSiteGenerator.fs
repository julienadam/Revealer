module StaticSiteGenerator

open FSharp.Formatting.Markdown
open Giraffe.ViewEngine
open System.IO

open MarkdownToReveal
open System.IO.Compression

let copyDir source dest =
    let rec copyRecursive (source:DirectoryInfo) (dest:DirectoryInfo) =
        for f in source.GetFiles() do
            f.CopyTo(Path.Combine(dest.ToString(), f.Name), true) |> ignore

        for d in source.GetDirectories() do
            copyRecursive d (dest.CreateSubdirectory(d.Name)) |> ignore

    if Directory.Exists(source) = true then
        if Directory.Exists(dest) = false then
            Directory.CreateDirectory(dest) |> ignore

        let sourceDir = DirectoryInfo(source)
        let destDir = DirectoryInfo(dest)
        if sourceDir.FullName <> destDir.FullName then
            copyRecursive sourceDir destDir

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

    printfn "Copying non markdown files to output"
    copyDir inputFolder outputFolder