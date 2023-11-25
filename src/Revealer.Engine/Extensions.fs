[<AutoOpen>]
module Extensions

open Pastel

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