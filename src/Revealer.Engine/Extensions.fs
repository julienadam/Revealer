﻿[<AutoOpen>]
module Extensions

open Pastel
open System.IO

let splitWhen condition list = 
    let yieldRevNonEmpty list = 
        if list = [] then []
        else [ List.rev list ]
    
    let rec loop groupSoFar list = 
        seq { 
            match list with
            | [] -> yield! yieldRevNonEmpty groupSoFar
            | head :: tail when condition(head) -> 
                yield! yieldRevNonEmpty groupSoFar
                yield! loop [] tail
            | head :: tail -> yield! loop (head :: groupSoFar) tail
        }
    
    loop [] list |> List.ofSeq

let splitBy value list = splitWhen (fun item -> item = value) list

let pastelHex (color:string) (str:string) = str.Pastel(color);
let pastelSys (color:System.ConsoleColor) (str:string) = str.Pastel(color);
let pastelCol (color:System.Drawing.Color) (str:string) = str.Pastel(color);
let printError (s:string) = printfn "%s\n" (s.Pastel(System.ConsoleColor.White).PastelBg(System.ConsoleColor.Red))

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