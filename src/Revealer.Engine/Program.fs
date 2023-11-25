open FSharp.Formatting.Markdown
open Generator
open Giraffe.ViewEngine

let document =
    """
- theme : beige
- title : Foo !

***

# Section 1
' trying out notes
' on multiple lines

---

## Hey

- [google](http://www.google.fr) is bad for your health

***

## Section 2

---

# Foo

```yaml
foo: false
bar: baz
```

"""

let parsed = Markdown.Parse(document)
let options = DeckConfiguration.parseConfigurationFromDocument parsed.Paragraphs
let pageTitle = options.TryFind("title") |> Option.defaultValue "Revealer"
let theme = options.TryFind("theme") |> Option.defaultValue "black"

System.IO.File.WriteAllBytes("index.html", 
    parsed
    |> buildSectionsAndSlides
    |> renderRevealHtml pageTitle theme
    |> RenderView.AsBytes.htmlDocument)
