# Command Line Usage

> This is for invoking the CLI directly. For the Cake build option, see the [Cake documentation](Cake.md) instead.

## Switches and syntax

Run DocCreator with the `-h` or `/?` options to see the inline help:

```
b:theme         Bootswatch theme to use. Defaults to plain bootstrap
i:input         Input markdown file
o:output-dir    Directory for output files
t:title         Title for the output file. Defaults to 'Documentation'.
rewrite-links   Rewrites relative .md links in processed documents
```

Each switch can be used with either `-` or `/`. That is, `-t`, `/t`, `--title` and `/title` are all equivalent.

The `input`/`-i` option accepts either a single file or a directory containing the Markdown files. Note that files must have a `.md` extension to be loaded.

The `output-dir`/`-o` option is the directory to output files into. Defaults to a temporary folder.

The `title`/`-t` option changes the default page title shown on generated pages.

### Theme support

Check the [theme documentation](Themes.md).

### Link rewriting
Using the `--rewrite-links` option will enable the (experimental) link rewriting. This will read each `.md` file and find links to other `.md` files and change them into `.html` links. This allows you to link internally between documentation pages in Markdown and have the links converted to their HTML equivalents at runtime.
