- theme : night
- title : Chapter 001
- description : Something or other

***

# FSharp code block

```fs
let x = ["abc"; "def"]
x 
|> Seq.filter(fun s -> s.StartsWith("a")) 
|> Seq.iter (fun s -> printfn "%s" s)
```
