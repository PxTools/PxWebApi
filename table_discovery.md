On caching: Say one user asks for B and before B is created and put in cache another user asks for B. How to avoid that this causes 2 backend requests?

Issue: How to have Menu.xml and The index ready at 8:00:00,00?

Drawing of things for the menu as is-ish
```mermaid
---
title: Getting to a table
---
flowchart TB
    a1-->d1
    a1-->d4
    a2-->b2
    a3-->b3
    b2---d3
    b2---d2
    b2---d5    

    subgraph one[Controllers]
    direction LR
    a1["GetFolder(id,lang)"\nFolder is a PxMenuItem + config]
    a3["GetTables(id?,lang)\nIs a Search"]
    subgraph admin
    a2["write Search index"] 
    a4["write menu.xml\n(PX only)"]
    end
    end

    subgraph two["Search index"]
      b2["Build index"]
      b1["index"]
      b3["DoSearch"]
    end
    
    subgraph sg5["Data source"]
        subgraph non_paxiom
        d1["List of folderIds"]
        d2["List of tableIds"]
        d4["CreateMenu(id, lang) \n 'get PxMenuItem' \n PCAxis.Menu nuget \n differs cnmm vs Menu.xml"]
        d5["CreateMenuTableLink \n 'get TableLink'"]
        end

        d3["'get paxiom'"]
    end

    subgraph sg6["PX"]
        px["Menu.xml"]
    end

    subgraph sg16["Data storage"]
    sg16_21["Filesystem"]
    sg16_22["CNMM DB"]
    end
    a4-->sg6
    non_paxiom-->sg6
    non_paxiom--CNMM, has no Menu.xml-->sg16_22
    sg6-->sg16_21
    d3-->sg16
