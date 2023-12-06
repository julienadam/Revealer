module Configuration

open System.Text.RegularExpressions
open Markdig.Syntax
open Markdig.Extensions.Yaml
open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions

[<CLIMutable>]
type DeckConfiguration = {
    Theme : string
    HighlightTheme : string
    Description : string
    Author : string
    Title : string
}

let DefaultConfiguration = { 
    Theme = "night"
    HighlightTheme = "base16/edge-dark-mod"
    Description = ""
    Author = ""
    Title = ""
}

// Parses metadata from a YAML Frontmatter block
let internal parseFrontMatter (yamlBlock:YamlFrontMatterBlock) =
    let deserializer = 
        DeserializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build()
    try
        deserializer.Deserialize<DeckConfiguration>(yamlBlock.Lines.ToString())
    with
    | _ -> 
        printError "Could not parse YAML front matter. Using default configuration"
        DefaultConfiguration

let internal RegexNameValue = Regex "^\s*(?<name>[a-z-]+)\s*:\s*(?<value>.*)$"

/// Parse metadata from the first slide. Each bullet point in the form "name : value" 
/// represents a key / value configuration
let internal parseListBlockConfiguration (listBlock:ListBlock) =
    let extractLiteralsFromListBlock (listBlock:ListBlock) = seq {
        for listItem in listBlock.Descendants<ListItemBlock>() do
            match listItem |> Seq.head with
            | :? ParagraphBlock as p ->
                match p.Inline.FirstChild with 
                | :? Inlines.LiteralInline as lit ->
                   yield lit.Content.ToString()
                | _ -> ()
            | _ -> ()
    }

    let extractKeyValue lit = 
        let m = RegexNameValue.Match(lit)
        if m.Success then
            let k = m.Groups.["name"].Value.ToLower().Trim()
            let v = m.Groups.["value"].Value.Trim()
            (k,v)
        else
            failwithf "not a valid key value pair"
    
    let keyValues = 
        extractLiteralsFromListBlock listBlock
        |> Seq.map extractKeyValue
        |> Map.ofSeq

    { 
        Theme = keyValues.TryFind "theme" |> Option.defaultValue DefaultConfiguration.Theme
        HighlightTheme = keyValues.TryFind "highlight-theme" |> Option.defaultValue DefaultConfiguration.HighlightTheme
        Author = keyValues.TryFind "author" |> Option.defaultValue DefaultConfiguration.Author
        Title = keyValues.TryFind "title" |> Option.defaultValue DefaultConfiguration.Title
        Description = keyValues.TryFind "description" |> Option.defaultValue DefaultConfiguration.Description
    }
    
let parseConfigurationFromDocument (document:MarkdownDocument) =
    
    match document |> Seq.head with
    | :? YamlFrontMatterBlock as yamlBlock ->
        parseFrontMatter yamlBlock, false
    | :? ListBlock as listBlock when listBlock.IsOrdered = false ->
        try
            parseListBlockConfiguration listBlock, true
        with
        | _ -> 
            DefaultConfiguration, false
    | _ -> 
        DefaultConfiguration, false

