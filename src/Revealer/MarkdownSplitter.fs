module MarkdownSplitter

open Markdig.Syntax
open System
open Markdig
open System.Text

type Section = string seq

let splitByThematicBlock (source:string) splitChar = 
    let lines = source.Split([|"\r\n"; "\n"|], StringSplitOptions.None)
    let document = Markdown.Parse(source)
    // Put all lines where a thematic block appears in a Set
    let breakLines = 
        document.Descendants() 
        |> Seq.filter(fun (blk:Block) -> 
            match blk with 
            | :? ThematicBreakBlock as brk when brk.ThematicChar = splitChar -> true
            | _ -> false)
        |> Seq.map(fun blk -> blk.Line)
        |> Set.ofSeq

    // Accumulate lines one by one, yielding every time a break is found
    seq {
        let mutable builder = new StringBuilder();
        for i = 0 to lines.Length - 1 do 
            if breakLines.Contains i then
                yield builder.ToString()
                builder <- new StringBuilder();
            else
                if builder.Length > 0 then
                    builder.Append("\n") |> ignore
                builder.Append(lines[i]) |> ignore
        yield builder.ToString()
    }

let splitSectionsAndSlides (source:string) : Section seq =
    splitByThematicBlock source '*'
    |> Seq.map(fun sectionContents ->
        splitByThematicBlock sectionContents '-'
    )
