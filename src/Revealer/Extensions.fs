[<AutoOpen>]
module Extensions

open Pastel
open System.IO
open System.Diagnostics

let pastelHex (color:string) (str:string) = str.Pastel(color);
let pastelSys (color:System.ConsoleColor) (str:string) = str.Pastel(color);
let pastelCol (color:System.Drawing.Color) (str:string) = str.Pastel(color);
let printError (s:string) = printfn "%s\n" (s.Pastel(System.ConsoleColor.White).PastelBg(System.ConsoleColor.Red))

let openUrlInBrowser url =
    let psi = ProcessStartInfo()
    psi.FileName <- url
    psi.UseShellExecute <- true
    Process.Start(psi) |> ignore

let copyDir source dest =
    let rec copyRecursive (source:DirectoryInfo) (dest:DirectoryInfo) =
        for f in source.GetFiles() do
            f.CopyTo(Path.Combine(dest.ToString(), f.Name), true) |> ignore

        for d in source.GetDirectories() do
            copyRecursive d (dest.CreateSubdirectory(d.Name)) |> ignore

    if Directory.Exists(source) = true then
        if Directory.Exists(dest) = false then
            Directory.CreateDirectory(dest) |> ignore

        let sourceDir = DirectoryInfo(source)
        let destDir = DirectoryInfo(dest)
        if sourceDir.FullName <> destDir.FullName then
            copyRecursive sourceDir destDir