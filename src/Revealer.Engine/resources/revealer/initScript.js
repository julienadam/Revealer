const codeElements = document.getElementsByTagName('code');
for (const codeElement of codeElements) {
    var found = false;
    codeElement.classList.forEach((cls) => {
        if (cls.startsWith("language-")) {
            found = true;
        }
    }
    );

    if (found) {
        codeElement.setAttribute("data-line-numbers", "");
    } else {
        codeElement.classList.add("code-span");
    }
}

Reveal.initialize({
    hash: true,
    backgroundTransition: 'fade',
    slideNumber: 'c',
    plugins: [
        RevealMarkdown,
        RevealHighlight,
        RevealNotes,
        RevealSearch,
        RevealMath,
        RevealZoom,
        RevealMermaid]
});