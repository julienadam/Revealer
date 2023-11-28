namespace Revealer.Markdig.Extensions

open Markdig
open Markdig.Syntax
open Markdig.Renderers
open Markdig.Renderers.Html

type LineNumbersCodeBlockRenderer () =
    inherit CodeBlockRenderer()
    override this.Write(renderer : HtmlRenderer, obj : CodeBlock) = 
        // Only add the line num attribute for code clock that are not rendered as divs
        match obj with
        | :? FencedCodeBlock as fenced when this.BlocksAsDiv.Contains(fenced.Info) ->
            ()
        | _ ->
            obj.GetAttributes().AddProperty("data-line-numbers", null)
        base.Write(renderer, obj)

type LineNumbersCodeBlockExtension () =
    interface IMarkdownExtension with
        member _.Setup(_ : MarkdownPipelineBuilder) = ()
        member _.Setup(_ : MarkdownPipeline, renderer : IMarkdownRenderer) = 
            ()
            match renderer with 
            | :? HtmlRenderer as htmlRenderer ->
                let existing = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>()
                let replacement = new LineNumbersCodeBlockRenderer()
                replacement.BlocksAsDiv.UnionWith(existing.BlocksAsDiv)
                replacement.OutputAttributesOnPre <- existing.OutputAttributesOnPre
                htmlRenderer.ObjectRenderers.Replace<CodeBlockRenderer>(replacement) |> ignore
            | _ -> ()