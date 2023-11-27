module SlideMarkdownFormatter

open Markdig
open Markdig.Syntax
open Markdig.Renderers
open Markdig.Syntax.Inlines
open Markdig.Renderers.Html
open Markdig.Renderers.Html.Inlines

type CustomCodeInlineRenderer() =
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
                htmlRenderer.ObjectRenderers.Replace<CodeInlineRenderer>(new CustomCodeInlineRenderer()) |> ignore
            | _ -> ()

let markdownToHml (document:MarkdownDocument) =
    let pipeline = 
        MarkdownPipelineBuilder()
            .Use(new ClassOnCodeInlineExtension())
            .UseAutoLinks()
            .UseDiagrams()
            .UseEmphasisExtras()
            .UseGridTables()
            .UseMathematics()
            .UsePipeTables()
            .UseListExtras()
            .UseYamlFrontMatter()
            .Build();
    Markdown.ToHtml(document, pipeline);
