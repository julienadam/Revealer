open FSharp.Formatting.Markdown
open FSharp.Formatting.Common
open Giraffe.ViewEngine

let document =
    """

# Section 1 - Slide 1

---

# Section 1 - Slide 2

***

# Section 2 - Slide 1
"""

let parsed = Markdown.Parse(document)

let slides = 
    parsed.Paragraphs |> List.choose (fun p -> 
        match p with
        | HorizontalRule ('-', _) ->
            div [ _class "section" ] [] |> Some
        | HorizontalRule ('*', _) ->
            div [ _class "slide" ] [] |> Some
        | _ -> None
        //| Heading (size = 1; body = [ Literal (text = text) ]) ->
        //    // Recognize heading that has a simple content
        //    // containing just a literal (no other formatting)
        //    header [] [ str text] |> Some
        //| 
        //| _ -> None
    )

let view =
    html [] [
        head [] [ title [] [ str "Slide test" ] ]
        body [] slides
    ]

let result = RenderView.AsString.htmlDocument view
printfn "%s" result

