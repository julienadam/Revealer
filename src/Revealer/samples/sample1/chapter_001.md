---
theme : night
title : Chapter 001
author : foo
description : Something or other
highlight-theme : zenburn
---

## FSharp code block

```fs
let x = ["abc"; "def"]
x 
|> Seq.filter(fun s -> s.StartsWith("a")) 
|> Seq.iter (fun s -> printfn "%s" s)
```
