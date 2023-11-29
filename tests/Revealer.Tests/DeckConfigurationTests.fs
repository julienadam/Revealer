namespace Revealer.Tests

open Xunit
open Faqt
open Markdig
open Configuration

module ``Deck configuration loading`` =
    
    let validBulletConfig = """
- theme : red
- title : A Foo In a Bar
- author : J.
- description : A very nice description
- highlight-theme : green

"""

    [<Fact>]
    let ``Valid list block configuration is recognized`` () =
        let doc = RevealMarkdown.parse(validBulletConfig)
        let config, skip = Configuration.parseConfigurationFromDocument(doc)
        
        let expected = {
            Theme = "red"
            Title = "A Foo In a Bar"
            Author = "J."
            HighlightTheme = "green"
            Description = "A very nice description"
        }

        config
            .Should()
            .Be(expected) |> ignore

        skip
            .Should()
            .Be(true) |> ignore

    let validYamlConfig = """---
theme : red
title : A Foo In a Bar
author : J.
description : A very nice description
highlight-theme : green
---

"""

    [<Fact>]
    let ``Valid yaml front matter is recognized`` () =
        let doc = RevealMarkdown.parse(validYamlConfig)
        let config, skip = Configuration.parseConfigurationFromDocument(doc)
        
        let expected = {
            Theme = "red"
            Title = "A Foo In a Bar"
            Author = "J."
            HighlightTheme = "green"
            Description = "A very nice description"
        }

        config
            .Should()
            .Be(expected) |> ignore

        skip
            .Should()
            .Be(false) |> ignore

    [<Fact>]
    let ``Invalid list block returns default configuration but is not skipped`` () =
        let doc = RevealMarkdown.parse("- foobar\n\n")
        let config, skip = Configuration.parseConfigurationFromDocument(doc)
        config
            .Should()
            .Be(DefaultConfiguration) |> ignore

        skip
            .Should()
            .Be(false) |> ignore

    let invalidYamlConfig = """---
@sldjkhgfjk
---

"""

    [<Fact>]
    let ``Invalid yaml front matter is ignored and returns default configuration`` () =
        let doc = RevealMarkdown.parse(invalidYamlConfig )
        let config, skip = Configuration.parseConfigurationFromDocument(doc)
        config
            .Should()
            .Be(DefaultConfiguration) |> ignore

        skip
            .Should()
            .Be(false) |> ignore
