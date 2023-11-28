module RevealPageBuilder

open Giraffe.ViewEngine

let initScript = """
Reveal.initialize({
    hash: true,
    backgroundTransition: 'fade',
    slideNumber: 'c',
    pdfMaxPagesPerSlide: 1,
    plugins: [
        RevealMarkdown,
        RevealHighlight,
        RevealNotes,
        RevealSearch,
        RevealMath.KaTeX,
        RevealZoom,
        PdfExport,
        ],
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
        "plugin/pdfexport/pdfexport.js"
    ]
    
    html [ _lang "en"] [
        head [] [
            yield meta [ _charset "utf-8"]
            yield meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"]
            yield! css |> List.map(fun c -> link [ _rel "stylesheet"; _href c])
            yield title [] [ str pageTitle ]
            yield link [_rel "apple-touch-icon"; _sizes "180x180"; _href "/apple-touch-icon.png"]
            yield link [_rel "icon"; _type "image/png"; _sizes "32x32"; _href "/favicon-32x32.png"]
            yield link [_rel "icon"; _type "image/png"; _sizes "16x16"; _href "/favicon-16x16.png"]
            yield link [_rel "manifest"; _href "/site.webmanifest"]
        ]
        body [] [
            yield div [ _class "reveal"] [ div [_class "slides"] content ]
            yield! scriptRefs |> List.map(fun s -> script [ _src s ] [])
            yield script [] [ rawText initScript ]
        ]
    ]


let parseSectionsAndSlides source = 
    let documents = 
        MarkdownSplitter.splitSectionsAndSlides source
        |> Seq.map (fun section -> 
            section 
            |> Seq.map RevealMarkdown.parse
            |> Seq.toList
        )
        |> Seq.toList

    let options = 
        if (not documents.IsEmpty) && (not documents.Head.IsEmpty) then
            let firstSlide = documents.Head.Head
            DeckConfiguration.parseConfigurationFromDocument(firstSlide)
        else
            Map.empty

    match options.IsEmpty with
    | true -> options, documents
    | false -> options, documents |> List.skip 1


let renderSectionAndSlides sections =
    sections |> List.map (fun slides ->
        slides |> List.map (fun slide -> 
            section [] [ rawText (slide |> RevealMarkdown.toHtml) ]
        )
        |> section []
    )

let parseAndRender markdownContents forcedTheme forcedHighlightTheme =
    let (options, sections) = parseSectionsAndSlides markdownContents
    let pageTitle = options.TryFind("title") |> Option.defaultValue "Revealer"
    let theme = 
        forcedTheme 
        |> Option.defaultValue (options.TryFind("theme") |> Option.defaultValue "black")
    let highlightTheme = 
        forcedHighlightTheme 
        |> Option.defaultValue (options.TryFind("highlight-theme") |> Option.defaultValue "base16/edge-dark")
    // Print in a single block to avoid interleaved messages in case of simultaneaous requests (like when printing)
    let message = 
        sprintf "\tTitle           : %s\n\tTheme           : %s\n\tHighlight theme : %s" 
            (pageTitle |> pastelSys System.ConsoleColor.DarkCyan) 
            (theme |> pastelSys System.ConsoleColor.DarkCyan) 
            (highlightTheme |> pastelSys System.ConsoleColor.DarkCyan)
    printfn "%s" message

    sections 
    |> renderSectionAndSlides
    |> renderRevealHtml pageTitle theme highlightTheme