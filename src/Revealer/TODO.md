## Features

- Config option to auto reload on markdown changes when hosting in the built-in server
- Allow YAML front matter instead of bullet list for configuration
- Configuration should be richer and translate into Reveal configuration
- Allow specifying paper size when printing
- Support highlighting specific lines in a code block
- Add favicon

## Bugs

- Some Mermaid diagrams are not rendering, refreshing page seems to bring them back somehow ?
- Remove all CDN downloads, replace with local ones
	- themes almost all download some fonts ...
- Fix either or both :
	- Indented code blocks between list elements don't take their full size, scrollbars appear
	- Non-indented code block between list items results in the list items being centered ...
