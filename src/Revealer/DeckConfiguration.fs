module DeckConfiguration

open System.Text.RegularExpressions
open Markdig.Syntax

let internal extractLiteralsFromListBlock (listBlock:ListBlock) = seq {
    for listItem in listBlock.Descendants<ListItemBlock>() do
        match listItem |> Seq.head with
        | :? ParagraphBlock as p ->
            match p.Inline.FirstChild with 
            | :? Inlines.LiteralInline as lit ->
               yield lit.Content.ToString()
            | _ -> ()
        | _ -> ()
}

/// Parse metadata from the first slide. Each bullet point in the form "name : value" 
/// represents a key / value configuration
/// TODO: use a YAML front-matter syntax supported by the parser
let parseConfigurationFromDocument (document:MarkdownDocument) =
    let re = Regex "^\s*(?<name>[a-z-]+)\s*:\s*(?<value>.*)$"

    let extractKeyValue lit = 
        let m = re.Match(lit)
        if m.Success then
            let k = m.Groups.["name"].Value.ToLower().Trim()
            let v = m.Groups.["value"].Value.Trim()
            Some (k,v)
        else None

    match document |> Seq.head with
    | :? ListBlock as listBlock when listBlock.IsOrdered = false ->
        extractLiteralsFromListBlock listBlock
        |> Seq.choose extractKeyValue
        |> Map.ofSeq
    | _ -> 
        Map.empty
