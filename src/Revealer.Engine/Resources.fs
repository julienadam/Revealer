module Resources

open System.Reflection
open System.IO

let initScript = (new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Revealer.resources.initScript.js"))).ReadToEnd()
let inlineStyles = (new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Revealer.resources.revealer.css"))).ReadToEnd()