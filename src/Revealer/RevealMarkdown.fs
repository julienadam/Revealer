module RevealMarkdown

open Markdig
open Markdig.Syntax
open Markdig.Renderers
open Markdig.Syntax.Inlines
open Markdig.Renderers.Html
open Markdig.Renderers.Html.Inlines

type ClassOnCodeInlineRenderer() =
    inherit CodeInlineRenderer()
    override _.Write(renderer : HtmlRenderer, obj : CodeInline) = 
        let attrs = new HtmlAttributes()
        attrs.AddClass("code-span")
        obj.SetAttributes(attrs);
        base.Write(renderer, obj)

type ClassOnCodeInlineExtension () =
    interface IMarkdownExtension with
        member _.Setup(_ : MarkdownPipelineBuilder) = ()
        member _.Setup(_ : MarkdownPipeline, renderer : IMarkdownRenderer) =
            match renderer with 
            | :? HtmlRenderer as htmlRenderer ->
                htmlRenderer.ObjectRenderers.Replace<CodeInlineRenderer>(new ClassOnCodeInlineRenderer()) |> ignore
            | _ -> ()

let pipeline = 
    MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseGridTables()
        .Use(new ClassOnCodeInlineExtension())
        .UseAutoLinks()
        .UseDiagrams()
        .UseMathematics()
        .UseEmphasisExtras()
        .UseListExtras()
        .UseYamlFrontMatter()
        .Build();

let inline toHtml (document:MarkdownDocument) = Markdown.ToHtml(document, pipeline)

let inline parse contents = Markdown.Parse(contents, pipeline)