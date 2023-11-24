﻿open FSharp.Formatting.Markdown
open Giraffe.ViewEngine
open System.Text.RegularExpressions

let document =
    """
- theme : beige
- title : Foo !

***

# Section 1

---

## Hey

- [google](http://www.google.fr) is bad for your health

***

## Section 2

---

# Foo

```yaml
foo: false
bar: baz
```

"""

let parsed = Markdown.Parse(document)

/// Parse metadata from the first slide. Each bullet point in the form "name : value" 
/// represents a key / value configuration
/// TODO: use a YAML front-matter syntax supported by the parser
let parseConfigurationFromDocument paragraphs =
    let re = Regex "^\s*(?<name>[a-z]+)\s*:\s*(?<value>.*)$"

    let readOptions (items:MarkdownParagraphs list) =
        let mutable state = Map.empty
        items |> Seq.iter (fun item -> 
            match item with
            | [ Span ([Literal (str, _)], _) ] -> 
                let m = re.Match(str)
                if m.Success then
                    state <- Map.add (m.Groups.["name"].Value.ToLower().Trim()) (m.Groups.["value"].Value.Trim()) state
            | _ -> ()
        )
        state

    match paragraphs |> Seq.tryHead  with
    | Some (ListBlock (MarkdownListKind.Unordered, items, _)) -> 
        readOptions items
    | _ ->
        Map.empty


/// Split sections identified by a *** horizontal rule into groups of slides
/// The first "section" is reserved for metadata and is skipped
/// TODO: use a YAML front-matter syntax supported by the parser
let getSections (paragraphs : MarkdownParagraphs) = seq {
    let mutable sectionParagraphs = []
    for p in paragraphs |> Seq.skip 1 do
        match p with
        | HorizontalRule ('*', _) ->
            if sectionParagraphs <> [] then
                yield sectionParagraphs
            sectionParagraphs <- []
        | _ -> 
            sectionParagraphs <- List.append sectionParagraphs [p]

    if sectionParagraphs <> [] then
        yield sectionParagraphs
}

let getSlides (paragraphs : MarkdownParagraphs) = seq {
    let mutable slideParagraphs = []
    for p in paragraphs do
        match p with
        | HorizontalRule ('-', _) ->
            if slideParagraphs <> [] then
                yield slideParagraphs
            slideParagraphs <- []
        | _ -> 
            slideParagraphs <- List.append slideParagraphs [p]

    if slideParagraphs <> [] then
        yield slideParagraphs
}

let sections = getSections parsed.Paragraphs |> Seq.toList

let sectionsAndSlides = 
    sections 
    |> List.map(fun sectionContents -> 
        getSlides sectionContents
        |> Seq.toList
        |> List.map (fun slideContents ->
            let doc = FSharp.Formatting.Markdown.MarkdownDocument(slideContents, parsed.DefinedLinks)
            section [] [
                rawText (Markdown.ToHtml(doc))
            ]
        )
        |> section []
    )

let renderRevealHtml pageTitle theme content =
    let themeCss = sprintf "dist/theme/%s.css" theme;
    let css = ["dist/reset.css"; "dist/reveal.css"; themeCss; "plugin/highlight/monokai.css"]
    let scriptRefs = ["dist/reveal.js"; "plugin/notes/notes.js"; "plugin/markdown/markdown.js"; "plugin/highlight/highlight.js"]
    let initScript = "Reveal.initialize({ hash: true, plugins: [ RevealMarkdown, RevealHighlight, RevealNotes ] });"

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
            yield script [] [ str initScript ]
        ]
    ]

let options = parseConfigurationFromDocument parsed.Paragraphs
let pageTitle = options.TryFind("title") |> Option.defaultValue "Revealer"
let theme = options.TryFind("theme") |> Option.defaultValue "black"

System.IO.File.WriteAllBytes("index.html", 
    sectionsAndSlides
    |> renderRevealHtml pageTitle theme
    |> RenderView.AsBytes.htmlDocument)
