module RevealPageBuilder

open Giraffe.ViewEngine
open Configuration
open System

let initScript = """
Reveal.initialize({
    hash: true,
    backgroundTransition: 'fade',
    slideNumber: 'c',
    pdfMaxPagesPerSlide: 1,
    mermaid: {
    },
    katex: {
        local: 'lib/katex'
    },
    customcontrols: {
        controls: [
          { icon: '<i class="fa fa-chalkboard"></i>',
            title: 'Toggle chalkboard (B)',
            action: 'RevealChalkboard.toggleChalkboard();'
          },
          { icon: '<i class="fa fa-pen"></i>',
            title: 'Toggle notes canvas (C)',
            action: 'RevealChalkboard.toggleNotesCanvas();'
          }
        ]
    },
    chalkboard: {
        
    },
    plugins: [
        RevealMarkdown,
        RevealHighlight,
        RevealNotes,
        RevealSearch,
        RevealMath.KaTeX,
        RevealZoom,
        RevealMermaid,
        RevealCustomControls,
        RevealChalkboard,
        PdfExport,
        ]
});
"""


let renderRevealHtml (options:DeckConfiguration) content =
    let css = [
        "dist/reset.css"
        "dist/reveal.css"
        sprintf "dist/theme/%s.css" options.Theme
        sprintf "plugin/highlight/%s.css" options.HighlightTheme
        "lib/font-awesome/all.min.css"
        "plugin/customcontrols/style.css"
        "plugin/chalkboard/style.css"
        "revealer/revealer.css"
        "custom.css"
    ]
    let scriptRefs = [
        "dist/reveal.js"
        "lib/font-awesome/all.min.js"
        "plugin/notes/notes.js"
        "plugin/markdown/markdown.js"
        "plugin/search/search.js"
        "plugin/math/math.js"
        "plugin/zoom/zoom.js"
        "plugin/highlight/highlight.js"
        "plugin/mermaid/mermaid.js"
        "plugin/pdfexport/pdfexport.js"
        "plugin/customcontrols/plugin.js"
        "plugin/chalkboard/plugin.js"
    ]
    
    html [ _lang "en"] [
        head [] [
            yield meta [ _charset "utf-8"]
            if String.IsNullOrEmpty( options.Author) = false then
                yield meta [ _name "author"; _content options.Author ]
            yield meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no"]
            yield! css |> List.map(fun c -> link [ _rel "stylesheet"; _href c])
            yield title [] [ str options.Title ]
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
            Configuration.parseConfigurationFromDocument(firstSlide)
        else
            None

    match options with
    | None -> Configuration.DefaultConfiguration, documents
    | Some config -> config, documents |> List.skip 1

let renderSectionAndSlides sections =
    sections |> List.map (fun slides ->
        slides |> List.map (fun slide -> 
            section [] [ rawText (slide |> RevealMarkdown.toHtml) ]
        )
        |> section []
    )

let parseAndRender markdownContents (forcedTheme: string option) (forcedHighlightTheme: string option) =
    let (options, sections) = 
        let (opts, sections) = parseSectionsAndSlides markdownContents
        let mutable opts = opts;
        if forcedTheme.IsSome then
            opts <- { opts  with Theme = forcedTheme.Value }
        if forcedHighlightTheme.IsSome then
            opts <- { opts  with HighlightTheme = forcedHighlightTheme.Value }
        (opts, sections)

    // Print in a single block to avoid interleaved messages in case of simultaneaous requests (like when printing)
    let message = 
        sprintf "\tTitle           : %s\n\tTheme           : %s\n\tHighlight theme : %s" 
            (options.Title |> pastelSys System.ConsoleColor.DarkCyan) 
            (options.Theme |> pastelSys System.ConsoleColor.DarkCyan) 
            (options.HighlightTheme |> pastelSys System.ConsoleColor.DarkCyan)
    printfn "%s" message

    sections 
    |> renderSectionAndSlides
    |> renderRevealHtml options