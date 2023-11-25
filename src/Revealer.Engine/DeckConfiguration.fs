module DeckConfiguration

open FSharp.Formatting.Markdown
open System.Text.RegularExpressions

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