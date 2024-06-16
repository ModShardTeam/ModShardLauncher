# ModShardLoader API

---
<!-- TEMPLATE -->
<!-- ??? info "AddObject `method`"
    ### Summary
    Adds a gameobject to the game.
    ### Example
    ```c#
    Msl.AddObject("o_myobject")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the gameobject to create. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleGameObject` | The gameobject created. | -->

!!! warning "WIP"
    The API documentation is a work in progress and not automated. </br>
    It may become out of date, or change drastically. </br> </br>
    Not all methods are listed here, as some are very niche or shouldn't really be used when making mods.
    To get the latest, up-to-date documentation, read the XAML docstrings in MSL's source code.

    <h3>**Last Update : MSL v0.11.1.0**</h3>

## Objects

??? info "AddObject `method`"
    <h3>Summary</h3>
    Adds a gameobject to the game.
    <h3>Example</h3>
    ```c#
    UndertaleGameObject npcTrainer = Msl.AddObject(
            name:"o_npc_trainer",
            spriteName:"s_npc_merc_inn_fight",
            parentName:"o_npc_baker",
            isVisible:true,
            isAwake:true,
            collisionShapeFlags:CollisionShapeFlags.Circle
    );
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the gameobject to create. |
    | `String` | `spriteName` | The name of the gameobject to create. |
    | `String` | `parentName` | The name of the gameobject to create. |
    | `String` | `isVisible` | The name of the gameobject to create. |
    | `String` | `isAwake` | The name of the gameobject to create. |
    | `String` | `collisionShapeFlags` | The name of the gameobject to create. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleGameObject` | The gameobject created. |

??? info "GetObject `method`"
    <h3>Summary</h3>
    Gets a gameobject from the game files.
    <h3>Example</h3>
    ```c#
    Msl.GetObject("o_myobject")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description** |
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the gameobject to get.

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleGameObject` | The gameobject if found. |

??? info "SetObject `method`"
    <h3>Summary</h3>
    Replaces a gameobject from the game with your own.
    <h3>Example</h3>
    ```c#
    Msl.SetObject("o_myobject", myObject)
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description** |
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the gameobject to replace. |
    | `UndertaleGameObject` | `o` | The gameobject to replace it with. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A. |

---

## Events

??? example "AddNewEvent `method`"
    <h3>Summary</h3>
    Adds a new event to a gameobject.
    <h3>Examples</h3>
    === "By object name"
        ```c#
        // Example 1 : Create Event from string, with object name
        Msl.AddNewEvent("o_myobject", "scr_actionsLogUpdate(\"hello world\")", EventType.Create, 0);

        // Example 2 : Other Event from file, with object name
        Msl.AddNewEvent("o_anotherobject", ModFiles.GetCode("myScript.gml"), EventType.Other, 24);
        ```
    
    === "By object as a variable"
        ```c#
        // Example 3 : Create Event from string, with object as a variable
        Msl.AddNewEvent(myVarContainingAGameObject, "scr_actionsLogUpdate(\"hello world\")", EventType.Create, 0);
        ```
    <h3>Arguments</h3>
    === "By object name"
        | **Type** | **Name** | **Description** |
        | :---: | :---: | :--- |
        | `String` | `objectName` | The name of the gameobject to add the event to. |
        | `String` | `eventCode` | The code for the event you're adding. |
        | `Msl.EventType` | `eventType` | The type of event to add. |
        | `uint` | `subtype` | The subtype of the event to add. </br>(Some events have different sub-events, like `Other` or `Draw`) |
    === "By object as a variable"
        | **Type** | **Name** | **Description** |
        | :---: | :---: | :--- |
        | `UndertaleGameObject` | `gameObject` | Reference to the gameobject to add the event to. |
        | `String` | `eventCode` | The code for the event you're adding. |
        | `Msl.EventType` | `eventType` | The type of event to add. |
        | `uint` | `subtype` | The subtype of the event to add. </br>(Some events have different sub-events, like `Other` or `Draw`) |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

    <h3>Exception</h3>
    `ArgumentException` thown if the event already exists.

??? example "ApplyEvent `method`"
    _TODO_
---

## Sprites

??? info "GetSprite `method`"
    <h3>Summary</h3>
    Gets a sprite from the game files.
    <h3>Example</h3>
    ```c#
    Msl.GetSprite("s_mySprite")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the sprite to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleSprite` | The sprite if found. |

---

## Tables

??? info "GetTable `method`"
    <h3>Summary</h3>
    Gets a table from the game's files.
    <h3>Example</h3>
    ```c#
    Msl.GetTable("gml_GlobalScript_table_Miniboss_type")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the table to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `List<string>?` | The table's content as a list of strings. |

??? info "SetTable `method`"
    <h3>Summary</h3>
    Replaces a table from the game with your own.
    <h3>Example</h3>
    ```c#
    Msl.SetTable("gml_GlobalScript_table_Miniboss_type")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `List<string>` | `table` | The table to replace the game's table with. |
    | `String` | `name` | The name of the table to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? example "InjectTableX `method`"
    <h3>Summary</h3>
    Injects your data into an existing table from the vanilla game. </br>
    The various fields depend on the specific method, and your IDE should let you know what it needs. </br> </br>
    Here's a list of the available methods :

    - `InjectTableConsumParam`
    - `InjectTableContract`
    - `InjectTableCreditsBackers`
    - `InjectTableEnemyBalance`
    - `InjectTableLocalizationUtils`
    - `InjectTablePotion`
    - `InjectTableSkillsStat`
    - `InjectTableArmor`
    - `InjectTableWeapons`

    <h3>Examples</h3>
    ```c#
    // Example 1 (EnemyBalance)
    Msl.InjectTableEnemyBalance("Enemy1", 1, "ID1", Msl.EnemyBalanceType.undead, Msl.EnemyBalanceFaction.Undead, Msl.EnemyBalancePattern.Melee, Msl.EnemyBalanceSpawnType1.Fighter, Msl.EnemyBalanceWeapon.sword, Msl.EnemyBalanceArmor.Light, Msl.EnemyBalanceMatter.bones, 1);
    
    // Example 2 (SkillsStat)
    Msl.InjectTableSkillsStat(Msl.SkillsStatMetaGroup.BEASTS, "wild_shape", "o_wild_shape", Msl.SkillsStatTarget.NoTarget, "0", 30, 20, 0, 0, 0, 0, false, Msl.SkillsStatPattern.normal, Msl.SkillsStatClass.spell, true, "", Msl.SkillsStatBranch.none, false, true, Msl.SkillsStatMetacategory.none, 0, "", false, false, false, false, true);
    ```

---

## Code

??? info "AddCode `method`"
    <h3>Summary</h3>
    Adds a code to the game.
    <h3>Example</h3>
    ```c#
    Msl.AddCode("scr_actionsLogUpdate(\"Hello World !\")", "myCode")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The gml code as a string. |
    | `String` | `name` | The name of the code to create. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleCode` | The code created. |

??? info "GetUMTCodeFromFile `method`"
    <h3>Summary</h3>
    Gets a code from the game files.
    <h3>Example</h3>
    ```c#
    Msl.GetUMTCodeFromFile("gml_GlobalScript_scr_sessionDataInit")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `fileName` | The name of the code to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleCode` | The code if found. |

??? info "GetStringGMLFromFile `method`"
    <h3>Summary</h3>
    Gets the content of a code from the game files as a string of GML.
    <h3>Example</h3>
    ```c#
    Msl.GetStringGMLFromFile("gml_GlobalScript_scr_sessionDataInit")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `fileName` | The name of the code to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `String` | The content of the code if found. |

??? info "SetStringGMLInFile `method`"
    <h3>Summary</h3>
    Replaces the file's code with a string of GML.
    <h3>Example</h3>
    ```c#
    SetStringGMLInFile("scr_actionsLogUpdate(\"hello world\")", "gml_Object_o_player_KeyPress_116");
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The string of GML code to insert. |
    | `String` | `fileName` | The name of the code to replace. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? info "ReplaceGMLString `method`"
    <h3>Summary</h3>
    Replaces a code in the game with a string of GML.
    <h3>Example</h3>
    ```c#
    Msl.ReplaceGMLString("gml_GlobalScript_scr_sessionDataInit")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The string of GML code to insert. |
    | `String` | `fileName` | The name of the code to replace a line. |
    | `Int` | `position` | The line to replace with the provided GML. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? info "InsertGMLString `method`"
    <h3>Summary</h3>
    Inserts a string of GML into a file at a specific position.
    <h3>Example</h3>
    ```c#
    Msl.InsertGMLString("scr_actionsLogUpdate(\"Hello World !\")", "gml_GlobalScript_scr_sessionDataInit", 14)
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The string of GML code to insert. |
    | `String` | `fileName` | The name of the code to insert the GML into. |
    | `Int` | `position` | The line below which we inject the GML code. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

---

## Assembly

??? info "GetAssemblyString `legacy method`"
    _TODO_

??? info "SetAssemblyString `legacy method`"
    _TODO_

??? info "InsertAssemblyString `legacy method`"
    <h3>Summary</h3>
    Inserts an assembly instruction below the provided line.
    <h3>Example</h3>
    ```c#
    // Example : Inserts the instruction 'pushi.e 1' in 'gml_GlobalScript_scr_sessionDataInit' below line 14.
    Msl.InsertAssemblyString("pushi.e 1", "gml_GlobalScript_scr_sessionDataInit", 14)
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The string of assembly instruction(s) to insert. |
    | `String` | `fileName` | The name of the code to insert the assembly into |
    | `Int` | `position` | The line below which we inject the assembly instruction(s) |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? info "ReplaceAssemblyString `legacy method`"
    <h3>Summary</h3>
    Replaces the provided line with an assembly instruction given as string.
    <h3>Example</h3>
    === "Replace"
        ```c#
        // Example : Replaces the 8th line in 'gml_GlobalScript_scr_sessionDataInit' with 'pushi.e 1'.
        Msl.ReplaceAssemblyString("pushi.e 1", "gml_GlobalScript_scr_sessionDataInit", 8)
        ```
    === "Replace and Erase"
        ```c#
        // Example : Replaces the 8th line in 'gml_GlobalScript_scr_sessionDataInit' with 'pushi.e 1' and erases the 3 lines below.
        Msl.ReplaceAssemblyString("pushi.e 1", "gml_GlobalScript_scr_sessionDataInit", 8, 3)
        ```
    <h3>Arguments</h3>
    === "Replace"
        | **Type** | **Name** | **Description**|
        | :---: | :---: | :--- |
        | `String` | `codeAsString` | The string of assembly instruction(s) to insert. |
        | `String` | `fileName` | The name of the code to insert the assembly into. |
        | `Int` | `position` | The line to be replaced. |
    === "Replace and Erase"
        | **Type** | **Name** | **Description**|
        | :---: | :---: | :--- |
        | `String` | `codeAsString` | The string of assembly instruction(s) to insert. |
        | `String` | `fileName` | The name of the code to insert the assembly into. |
        | `Int` | `start` | The line to be replaced. |
        | `Int` | `len` | The number of lines to be erased after the replacement. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? info "InjectAssemblyInstruction `legacy method`"
    _TODO_
---

## Functions / Strings / Variables

??? info "AddFunction `method`"
    <h3>Summary</h3>
    Adds a function to the game.
    <h3>Example</h3>
    ```c#
    Msl.AddFunction("scr_actionsLogUpdate(\"Hello World !\")", "myFunction")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The gml code to put in the function as a string. |
    | `String` | `name` | The name of the function to create. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleCode` | The code created for the function. |

??? info "GetVariable `method`"
    <h3>Summary</h3>
    Gets a variable from the game files.
    <h3>Example</h3>
    ```c#
    Msl.GetVariable("display_x")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the variable to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleVariable` | The variable if found. |

??? info "GetString `method`"
    <h3>Summary</h3>
    Gets a variable from the game files.
    <h3>Example</h3>
    ```c#
    Msl.GetString("questBreweryOdarAccept00")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the string to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleString` | The string if found. |

---

## Settings

??? example "UIComponent `type`"
    <h3>Summary</h3>
    Hold data needed to create an entry in the in-game mod menu.
    <h3>Example</h3>

    === "Slider"

        ```c#
        // Example : Creates a slider that changes the value held by a global variable named menu_slider in gml.
        // The slider will have a range from 2 to 5, with a default value of 4.
        UIComponent sliderEntry = new(name: "This is a slider", associatedGlobal: "menu_slider", UIComponentType.Slider, (2, 5), 4)
        ```

    === "ComboBox (Drop Down List)"

        ```c#
        // Example : Creates a drop down list that changes the value held by a global variable named menu_combo in gml.
        // The drop down list will display only two elements, Option_1 and Option_2.
        UIComponent comboEntry = new(name: "This is a drop down list", associatedGlobal: "menu_combo", UIComponentType.ComboBox, new string[] { "Option_1", "Option_2",})
        ```

    === "CheckBox"

        ```c#
        // Example : Creates a check box that changes the value held by a global variable named menu_check in gml.
        // The check box will have the value 0 if not slected, 10 else.
        UIComponent checkEntry = new(name: "This is a check box", associatedGlobal: "menu_check", UIComponentType.CheckBox, 10)
        ```

    <h3>Arguments</h3>

    === "Slider"

        | **Type** | **Name** | **Description**|
        | :---: | :---: | :--- |
        | `String` | `name` | The displayed name in the menu. |
        | `String` | `associatedGlobal` | The name of the global variable modified by this component. In the case of a slider, the global variable will hold a real value. |
        | `UIComponentType` | `componentType` | The type of the component. Expected to be a `UIComponentType.Slider`. |
        | `(int, int)` | `sliderValues` | Min and Max values for a slider, floating-point values are not supported. |
        | `int` | `defaultValue` | Default value of the component. |
        | `bool` | `onlyInMainMenu` | If true, this entry will only appear in the main menu of the game. False by default. |

    === "ComboBox (Drop Down List)"

        | **Type** | **Name** | **Description**|
        | :---: | :---: | :--- |
        | `String` | `name` | The displayed name in the menu. |
        | `String` | `associatedGlobal` | The name of the global variable modified by this component. In the case of a combobox, the global variable will hold a string. |
        | `UIComponentType` | `componentType` | The type of the component. Expected to be a `UIComponentType.ComboBox`. |
        | `string[]` | `dropDownValues` | List of all possible values held by the global variable. |
        | `bool` | `onlyInMainMenu` | If true, this entry will only appear in the main menu of the game. False by default. |
        
    === "CheckBox"

        | **Type** | **Name** | **Description**|
        | :---: | :---: | :--- |
        | `String` | `name` | The displayed name in the menu. |
        | `String` | `associatedGlobal` | The name of the global variable modified by this component. In the case of a combobox, the global variable will hold either 0 or 1. |
        | `UIComponentType` | `componentType` | The type of the component. Expected to be a `UIComponentType.CheckBox`. |
        | `int` | `defaultValue` | Default value of the component. |
        | `bool` | `onlyInMainMenu` | If true, this entry will only appear in the main menu of the game. False by default. |


    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UIComponent` | A ui component |

??? example "AddMenu `method`"
    <h3>Summary</h3>
    Adds entries to the in-game mod menu, which lets users configure settings at runtime.
    <h3>Example</h3>
    ```c#
    // Example : Adds a slider to the mod menu to change a value hold by a global named menu_test in gml.
    // The slider will have a range from 2 to 5, with a default value of 4.
    Msl.AddMenu("MyMod", new UIComponent(name: "The displayed name in the menu", associatedGlobal: "menu_test", UIComponentType.Slider, (2, 5), 4))
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the category in which to place the new entry. |
    | `UIComponent[]` | `components` | The component(s) to add to the mod menu. Can be a `CheckBox`, `ComboBox` or `Slider`. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? example "AddCreditDisclaimerRoom `method`"
    <h3>Summary</h3>
    Adds your mod's name and authors to MSL's credits room, which is shown before the main menu.
    <h3>Example</h3>
    ```c#
    AddCreditDisclaimerRoom("MyMod", "Myself", "Someone else", "Another person", "etc...")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `modName` | The name under which to show your mod in the credits room. |
    | `String[]` | `authors` | The authors of the mod. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? example "AddCustomDisclaimerRoom `method`"
    <h3>Summary</h3>
    Adds a custom disclaimer room to the game, which is shown after the credit room but before the main menu.
    <h3>Example</h3>
    ```c#
    Msl.AddCustomDisclaimer("r_myDisclaimerRoom", myOverlay)
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `roomName` | The name of the room to create. |
    | `UndertaleRoom.GameObject` | `overlay` | The gameobject to show in the room. This should act as an overlay and contain all you want to display. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

---

## Loot

??? example "AddLootTable `method`"
    <h3>Summary</h3>
    Creates a new loot table, which is a weighted list of gameobjects that can be found/dropped.
    <h3>Example</h3>
    ```c#
    // Creates a new loot table with guaranteed items and items that have various chances of dropping.
    // Also handles empty drops and items with rarity / durability.
    Msl.AddLootTable(
        lootTableID: "bookshelf",
        guaranteedItems: new ItemsTable(
            listItems: new string[] { "copper_candelabrum", "scroll_disenchant"},
            listRarity: new int[] { -1, -1},
            listDurability: new int[] { -1, -1}
        ),
        randomLootMin: 1,
        randomLootMax: 2,
        emptyWeight: 100,
        randomItemsTable: new RandomItemsTable(
            listItems: new string[] { "oil", "bottle", "thread", "Joust Cape"},
            listRarity: new int[] { -1, -1, -1, 6},
            listDurability: new int[] { -1, -1, -1, 10 },
            listWeight: new int[] { 20, 1, 1, 100 }
        )
    );
    ```
    <h3>Arguments</h3>
    
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `lootTableID` | The name of the loot table to create. |
    | `ItemsTable` | `guaranteedItems` | The items that are guaranteed to drop. |
    | `int` | `randomLootMin` | The minimum amount of random items to drop. |
    | `int` | `randomLootMax` | The maximum amount of random items to drop. |
    | `int` | `emptyWeight` | The weight of the empty drop. |
    | `RandomItemsTable` | `randomItemsTable` | The items that can randomly drop, if any. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? example "AddReferenceTable `method`"
    <h3>Summary</h3>
    Assigns a gameobject to a specific loot table.</br>
    This means that any and all instances of the gameobject will drop items from the specified loot table.
    <h3>Examples</h3>
    ```c#
    // Example 1: Assigns all `o_bandit_goon_club` instances to the `barrelsSpecial` loot table.
    Msl.AddReferenceTable(nameObject:"o_bandit_goon_club", table:"barrelsSpecial");


    // Example 2: Assigns all `o_bandit_goon_club` to the `barrelsSpecial` loot table
    // except the instances with ids 2334 and 2335 which are assigned to the `goon` and `goon2` tables.
    Msl.AddReferenceTable(
        nameObject:"o_bandit_goon_club", 
        table:"barrelsSpecial", 
        ids: new ids[] { {id: 2334, table:"goon"}, {id: 2335, table:"goon2"} }
    );


    // Example 3: Assigns all `o_bandit_goon_club` to the `barrelsSpecial` loot table by default.
    // If a tier can be computed (such as in a donjon), it will follow the rules below:
    // - Tiers 1-3 will be assigned to the default table.
    // - Tiers 4-6 will be assigned to the `goon` table.
    // - Tiers 7+ will be assigned to the `goon2` table.
    Msl.AddReferenceTable(
        nameObject:"o_bandit_goon_club", 
        table:"barrelsSpecial", 
        tiers: new tiers[] { {tier: 4, table:"goon"}, {tier: 7, table:"goon2"} }
    );


    // Example 4: Same as above, except ids 2334 and 2335 ignore the tier rules and are assigned to `goon3` and `goon4` respectively.
    Msl.AddReferenceTable(
        nameObject:"o_bandit_goon_club", 
        table:"barrelsSpecial", 
        tiers: new tiers[] { {tier: 4, table:"goon3"}, {id: 7, table:"goon4"} }, 
        ids: new ids[] { {id: 2334, table:"goon"}, {id: 2335, table:"goon2"} }
    );
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `nameObject` | The name of the gameobject to assign to a loot table. |
    | `String` | `table` | The name of the loot table to assign the gameobject to. |
    | `Dict<int, string>` | `ids` | Overrides for specific ids. |
    | `Dict<int, string>` | `tiers` | Overrides for specific tiers. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? example "AddReferenceTableForMultipleObjects `method`"
    <h3>Summary</h3>
    Assigns multiple gameobjects to a loot table.</br>
    This means that any and all instances of these gameobjects will drop items from the specified loot table.
    <h3>Example</h3>
    ```c#
    // Assigns all instances of `o_bandit_goon_club` and `o_bandit_goon_cleaver` to the `barrelsSpecial` loot table.
    Msl.AddReferenceTableForMultipleObjects(table: "barrelsSpecial", "o_bandit_goon_club", "o_bandit_goon_cleaver");
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `table` | The name of the loot table to assign the gameobjects to. |
    | `String[]` | `nameObjects` | The names of the gameobjects to assign to the loot table. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

---

## Rooms

??? example "GetRoom `method`"
    <h3>Summary</h3>
    Gets a room from the game files by name.
    <h3>Example</h3>
    ```c#
    Msl.GetRoom("r_myRoom")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the room to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleRoom` | The room if found. |

??? example "GetRooms `method`"
    <h3>Summary</h3>
    Gets a list of all the rooms in the game.
    <h3>Example</h3>
    ```c#
    Msl.GetRooms()
    ```
    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `IEnumerable<UndertaleRoom>` | An iterable of all the rooms in the game. |

??? example "AddRoom `method`"
    <h3>Summary</h3>
    Adds a room to the game.
    <h3>Examples</h3>
    ```c#
    // Example 1 : Adds a room named 'r_myRoom'
    Msl.AddRoom("r_myRoom")

    // Example 2 : Adds a room named 'r_myRoom' with a width of 5 and a height of 10
    Msl.AddRoom("r_myRoom", 5, 10)
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the room to create. |
    | `int` | `width` | The width of the room. |
    | `int` | `height` | The height of the room. |
    

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleRoom` | The room created. |

??? example "GetLayer `method`"
    <h3>Summary</h3>
    Gets a layer from a room.
    <h3>Example</h3>
    ```c#
    // Example : Gets the tiles layer named 'myLayer' from the room 'r_myRoom'
    var layer = Msl.GetRoom("r_someRoom").GetLayer(UndertaleRoom.LayerType.Tiles, "myLayer")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `UndertaleRoom` | `room` | The room to get the layer from. |
    | `UndertaleRoom.LayerType` | `type` | The type of the layer to get. |
    | `String` | `name` | The name of the layer to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleLayer` | The layer found. |

??? example "AddLayer `method`"
    <h3>Summary</h3>
    Adds a layer to a room.
    <h3>Example</h3>
    ```c#
    // Example : Adds a layer named 'myLayer' to the room 'r_myRoom'
    var room = Msl.AddRoom("r_myRoom")
    room.AddLayer(UndertaleRoom.LayerType.Tiles, "myLayer")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `UndertaleRoom` | `room` | The name of the room to add the layer to. |
    | `UndertaleRoom.LayerType` | `type` | The name of the layer to create. |
    | `String` | `name` | The name of the layer to create. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleLayer` | The layer created. |

??? example "AddGameObject `method`"
    <h3>Summary</h3>
    Adds a gameobject to a room.</br>
    Layer and Object name can be supplied as objects or strings.
    <h3>Example</h3>
    ```c#
    // Example : Adds a gameobject named 'o_myObject' to the room 'r_myRoom'
    // with a creation code at position (5, 10) on the layer 'instanceLayer'
    var creationCode = Msl.GetUMTCodeFromFile("someScriptFromTheGame")
    var room = Msl.GetRoom("r_someRoom")
    room.AddGameObject(
        layerName: "instanceLayer",
        obName: "o_myObject",
        creationCode: creationCode,
        x: 5,
        y: 10
    )
    ```

??? example "GetGameObject `method`"
    <h3>Summary</h3>
    Gets a gameobject from a room.
    <h3>Example</h3>
    ```c#
    // Example : Gets the gameobject named 'o_myObject' in the layer 'instanceLayerName' from the room 'r_myRoom'
    var room = Msl.GetRoom("r_someRoom")
    room.GetGameObject("instanceLayerName", "o_myObject")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `layerName` | The name of the layer to get the gameobject from. |
    | `String` | `obName` | The name of the gameobject to get. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleGameObject` | The gameobject found. |

---

## Patching
### Stage 1 (Loading)

The 'Loading' stage is used to get the GML or assembly from the game's files.

??? info "LoadGML `method`"
    <h3>Summary</h3>
    The first step to inject GML. </br>
    Loads an existing code from the game files and returns it.
    <h3>Example</h3>
    ```c#
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `fileName` | The name of the code to get from the game files. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | A class containing the loaded GML file, used in Stage 2 methods. |

??? info "LoadAssemblyAsString `method`"
    <h3>Summary</h3>
    The first step to inject Assembly. </br>
    Loads the assembly corresponding to an existing code from the game files and returns it.
    <h3>Example</h3>
    ```c#
    Msl.LoadAssemblyAsString("gml_GlobalScript_scr_sessionDataInit")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `fileName` | The name of the code to get the assembly from in the game files. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | A class containing the assembly for the loaded code, used in Stage 2 methods. |


### Stage 2 (Matching)

The 'Matching' stage is used to select a line, or multiple lines that we're going to act upon in stage 3.
It is done by providing code to find in the file, or matching every line.

??? info "MatchFrom `method`"
    <h3>Summary</h3>
    The second step to inject GML or Assembly. </br>
    Finds the code in the previously opened file and selects every matching line.

    !!! tip "Signatures"
        This method has multiple signatures. </br>
        This documentation only covers the `FileEnumerable + String` signature (*see Example 1*).

        === "Main"

            ```c#
            public static IEnumerable<(Match, string)> MatchFrom(this IEnumerable<string> ienumerable, IEnumerable<string> other)
            ```

        === "String other"

            ```c#
            public static IEnumerable<(Match, string)> MatchFrom(this IEnumerable<string> ienumerable, string other) 
            ```

        === "FileEnumerable + String"

            ```c#
            public static FileEnumerable<(Match, string)> MatchFrom(this FileEnumerable<string> fe, string other) 
            ```
        
        === "FileEnumerable + File"

            ```c#
            public static FileEnumerable<(Match, string)> MatchFrom(this FileEnumerable<string> fe, ModFile modFile, string fileName)
            ```

    <h3>Examples</h3>
    ```c#
    // Example 1 : Selecting a matching String in a GML file
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("if (!instance_exists(o_music_controller))")

    // Example 2 : Selecting the matching content of a file in a GML file
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom(ModFiles, "my_matching_file.gml")

    // Example 3 : Selecting a matching assembly instruction String in an assembly file
    Msl.LoadAssemblyAsString("gml_GlobalScript_scr_sessionDataInit").MatchFrom("pushi.e 1")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<string>` | `fe` | The previously opened file |
    | `String` | `other` | The code to find and match with the file |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<(Match, string)>` | A class that contains the loaded code with selected parts defined, ready for Stage 3. |

??? info "MatchBelow `method`"
    <h3>Summary</h3>
    The second step to inject GML or Assembly. </br>
    Finds the passed string/file in the previously opened file and selects the `N` lines below the matching part.

    !!! tip "Signatures"
        This method has multiple signatures. </br>
        This documentation only covers the `FileEnumerable + String` signature (*see Example 1*).

        === "Main"

            ```c#
            public static IEnumerable<(Match, string)> MatchBelow(this IEnumerable<string> ienumerable, IEnumerable<string> other, int len)
            ```

        === "String other"

            ```c#
            public static IEnumerable<(Match, string)> MatchBelow(this IEnumerable<string> ienumerable, string other, int len) 
            ```

        === "FileEnumerable + String"

            ```c#
            public static FileEnumerable<(Match, string)> MatchBelow(this FileEnumerable<string> fe, string other, int len) 
            ```
        
        === "FileEnumerable + File"

            ```c#
            public static FileEnumerable<(Match, string)> MatchBelow(this FileEnumerable<string> fe, ModFile modFile, string fileName, int len) 
            ```


    <h3>Example</h3>
    ```c#
    // Example 1 : Selecting 1 line below a matching String in a GML file
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchBelow("if (!instance_exists(o_music_controller))", 1)

    // Example 2 : Selecting the 4 lines below a matching file in a GML file
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchBelow(ModFiles, "my_matching_file.gml", 4)

    // Example 3 : Selecting 1 line below a matching assembly instruction String in an assembly file
    Msl.LoadAssemblyAsString("gml_GlobalScript_scr_sessionDataInit").MatchBelow("pushi.e 1", 1)
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<string>` | `fe` | The previously opened file |
    | `String` | `other` |The code to match with the file |
    | `Int` | `len` | The amount of lines below the match to select. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<(Match, string)>` | A class that contains the loaded code with selected parts defined, ready for Stage 3. |

??? info "MatchAll `method`"
    <h3>Summary</h3>
    The second step to inject GML or Assembly. </br>
    Selects all lines in the previously opened file.

    !!! tip "Signatures"
        This method has multiple signatures. </br>
        This documentation only covers the `FileEnumerable` signature.

        === "Main"

            ```c#
            public static IEnumerable<(Match, string)> MatchAll(this IEnumerable<string> ienumerable)
            ```

        === "FileEnumerable"

            ```c#
            public static FileEnumerable<(Match, string)> MatchAll(this FileEnumerable<string> fe) 
            ```

    <h3>Example</h3>
    ```c#
    // Example : Selecting every line in a GML file
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchAll()
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<string>` | `fe` | The previously opened file. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<(Match, string)>` | A class that contains the loaded code with selected parts defined, ready for Stage 3. |

??? info "MatchFromUntil `method`"
    <h3>Summary</h3>
    The second step to inject GML or Assembly. </br>
    Selects all lines from the first match until the second match (included).

    !!! tip "Signatures"
        This method has multiple signatures. </br>
        This documentation only covers the `FileEnumerable` signature.

        === "Main"

            ```c#
            public static IEnumerable<(Match, string)> MatchFromUntil(this IEnumerable<string> ienumerable, IEnumerable<string> otherfrom, IEnumerable<string> otheruntil)
            ```

        === "Strings"

            ```c#
            public static FileEnumerable<(Match, string)> MatchFromUntil(this IEnumerable<string> ienumerable, string otherfrom, string otheruntil) 
            ```
        
        === "FileEnumerable"

            ```c#
            public static FileEnumerable<(Match, string)> MatchFromUntil(this FileEnumerable<string> fe, string otherfrom, string otheruntil)
            ```
        
        === "Mod Files"

            ```c#
            public static FileEnumerable<(Match, string)> MatchFromUntil(this FileEnumerable<string> fe, ModFile modFile, string filenameOther, string filenameUntil)
            ```

    <h3>Example</h3>
    ```c#
    // Example 1 : Selecting all lines from the line containing "scr_locationPositionInit()" until the line containing "if (!instance_exists(o_music_controller))".
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFromUntil("scr_locationPositionInit()", "if (!instance_exists(o_music_controller))")

    // Example 2 : Selecting all lines from the line contained in "firstMatchingLine.gml" until the line contained in "otherMatchingLine.gml".
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom(ModFiles, "firstMatchingLine.gml", "otherMatchingLine.gml")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<string>` | `fe` | The previously opened file. |
    | `String` | `otherfrom` | The beginning of the code to match |
    | `String` | `otheruntil` | The end of the code to match |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<(Match, string)>` | A class that contains the loaded code with selected parts defined, ready for Stage 3. |


### Stage 3 (Acting)

The 'Acting' stage is used to perform an action on the selected lines. </br>
This can be inserting, replacing, deleting or others, which lead to modification of the original file.

??? info "Remove `method`"
    <h3>Summary</h3>
    The third step to inject GML / assembly. </br>
    Removes previously selected lines from opened file.
    <h3>Example</h3>
    ```c#
    // Example 1 : Remove all lines from script gml_GlobalScript_scr_sessionDataInit
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchAll().Remove()

    // Example 2 : Remove the `pushi.e 1` instruction from gml_Object_c_bed_sleep_crafted_Alarm_0's assembly
    Msl.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").Remove()
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "KeepOnly `method`"
    <h3>Summary</h3>
    The third step to inject GML / assembly. </br>
    Removes all non-selected lines from opened file.
    <h3>Example</h3>
    ```c#
    // Example 1 : Remove all lines from script gml_GlobalScript_scr_sessionDataInit except the line where 'global.HP = -1' appears.
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").KeepOnly()

    // Example 2 : Remove all instructions from gml_Object_c_bed_sleep_crafted_Alarm_0's assembly except `pushi.e 1`
    Msl.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").KeepOnly()
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "InsertAbove `method`"
    <h3>Summary</h3>
    The third step to inject GML / assembly. </br>
    Inserts a string/file content above the selected line.
    <h3>Example</h3>
    ```c#
    // Example 1 : Adds 'global.myVar = 14' above the line containing 'global.HP = -1'
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").InsertAbove("global.myVar = 14")

    // Example 2 : Adds the content of my_gml_code.gml above the line containing 'global.HP = -1
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").InsertAbove(ModFiles, "my_gml_code.gml")

    // Example 3 : Adds the instruction 'popz.v' above the line containing the instruction 'pushi.e 1'
    Msl.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").InsertAbove("popz.v")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |
    | 'String' | `inserting` | The code to insert above the selected line.

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "InsertBelow `method`"
    <h3>Summary</h3>
    The third step to inject GML / assembly. </br>
    Inserts a string/file content below the selected line.
    <h3>Example</h3>
    ```c#
    // Example 1 : Adds 'global.myVar = 14' below the line containing 'global.HP = -1'
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").InsertBelow("global.myVar = 14")

    // Example 2 : Adds the content of my_gml_code.gml below the line containing 'global.HP = -1
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").InsertBelow(ModFiles, "my_gml_code.gml")

    // Example 3 : Adds the instruction 'popz.v' below the line containing the instruction 'pushi.e 1'
    Msl.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").InsertBelow("popz.v")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |
    | 'String' | `inserting` | The code to insert below the selected line. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "ReplaceBy `method`"
    <h3>Summary</h3>
    The third step to inject GML / assembly. </br>
    Replaces the selected line with a string/file content.
    <h3>Example</h3>
    ```c#
    // Example 1 : Replaces the line containing 'global.HP = -1' with 'global.HP = 50'
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").ReplaceBy("global.HP = 50")

    // Example 2 : Replaces the line containing 'global.HP = -1 with the content of 'my_gml_code.gml'
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").ReplaceBy(ModFiles, "my_gml_code.gml")

    // Example 3 : Replaces the line containing the instruction 'pushi.e 1' with the instruction 'popz.v'
    Msl.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").ReplaceBy("popz.v")
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |
    | 'String' | `inserting` | The code to replace the selected line with. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "FilterMatch `method`"
    <h3>Summary</h3>
    The third step to inject GML / assembly. </br>
    TODO
    <h3>Example</h3>
    ```c#
    TODO
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |


### Stage 4 (Saving)

The 'Saving' stage saves the patched difference into the original file. </br>
Failing to add this at the end of your chain of calls will render it useless.

??? info "Save `method`"
    <h3>Summary</h3>
    The final step to inject GML / assembly. </br>
    Saves the modified content to the original file.
    <h3>Example</h3>
    ```c#
    // Example : Loads a script, selects the line containing 'global.HP = -1', replaces it with 'global.HP = 50' and saves it.
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").ReplaceBy("global.HP = 50").Save()
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |

    <h3>Returns</h3>

    | **Type** | **Description** |
    | :--- | :--- |
    | `ModSummary` | The file with the result of the action. |


### Utility

Utility functions can be called at any time and don't impact the file in any way.</br>
They are mostly used to print information or debug.

??? info "Peek `method`"
    <h3>Summary</h3>
    Can be used at any stage before saving to print a log in MSL's console containing the current state of the file. </br>
    This method doesn't modify the input at all, simply prints it and passes it forward to the next method. </br>
    It can also be used multiple times in a single chain of calls.
    <h3>Example</h3>
    ```c#
    // Example : Prints the state of the file before and after replacing.
    Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit").Peek().MatchFrom("global.HP = -1").ReplaceBy("global.HP = 50").Peek().Save()
    ```
    <h3>Arguments</h3>

    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<T>` | `fe` | The previously opened file. |

    <h3>Returns</h3>
    
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<T>` | The file, exactly as it was when passed as an argument. |
