module MarkdownToReveal

open Markdig
open Markdig.Syntax
open Giraffe.ViewEngine
open System

let initScript = """
Reveal.initialize({
    hash: true,
    backgroundTransition: 'fade',
    slideNumber: 'c',
    plugins: [
        RevealMarkdown,
        RevealHighlight,
        RevealNotes,
        RevealSearch,
        RevealMath.KaTeX,
        RevealZoom,
        RevealMermaid],
    katex: {
        local: 'lib/katex'
    }
});
"""

let renderRevealHtml pageTitle theme highlightTheme content =
    let css = [
        "dist/reset.css"
        "dist/reveal.css"
        sprintf "dist/theme/%s.css" theme
        sprintf "plugin/highlight/%s.css" highlightTheme
        "revealer/revealer.css"
        "custom.css"
    ]
    let scriptRefs = [
        "dist/reveal.js"
        "plugin/notes/notes.js"
        "plugin/markdown/markdown.js"
        "plugin/search/search.js"
        "plugin/math/math.js"
        "plugin/zoom/zoom.js"
        "plugin/highlight/highlight.js"
        "plugin/mermaid/mermaid.js"
    ]
    
    html [ _lang "en"] [
        head [] [
            yield meta [ _charset "utf-8"]
            yield meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"]
            yield! css |> List.map(fun c -> link [ _rel "stylesheet"; _href c])
            yield title [] [ str pageTitle ]
        ]
        body [] [
            yield div [ _class "reveal"] [ div [_class "slides"] content ]
            yield! scriptRefs |> List.map(fun s -> script [ _src s ] [])
            yield script [] [ rawText initScript ]
        ]
    ]

let splitByThematicBlock (sourceLines:string array) (document:MarkdownDocument) (splitChar:char)= 
    document.Descendants() 
    |> Seq.toList
    |> splitWhen(fun (blk:Block) -> 
        match blk with 
        | :? ThematicBreakBlock as brk when brk.ThematicChar = splitChar -> true
        | _ -> false
    )
    |> Seq.map(fun blocks -> 
        let startLine = blocks.Head.Line
        let endLine = (blocks |> List.last).Line
        let lines = Array.sub sourceLines startLine (endLine - startLine + 1)
        lines, Markdown.Parse(String.Join(System.Environment.NewLine, lines))
        )

let buildSectionsAndSlides (sourceLines:string array) (document:MarkdownDocument) skipFirst = 
    splitByThematicBlock sourceLines document '*'
    |> Seq.skip (if skipFirst then 1 else 0)
    |> Seq.map(fun (sectionLines, sectionContents) ->
        splitByThematicBlock sectionLines sectionContents '-'
        |> Seq.map (fun (_, slideDoc) ->
            let html = slideDoc |> SlideMarkdownFormatter.markdownToHml
            section [] [ rawText html ]
        )
        |> Seq.toList
        |> section []
    ) 
    |> Seq.toList

let parseAndRender (markdownContents:string) =
    let lines = markdownContents.Split([|"\r\n"; "\n"|], StringSplitOptions.None)
    let parsed = Markdown.Parse(markdownContents)
    let options = DeckConfiguration.parseConfigurationFromDocument(parsed)
    let pageTitle = options.TryFind("title") |> Option.defaultValue "Revealer"
    let theme = options.TryFind("theme") |> Option.defaultValue "black"
    let highlightTheme = options.TryFind("highlight-theme") |> Option.defaultValue "monokai"
    printfn "\tTitle           : %s" (pageTitle |> pastelSys System.ConsoleColor.DarkGreen)
    printfn "\tTheme           : %s" (theme |> pastelSys System.ConsoleColor.DarkGreen)
    printfn "\tHighlight theme : %s" (highlightTheme |> pastelSys System.ConsoleColor.DarkGreen)

    buildSectionsAndSlides lines parsed (not options.IsEmpty)
    |> renderRevealHtml pageTitle theme highlightTheme