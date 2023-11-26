Reveal.initialize({
    hash: true,
    backgroundTransition: 'fade',
    slideNumber: 'c',
    plugins: [
        RevealMarkdown,
        RevealHighlight,
        RevealNotes,
        RevealSearch,
        RevealMath.KaTeX,
        RevealZoom,
        RevealMermaid],
    katex: {
        local: 'lib/katex'
    }
});