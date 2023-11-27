module DeckConfiguration

open System.Text.RegularExpressions
open Markdig.Syntax

/// Parse metadata from the first slide. Each bullet point in the form "name : value" 
/// represents a key / value configuration
/// TODO: use a YAML front-matter syntax supported by the parser
let parseConfigurationFromDocument (document:MarkdownDocument) =
    let re = Regex "^\s*(?<name>[a-z-]+)\s*:\s*(?<value>.*)$"

    let mutable state = Map.empty
    match (document.Item(0)) with
    | :? ListBlock as listBlock when listBlock.IsOrdered = false ->
        listBlock.Descendants<ListItemBlock>()
        |> Seq.iter(fun listItem ->
            match listItem.Item(0) with
            | :? ParagraphBlock as p ->
                match p.Inline.FirstChild with 
                | :? Inlines.LiteralInline as lit ->
                    let m = re.Match(lit.Content.ToString())
                    if m.Success then
                        let k = m.Groups.["name"].Value.ToLower().Trim()
                        let v = m.Groups.["value"].Value.Trim()
                        state <- state |> Map.add k v 
                | _ -> ()
            | _ -> ()
        )
    | _ -> 
        ()
    state