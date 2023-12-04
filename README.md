# Revealer

Revealer is a tool to produce slideshows from markdown files, similar to [FsReveal](https://github.com/fsprojects/FsReveal). There are many markdown to slides tools but this one is mine.

The slideshows are based on [revealjs](https://revealjs.com/) and various plugins.

The markdown syntax is similar to the [syntax used in FsReveal](http://fsprojects.github.io/FsReveal/formatting.html), with added support for [mermaid](https://mermaid.js.org/) diagrams.

Works on a folder that contains markdown files. Each markdown file produces a slideshow.

## Features

- Generate static HTML files
    - Complete with required JS and css files
    - Should not require any internet connection 
        - Unfortunately that is not quite the case yet because of some Google Fonts
- Serve pages dynamically over HTTP
    - Internal HTTP server based on [Giraffe](https://giraffe.wiki/)
    - Pages are generated dynamically from the `.md` files
    - Refreshing the page takes changes in Markdown into account, no auto-refresh
    - Should not require any internet connection 
        - CSS and JS files are served from resources in the executable
        - Unfortunately that is not quite the case yet because of some Google Fonts)
- Print slide decks to PDF
    - Uses Chromium and the HTTP server
    - Prints a black-on-white, scaled down version of the slides to pdf
- Theme overrides
    - Reveal theme and syntax highlighting themes can be overridden
- Custom css
    - The custom.css file you provide along with the .md files is automatically linked

# Markdown syntax

Based on the [Markdig](https://github.com/xoofx/markdig/) library with most its extensions enabled and a few of mine too. Follows the [CommonMark](https://commonmark.org/) spec.

## Options and metadata

As a bullet list in the first slide (FsReveal style)

```md
- title : Page title
- author : The author
- description : The description
- theme : white
- highlight-theme : monokai

***

## Slide 1
```

Or in YAML front-matter format

```md
---
title : The page title
author : The author
description : The description
theme : night
highlight-theme : zenburn
---

## Slide 1
```

## General structure


`***` marks the boundary between sections, with Revealjs sections are groups of slides you navigate with the left / right keys or arrows.

`---` marks the boundary between slides, navigate between slides of the same section with the up / down keys or arrows

## Mermaid diagrams

Code blocks with the `mermaid` language specifier are rendered as Mermaid diagrams.

## Speaker notes

Block-quotes starting with a single quote `'` are considered as speaker notes for the current slide. Access them with the `S` key.

```md
## My slide

- Some content

' These are notes.
' They will **not** appear in the slides
' They will appear as speaker notes in Reveal JS
' You can access them by pressing `S`
```

# Usage

## Serve pages over HTTP :

Command :

```sh
revealer --serve
```

Options :

- `--input <DIRECTORY>` : folder where the Markdown files reside, defaults to the current directory
- `--port <PORT>` : port for the HTTP server, defaults to 8083
- `--auto-open` : opens the default browser on the `index.html` page. Not set by default
- `--log-level <LOG_LEVEL>` : log level for the HTTP Server, defaults to `Warning`. Can be `Fatal`, `Error`, `Warning`, `Information`, `Debug` or `Trace`

Url parameters :

`theme` : overrides the Revealjs theme defined in the markdown files
`highlight-theme` : overrides the Highlight.js theme defined in the markdown files
`print-pdf` : displays the page in print-mode. Works best with Chrome compatible broswers.

## Generate static pages

For each markdown file in the input folder, generates a static HTML file into the target folder. Also copies all the support files (CSS, js etc) into the target folder, including any file present in the input folder (images...).

Command :

```sh
revealer
```

Options :

- `--input <DIRECTORY>` : folder where the Markdown files reside, defaults to the current directory
- `--output <DIRECTORY>` : folder where the static files are written, defaults to the current directory
- `--theme <THEME>` : Revealjs theme to use, defaults to `night`
- `--highlight-theme <HL_THEME>` : Highlight.js theme to use for syntax highlighting, defaults to `base16/edge-dark`
- `--auto-open` : opens the default browser on the `index.html` page. Not set by default

## Print slideshows

Runs the HTTP server, opens every page corresponding to a markdown file in print-mode and generates a PDF file. Uses Chromium internally via the [PuppeteerSharp](https://www.puppeteersharp.com/) library. You need to have write access to the installation folder in order to download the relevant files.

This one is quite opinionated, the slides as they appear on screen are bad for reading and there are overflow issues because screen size is not paper size. So the PDFs that are generated are **black on white**, **scaled down** versions of the slides.

Options are available to add headers and / or footers. Use that to add watermarks, copyright notices or any such information.

Command :

```sh
revealer --print
```

Options :

- `--input <DIRECTORY>` : folder where the Markdown files reside, defaults to the current directory
- `--output <DIRECTORY>` : folder where the PDF files are written, defaults to the current directory
- `--port <PORT>` : port for the HTTP server, defaults to 8083
- `--theme <THEME>` : Revealjs theme to use, defaults to `white`
- `--highlight-theme <HL_THEME>` : Highlight.js theme to use for syntax highlighting, defaults to `github`
- `--print-header <THE_HEADER>` : Header, defaults to no header
- `--print-footer <THE_FOOTER>` : Footer, defaults to no footer
- `--log-level <LOG_LEVEL>` : log level for the HTTP Server, defaults to `Warning`. Can be `Fatal`, `Error`, `Warning`, `Information`, `Debug` or `Trace`
