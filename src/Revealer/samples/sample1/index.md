﻿- theme : night
- title : Foo !

***

## Links

- Internal (html): [First chapter](chapter_001.html)
- Internal (md): [First chapter](chapter_001.md)
- External : [Wikipedia](http://www.wikipedia.org)

---

## Basic formatting

- Strong : **example**
- Italics : _example_
- Italics alternative : *italics*
- Inline code : `example` <- bug if the example does not appear in red on white
- Quote before italics : '_example_ <- bug if the quote before example does not show
- Italics + strong : **_example_**

---

## Formatting bug test

- Formatting on the first char of a bullet should work
- _italics_
- **bold**
- **_both_** <- bug here if * appears

---

## Image

![Alt text for image](images/fsharp256.png)

---

## Speaker notes

- Some content
- Press `S` to open speaker notes

' trying out notes
' on multiple lines
'without a space
and what happens if there is no quote ?
' maybe it continues

---

## Speaker notes and quote

- Press `S` to open speaker notes

> Foo
> Bar

' note1
' note2

---

## Math

$x / y = cos(56.2)^2$

---

## YAML code block

```yaml
foo: false
bar: baz
```

---

## Non ascii chars

- sdlkjfhjdskjlfhlkjs kljh dkljfhs dkjfh 
	- ço_è_çesqèfxmdxzstr^àç)do podqikpomlio

---

## Table

| Col1 | Column 2   | Wow   |
|------|------------|-------|
| 42   | Some value | Nope! |

---

## Code blocks in list items

- Markdown example

```md
## Foo
- bar inside
```

- Sql example

```sql
SELECT TOP 10 * FROM Customer;
```



***

# Mermaid samples

---

## Mindmap

```mermaid
mindmap
root((mindmap))
    Origins
    Long history
    ::icon(fa fa-book)
    Popularisation
        British popular psychology author Tony Buzan
    Research
    On effectiveness and features
    On Automatic creation
        Uses
            Creative techniques
            Strategic planning
            Argument mapping
    Tools
    Pen and paper
    Mermaid
```

---

## Gitgraph

```mermaid
gitGraph
commit
commit
branch develop
checkout develop
commit
commit
checkout main
merge develop
commit
commit
```

---

## Requirements

```mermaid
requirementDiagram

requirement test_req {
id: 1
text: the test text.
risk: high
verifymethod: test
}
          
element test_entity {
type: simulation
}
          
test_entity - satisfies -> test_req
```

---

## Chart

```mermaid
pie title Pets adopted by volunteers
"Dogs" : 386
"Cats" : 85
"Rats" : 15
```

---

## Gantt

```mermaid
gantt
title A Gantt Diagram
dateFormat  YYYY-MM-DD
section Section
A task           :a1, 2014-01-01, 30d
Another task     :after a1  , 20d
section Another
Task in sec      :2014-01-12  , 12d
another task     : 24d
```

---

## User journey

```mermaid
journey
title My working day
section Go to work
    Make tea: 5: Me
    Go upstairs: 3: Me
    Do work: 1: Me, Cat
section Go home
    Go downstairs: 5: Me
    Sit down: 5: Me
```

---

## ER

```mermaid
erDiagram
CUSTOMER ||--o{ ORDER : places
ORDER ||--|{ LINE-ITEM : contains
CUSTOMER }|..|{ DELIVERY-ADDRESS : uses
```

---

## State

```mermaid
stateDiagram-v2
[*] --> Still
Still --> [*]

Still --> Moving
Moving --> Still
Moving --> Crash
Crash --> [*]
```

---

## Class

```mermaid
classDiagram
Animal <|-- Duck
Animal <|-- Fish
Animal <|-- Zebra
Animal : +int age
Animal : +String gender
Animal: +isMammal()
Animal: +mate()
class Duck{
    +String beakColor
    +swim()
    +quack()
}
class Fish{
    -int sizeInFeet
    -canEat()
}
class Zebra{
    +bool is_wild
    +run()
}
```

---

## Seq

```mermaid
sequenceDiagram
Alice->>John: Hello John, how are you?
John-->>Alice: Great!
Alice-)John: See you later!
```
---

## Flow

```mermaid
flowchart TD
A[Start] --> B{Is it?};
B -- Yes --> C[OK];
C --> D[Rethink];
D --> B;
B -- No ----> E[End];
```