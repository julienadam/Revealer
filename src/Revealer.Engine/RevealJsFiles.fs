module RevealJsFiles

open System.Reflection
open System.IO.Compression

let getRevealZip () =
    let resourceName = "Revealer.revealjs.zip"
    let stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
    new ZipArchive(stream)