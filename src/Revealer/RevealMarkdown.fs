module RevealMarkdown

open Markdig
open Markdig.Syntax
open Revealer.Markdig.Extensions

let pipeline = 
    MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseGridTables()
        .UseDiagrams() // must be before LineNumbersCodeBlockExtension
        .Use(new ClassOnCodeInlineExtension())
        .Use(new SpeakerNotesExtension())
        .Use(new LineNumbersCodeBlockExtension())
        .UseAutoLinks()
        .UseMathematics()
        .UseEmphasisExtras()
        .UseListExtras()
        .UseYamlFrontMatter()
        .Build();

let inline toHtml (document:MarkdownDocument) = Markdown.ToHtml(document, pipeline)

let inline parse contents = Markdown.Parse(contents, pipeline)