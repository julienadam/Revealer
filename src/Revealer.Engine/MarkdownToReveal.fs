module MarkdownToReveal

open FSharp.Formatting.Markdown
open Giraffe.ViewEngine
open System

/// Split sections identified by a *** horizontal rule into groups of slides
/// The first "section" is reserved for metadata and is skipped
let splitSections (paragraphs : MarkdownParagraphs) = 
    paragraphs 
    |> splitWhen (function | HorizontalRule ('*', _) -> true | _ -> false)
    |> List.skip 1

let splitSlides (paragraphs : MarkdownParagraphs) = 
    paragraphs 
    |> splitWhen (function | HorizontalRule ('-', _) -> true | _ -> false)

/// Replaces paragraphs with literals starting with "'" with the relevant markup
/// for notes (i.e <aside> tags with lines separated by <br/>)
let processSpeakerNotes (paragraphs:MarkdownParagraphs) = 
    /// Puts all lines starting with a quote in a single inline HTML block
    /// containing an <aside> element with each line separated by <br/>.
    let produceSingleNoteFromMutilineText (str:string) =
        let notes =
            str.Split([|"\r\n"; "\n"|], StringSplitOptions.RemoveEmptyEntries)
            |> Array.filter (fun s -> s.StartsWith("'"))
            |> Array.map (fun s -> s.Substring(1))
        
        let notesParagraph = String.Join("<br/>", notes)
        let html = sprintf "<aside class=\"notes\">%s</aside><br/>" (notesParagraph)
        MarkdownParagraph.InlineHtmlBlock(html, None, None)
    
    paragraphs 
    |> List.map (fun p ->
        match p with 
        | Paragraph ([Literal (s, _)], _) -> produceSingleNoteFromMutilineText s
        | _ -> p
    )

let buildSectionsAndSlides (document:MarkdownDocument) = 
    document.Paragraphs
    |> splitSections
    |> Seq.map(fun sectionContents -> 
        splitSlides sectionContents
        |> Seq.toList
        |> List.map (fun slideContents ->
            let slidesWithNotes = processSpeakerNotes slideContents
            let doc = MarkdownDocument(slidesWithNotes, document.DefinedLinks)
            section [] [ rawText (Markdown.ToHtml(doc)) ]
        )
        |> section []
    ) 
    |> Seq.toList

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
            yield script [] [ rawText Resources.initScript ]
        ]
    ]

let parseAndRender markdownContents =
    let parsed = Markdown.Parse(markdownContents)
    let options = DeckConfiguration.parseConfigurationFromDocument parsed.Paragraphs
    let pageTitle = options.TryFind("title") |> Option.defaultValue "Revealer"
    let theme = options.TryFind("theme") |> Option.defaultValue "black"
    let highlightTheme = options.TryFind("highlight-theme") |> Option.defaultValue "monokai"
    printfn "\tTitle           : %s" (pageTitle |> pastelSys System.ConsoleColor.DarkGreen)
    printfn "\tTheme           : %s" (theme |> pastelSys System.ConsoleColor.DarkGreen)
    printfn "\tHighlight theme : %s" (highlightTheme |> pastelSys System.ConsoleColor.DarkGreen)

    parsed
    |> buildSectionsAndSlides
    |> renderRevealHtml pageTitle theme highlightTheme