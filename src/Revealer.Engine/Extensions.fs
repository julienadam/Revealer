[<AutoOpen>]
module Extensions

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