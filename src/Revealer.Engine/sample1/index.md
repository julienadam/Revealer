- theme : night
- title : Foo !

***

## links

- Internal : [First chapter](chapter_001.html)
- External : [Wikipedia](http://www.wikipedia.org)

---

## Inline code

- Foo `bar` super `baz`

---

## Image

![Alt text for image](images/fsharp256.png)

---

# Speaker notes

- Some content

' trying out notes
' on multiple lines
'without a space
and what happens if there is no quote ?
' maybe it continues

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
| ---  | --------   | ---   |
| 42   | Some value | Nope! |

---

## Code blocks in list items

- Sql example

```sql
SELECT TOP 10 * FROM Customer;
```

- Markdown example
  
```md
## Foo
- Wow much nesting
```

***

# Mermaid samples

---

## Mindmap

<div class="mermaid">
<pre>
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
</pre>
</div>

---

## Gitgraph

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
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
</pre>
</div>

---

## Requirements

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
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
</pre>
</div>

---

## Chart

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
    pie title Pets adopted by volunteers
    "Dogs" : 386
    "Cats" : 85
    "Rats" : 15
</pre>
</div>

---

## Gantt

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
    gantt
    title A Gantt Diagram
    dateFormat  YYYY-MM-DD
    section Section
    A task           :a1, 2014-01-01, 30d
    Another task     :after a1  , 20d
    section Another
    Task in sec      :2014-01-12  , 12d
    another task     : 24d
</pre>
</div>

---

## User journey

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
    journey
    title My working day
    section Go to work
        Make tea: 5: Me
        Go upstairs: 3: Me
        Do work: 1: Me, Cat
    section Go home
        Go downstairs: 5: Me
        Sit down: 5: Me
</pre>
</div>

---

## ER

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
    erDiagram
    CUSTOMER ||--o{ ORDER : places
    ORDER ||--|{ LINE-ITEM : contains
    CUSTOMER }|..|{ DELIVERY-ADDRESS : uses
</pre>
</div>

---

## State

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
    stateDiagram-v2
    [*] --> Still
    Still --> [*]

    Still --> Moving
    Moving --> Still
    Moving --> Crash
    Crash --> [*]
</pre>
</div>

---

## Class

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
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
</pre>
</div>

---

## Seq

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
    sequenceDiagram
    Alice->>John: Hello John, how are you?
    John-->>Alice: Great!
    Alice-)John: See you later!
</pre>
</div>

---

## Flow

<div class="mermaid">
<pre>
    %%{init: {'theme': 'dark', 'themeVariables': { 'darkMode': true }}}%%
    flowchart TD
    A[Start] --> B{Is it?};
    B -- Yes --> C[OK];
    C --> D[Rethink];
    D --> B;
    B -- No ----> E[End];
</pre>
</div>