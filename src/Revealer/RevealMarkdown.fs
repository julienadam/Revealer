module RevealMarkdown

open Markdig
open Markdig.Syntax
open Revealer.Markdig.Extensions

let pipeline = 
    MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseGridTables()
        .Use(new ClassOnCodeInlineExtension())
        .Use(new SpeakerNotesExtension())
        .UseAutoLinks()
        .UseDiagrams()
        .UseMathematics()
        .UseEmphasisExtras()
        .UseListExtras()
        .UseYamlFrontMatter()
        .Build();

let inline toHtml (document:MarkdownDocument) = Markdown.ToHtml(document, pipeline)

let inline parse contents = Markdown.Parse(contents, pipeline)