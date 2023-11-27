namespace Revealer.Tests

open Xunit
open Faqt

module ``Markdown splitter`` =
    
    [<Fact>]
    let ``Markdown without any thematic block is left as is`` () =
        let sample = """"# First section"""
        let blocks = MarkdownSplitter.splitByThematicBlock sample '*' |> Seq.toList
        blocks.Length.Should().Be(1) |> ignore
        blocks.Head.Should().Be(sample)

    [<Fact>]
    let ``Markdown with one thematic block is split into 2 results`` () =
        let sample = """# First section
***
# Second section"""

        let blocks = MarkdownSplitter.splitByThematicBlock sample '*' |> Seq.toList
        blocks.Length.Should().Be(2) |> ignore
        (blocks.Item 0).Should().Be("# First section") |> ignore
        (blocks.Item 1).Should().Be("# Second section") |> ignore

    [<Fact>]
    let ``Markdown 2 sections with 2 slides each is split correctly`` () =
        let sample = """# Slide1.1
---
# Slide1.2
---
# Slide1.3
***
# Slide2.1
---
# Slide2.2
---
# Slide2.3"""

        let blocks = MarkdownSplitter.splitSectionsAndSlides sample |> Seq.toList
        blocks.Length.Should().Be(2) |> ignore
        let section1 = blocks.Item 0
        let section2 = blocks.Item 1
        
        section1.Should().SequenceEqual([
            "# Slide1.1"
            "# Slide1.2"
            "# Slide1.3"
        ]) |> ignore

        section2.Should().SequenceEqual([
            "# Slide2.1"
            "# Slide2.2"
            "# Slide2.3"
        ]) |> ignore


