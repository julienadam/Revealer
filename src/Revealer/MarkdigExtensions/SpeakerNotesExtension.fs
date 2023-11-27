namespace Revealer.Markdig.Extensions

open Markdig
open Markdig.Syntax
open Markdig.Renderers
open Markdig.Renderers.Html
open Markdig.Renderers.Html.Inlines
open Markdig.Parsers

type SpeakerNoteQuoteBlockParser () as this =
    inherit QuoteBlockParser()
    do
        this.OpeningCharacters <- [|'\''|]

type SpeakerNoteQuoteBlockRenderer () =
    inherit QuoteBlockRenderer()
    override _.Write(renderer : HtmlRenderer, obj : QuoteBlock) = 
        match obj.QuoteChar with
        | '\'' ->
            renderer.EnsureLine() |> ignore
            if renderer.EnableHtmlForBlock then
                renderer.Write("<aside class=\"notes\" ") |> ignore
                renderer.WriteAttributes(obj) |> ignore
                renderer.WriteLine('>') |> ignore
            let savedImplicitParagraph = renderer.ImplicitParagraph
            renderer.ImplicitParagraph <- true
            let lineRenderer = renderer.ObjectRenderers.FindExact<LineBreakInlineRenderer>();
            let savedRenderAsHardlineBreak = lineRenderer.RenderAsHardlineBreak
            lineRenderer.RenderAsHardlineBreak <- true
            renderer.WriteChildren(obj)
            lineRenderer.RenderAsHardlineBreak <- savedRenderAsHardlineBreak
            renderer.ImplicitParagraph <- savedImplicitParagraph
            if renderer.EnableHtmlForBlock then
                renderer.WriteLine("</aside>") |> ignore
            renderer.EnsureLine() |> ignore
        | _ -> 
            base.Write(renderer, obj)

type SpeakerNotesExtension () =
    interface IMarkdownExtension with
        member _.Setup(builder : MarkdownPipelineBuilder) = 
            builder.BlockParsers.Add(new SpeakerNoteQuoteBlockParser())
        member _.Setup(_ : MarkdownPipeline, renderer : IMarkdownRenderer) = 
            match renderer with 
            | :? HtmlRenderer as htmlRenderer ->
                htmlRenderer.ObjectRenderers.Replace<QuoteBlockRenderer>(new SpeakerNoteQuoteBlockRenderer()) |> ignore
            | _ -> ()

