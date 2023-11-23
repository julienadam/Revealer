open FSharp.Formatting.Markdown
open FSharp.Formatting.Common
open Giraffe.ViewEngine
open System.Text.RegularExpressions

let document =
    """
- theme : beige
- title : Foo !

***

# Section 1 - Slide 1

---

# Section 1 - Slide 2

***

# Section 2 - Slide 1
"""

let parsed = Markdown.Parse(document)

let parseOptionsFromDocument paragraphs =
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


//let slides = 
//    parsed.Paragraphs |> List.choose (fun p -> 
//        match p with
//        | HorizontalRule ('-', _) ->
//            div [ _class "section" ] [ str "section" ] |> Some
//        | HorizontalRule ('*', _) ->
//            div [ _class "slide" ] [ str "slide" ] |> Some
//        | _ -> None
//        //| Heading (size = 1; body = [ Literal (text = text) ]) ->
//        //    // Recognize heading that has a simple content
//        //    // containing just a literal (no other formatting)
//        //    header [] [ str text] |> Some
//        //| 
//        //| _ -> None
//    )

let sectionsAndSlides = [
        section [] [ str "Section 1"]
        section [] [ str "Section 2"]
    ]

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

let options = parseOptionsFromDocument parsed.Paragraphs
let pageTitle = options.TryFind("title") |> Option.defaultValue "Revealer"
let theme = options.TryFind("theme") |> Option.defaultValue "black"

System.IO.File.WriteAllBytes("index.html", 
    sectionsAndSlides
    |> renderRevealHtml pageTitle theme
    |> RenderView.AsBytes.htmlDocument)
