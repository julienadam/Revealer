module Resources

open System.Reflection
open System.IO

let private initScriptResourceName = "/revealer/initScript.js"
let private resourcesRoot = "resources";

let getResourceStream (path:string) = Assembly.GetExecutingAssembly().GetManifestResourceStream(sprintf "%s%s" resourcesRoot path)
let initScript = (new StreamReader(getResourceStream(initScriptResourceName))).ReadToEnd()

let extractAllResourcesTo (outputFolder:string) = 
    Assembly.GetExecutingAssembly().GetManifestResourceNames()
    |> Seq.filter(fun n -> n.StartsWith(resourcesRoot) && not(n.EndsWith(initScriptResourceName)))
    |> Seq.iter(fun n ->
        let relativePath = n.Substring(10).Replace('/', Path.DirectorySeparatorChar);
        let target = Path.Combine(outputFolder, relativePath)
        Directory.CreateDirectory(Path.GetDirectoryName(target)) |> ignore
        use fs = File.Create(target)
        Assembly.GetExecutingAssembly().GetManifestResourceStream(n).CopyTo(fs);
    )