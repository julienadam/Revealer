module StaticSiteGenerator

open Giraffe.ViewEngine
open System.IO

open RevealPageBuilder

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

let generateStaticSite inputFolder outputFolder theme highlightTheme =
    for markdownFile in Directory.GetFiles(inputFolder, "*.md") do
        printfn "Processing markdown file : %s" (markdownFile |> pastelSys System.ConsoleColor.DarkCyan)
        let source = File.ReadAllText(markdownFile)
        let rendered = parseAndRender source theme highlightTheme
        let filename = Path.Combine(outputFolder, Path.ChangeExtension(Path.GetFileName(markdownFile), ".html"))
        System.IO.File.WriteAllBytes(filename, rendered |> RenderView.AsBytes.htmlDocument)

    printfn "Extracting Reveal JS distribution files"
    Resources.extractAllResourcesTo(outputFolder)

    printfn "Copying non markdown files to output"
    copyDir inputFolder outputFolder