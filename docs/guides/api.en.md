# ModShardLoader API

---
<!-- TEMPLATE -->
<!-- ??? info "AddObject `method`"
    ### Summary
    Adds a gameobject to the game.
    ### Example
    ```c#
    ModLoader.AddObject("o_myobject")
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
    To get the latest, up to date documentation, read the XAML docstrings in MSL's source code.

## Objects

??? info "AddObject `method`"
    ### Summary
    Adds a gameobject to the game.
    ### Example
    ```c#
    ModLoader.AddObject("o_myobject")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the gameobject to create. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleGameObject` | The gameobject created. |

??? info "GetObject `method`"
    ### Summary
    Gets a gameobject from the game files.
    ### Example
    ```c#
    ModLoader.GetObject("o_myobject")
    ```
    ### Arguments
    | **Type** | **Name** | **Description** |
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the gameobject to get.

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleGameObject` | The gameobject if found. |

??? info "SetObject `method`"
    ### Summary
    Replaces a gameobject from the game with your own.
    ### Example
    ```c#
    ModLoader.SetObject("o_myobject", myObject)
    ```
    ### Arguments
    | **Type** | **Name** | **Description** |
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the gameobject to replace. |
    | `UndertaleGameObject` | `o` | The gameobject to replace it with. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A. |

---

## Sprites

??? info "GetSprite `method`"
    ### Summary
    Gets a sprite from the game files.
    ### Example
    ```c#
    ModLoader.GetSprite("s_mySprite")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the sprite to get. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleSprite` | The sprite if found. |

---

## Tables

??? info "GetTable `method`"
    ### Summary
    Gets a table from the game's files.
    ### Example
    ```c#
    ModLoader.GetTable("gml_GlobalScript_table_Miniboss_type")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the table to get. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `List<string>?` | The table's content as a list of strings. |

??? info "SetTable `method`"
    ### Summary
    Replaces a table from the game with your own.
    ### Example
    ```c#
    ModLoader.SetTable("gml_GlobalScript_table_Miniboss_type")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `List<string>` | `table` | The table to replace the game's table with. |
    | `String` | `name` | The name of the table to get. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

---

## Code

??? info "AddCode `method`"
    ### Summary
    Adds a code to the game.
    ### Example
    ```c#
    ModLoader.AddCode("scr_actionsLogUpdate(\"Hello World !\")", "myCode")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The gml code as a string. |
    | `String` | `name` | The name of the code to create. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleCode` | The code created. |

??? info "GetUMTCodeFromFile `method`"
    ### Summary
    Gets a code from the game files.
    ### Example
    ```c#
    ModLoader.GetUMTCodeFromFile("gml_GlobalScript_scr_sessionDataInit")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `fileName` | The name of the code to get. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleCode` | The code if found. |

??? info "GetStringGMLFromFile `method`"
    ### Summary
    Gets the content of a code from the game files as a string of GML.
    ### Example
    ```c#
    ModLoader.GetStringGMLFromFile("gml_GlobalScript_scr_sessionDataInit")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `fileName` | The name of the code to get. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `String` | The content of the code if found. |

??? info "ReplaceGMLString `method`"
    ### Summary
    Replaces a code in the game with a string of GML.
    ### Example
    ```c#
    ModLoader.ReplaceGMLString("gml_GlobalScript_scr_sessionDataInit")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The string of GML code to insert. |
    | `String` | `fileName` | The name of the code to replace a line. |
    | `Int` | `position` | The line to replace with the provided GML. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

---

## Assembly

??? info "InsertAssemblyString `method`"
    ### Summary
    Inserts an assembly instruction below the provided line.
    ### Example
    ```c#
    // Example : Inserts the instruction 'pushi.e 1' in 'gml_GlobalScript_scr_sessionDataInit' below line 14.
    ModLoader.InsertAssemblyString("pushi.e 1", "gml_GlobalScript_scr_sessionDataInit", 14)
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The string of assembly instruction(s) to insert. |
    | `String` | `fileName` | The name of the code to insert the assembly into |
    | `Int` | `position` | The line below which we inject the assembly instruction(s) |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

??? info "ReplaceAssemblyString `method`"
    ### Summary
    Replaces the provided line with an assembly instruction.
    ### Example
    ```c#
    // Example : Replaces the 8th line in 'gml_GlobalScript_scr_sessionDataInit' with 'pushi.e 1'.
    ModLoader.ReplaceAssemblyString("pushi.e 1", "gml_GlobalScript_scr_sessionDataInit", 8)
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The string of assembly instruction(s) to insert. |
    | `String` | `fileName` | The name of the code to insert the assembly into |
    | `Int` | `position` | The line below which we inject the assembly instruction(s) |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `N/A` | N/A |

---

## Functions / Strings / Variables

??? info "AddFunction `method`"
    ### Summary
    Adds a function to the game.
    ### Example
    ```c#
    ModLoader.AddFunction("scr_actionsLogUpdate(\"Hello World !\")", "myFunction")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `codeAsString` | The gml code to put in the function as a string. |
    | `String` | `name` | The name of the function to create. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleCode` | The code created for the function. |

??? info "GetVariable `method`"
    ### Summary
    Gets a variable from the game files.
    ### Example
    ```c#
    ModLoader.GetVariable("display_x")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the variable to get. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleVariable` | The variable if found. |

??? info "GetString `method`"
    ### Summary
    Gets a variable from the game files.
    ### Example
    ```c#
    ModLoader.GetString("questBreweryOdarAccept00")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `name` | The name of the string to get. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `UndertaleString` | The string if found. |

---

## Patching : Stage 1 (Loading)

The 'Loading' stage is used to get the GML or assembly from the game's files.

??? info "LoadGML `method`"
    ### Summary
    The first step to inject GML. </br>
    Loads an existing code from the game files and returns it.
    ### Example
    ```c#
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `fileName` | The name of the code to get from the game files. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | A class containing the loaded GML file, used in Stage 2 methods. |

??? info "LoadAssemblyAsString `method`"
    ### Summary
    The first step to inject Assembly. </br>
    Loads the assembly corresponding to an existing code from the game files and returns it.
    ### Example
    ```c#
    ModLoader.LoadAssemblyAsString("gml_GlobalScript_scr_sessionDataInit")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `String` | `fileName` | The name of the code to get the assembly from in the game files. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | A class containing the assembly for the loaded code, used in Stage 2 methods. |

---

## Patching : Stage 2 (Matching)

The 'Matching' stage is used to select a line, or multiple lines that we're going to act upon in stage 3.
It is done by providing code to find in the file, or matching every line.

??? info "MatchFrom `method`"
    ### Summary
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

    ### Examples
    ```c#
    // Example 1 : Selecting a matching String in a GML file
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("if (!instance_exists(o_music_controller))")

    // Example 2 : Selecting the matching content of a file in a GML file
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom(ModFiles, "my_matching_file.gml")

    // Example 3 : Selecting a matching assembly instruction String in an assembly file
    ModLoader.LoadAssemblyAsString("gml_GlobalScript_scr_sessionDataInit").MatchFrom("pushi.e 1")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<string>` | `fe` | The previously opened file |
    | `String` | `other` | The code to find and match with the file |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<(Match, string)>` | A class that contains the loaded code with selected parts defined, ready for Stage 3. |

??? info "MatchBelow `method`"
    ### Summary
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


    ### Example
    ```c#
    // Example 1 : Selecting 1 line below a matching String in a GML file
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchBelow("if (!instance_exists(o_music_controller))", 1)

    // Example 2 : Selecting the 4 lines below a matching file in a GML file
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchBelow(ModFiles, "my_matching_file.gml", 4)

    // Example 3 : Selecting 1 line below a matching assembly instruction String in an assembly file
    ModLoader.LoadAssemblyAsString("gml_GlobalScript_scr_sessionDataInit").MatchBelow("pushi.e 1", 1)
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<string>` | `fe` | The previously opened file |
    | `String` | `other` |The code to match with the file |
    | `Int` | `len` | The amount of lines below the match to select. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<(Match, string)>` | A class that contains the loaded code with selected parts defined, ready for Stage 3. |

??? info "MatchAll `method`"
    ### Summary
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

    ### Example
    ```c#
    // Example : Selecting every line in a GML file
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchAll()
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<string>` | `fe` | The previously opened file. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<(Match, string)>` | A class that contains the loaded code with selected parts defined, ready for Stage 3. |

---

## Patching : Stage 3 (Acting)

The 'Acting' stage is used to perform an action on the selected lines. </br>
This can be inserting, replacing, deleting or others, which lead to modification of the original file.

??? info "Remove `method`"
    ### Summary
    The third step to inject GML / assembly. </br>
    Removes previously selected lines from opened file.
    ### Example
    ```c#
    // Example 1 : Remove all lines from script gml_GlobalScript_scr_sessionDataInit
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchAll().Remove()

    // Example 2 : Remove the `pushi.e 1` instruction from gml_Object_c_bed_sleep_crafted_Alarm_0's assembly
    ModLoader.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").Remove()
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "KeepOnly `method`"
    ### Summary
    The third step to inject GML / assembly. </br>
    Removes all non-selected lines from opened file.
    ### Example
    ```c#
    // Example 1 : Remove all lines from script gml_GlobalScript_scr_sessionDataInit except the line where 'global.HP = -1' appears.
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").KeepOnly()

    // Example 2 : Remove all instructions from gml_Object_c_bed_sleep_crafted_Alarm_0's assembly except `pushi.e 1`
    ModLoader.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").KeepOnly()
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "InsertAbove `method`"
    ### Summary
    The third step to inject GML / assembly. </br>
    Inserts a string/file content above the selected line.
    ### Example
    ```c#
    // Example 1 : Adds 'global.myVar = 14' above the line containing 'global.HP = -1'
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").InsertAbove("global.myVar = 14")

    // Example 2 : Adds the content of my_gml_code.gml above the line containing 'global.HP = -1
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").InsertAbove(ModFiles, "my_gml_code.gml")

    // Example 3 : Adds the instruction 'popz.v' above the line containing the instruction 'pushi.e 1'
    ModLoader.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").InsertAbove("popz.v")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |
    | 'String' | `inserting` | The code to insert above the selected line.

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "InsertBelow `method`"
    ### Summary
    The third step to inject GML / assembly. </br>
    Inserts a string/file content below the selected line.
    ### Example
    ```c#
    // Example 1 : Adds 'global.myVar = 14' below the line containing 'global.HP = -1'
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").InsertBelow("global.myVar = 14")

    // Example 2 : Adds the content of my_gml_code.gml below the line containing 'global.HP = -1
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").InsertBelow(ModFiles, "my_gml_code.gml")

    // Example 3 : Adds the instruction 'popz.v' below the line containing the instruction 'pushi.e 1'
    ModLoader.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").InsertBelow("popz.v")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |
    | 'String' | `inserting` | The code to insert below the selected line. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "ReplaceBy `method`"
    ### Summary
    The third step to inject GML / assembly. </br>
    Replaces the selected line with a string/file content.
    ### Example
    ```c#
    // Example 1 : Replaces the line containing 'global.HP = -1' with 'global.HP = 50'
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").ReplaceBy("global.HP = 50")

    // Example 2 : Replaces the line containing 'global.HP = -1 with the content of 'my_gml_code.gml'
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").ReplaceBy(ModFiles, "my_gml_code.gml")

    // Example 3 : Replaces the line containing the instruction 'pushi.e 1' with the instruction 'popz.v'
    ModLoader.LoadAssemblyString("gml_Object_c_bed_sleep_crafted_Alarm_0").MatchFrom("pushi.e 1").ReplaceBy("popz.v")
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |
    | 'String' | `inserting` | The code to replace the selected line with. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

??? info "FilterMatch `method`"
    ### Summary
    The third step to inject GML / assembly. </br>
    TODO
    ### Example
    ```c#
    TODO
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<string>` | The file with the result of the action. |

---

## Patching : Stage 4 (Saving)

The 'Saving' stage saves the patched difference into the original file. </br>
Failing to add this at the end of your chain of calls will render it useless.

??? info "Save `method`"
    ### Summary
    The final step to inject GML / assembly. </br>
    Saves the modified content to the original file.
    ### Example
    ```c#
    // Example : Loads a script, selects the line containing 'global.HP = -1', replaces it with 'global.HP = 50' and saves it.
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").MatchFrom("global.HP = -1").ReplaceBy("global.HP = 50").Save()
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<(Match, string)>` | `fe` | The previously opened file. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `ModSummary` | The file with the result of the action. |

---

## Patching : Utility

??? info "Peek `method`"
    ### Summary
    Can be used before at any stage before saving to print a log in MSL's console containing the current state of the file. </br>
    This method doesn't modify the input at all, simply prints it and passes it forward to the next method.
    ### Example
    ```c#
    // Example : Prints the state of the file before and after replacing.
    ModLoader.LoadGML("gml_GlobalScript_scr_sessionDataInit").Peek().MatchFrom("global.HP = -1").ReplaceBy("global.HP = 50").Peek().Save()
    ```
    ### Arguments
    | **Type** | **Name** | **Description**|
    | :---: | :---: | :--- |
    | `FileEnumerable<T>` | `fe` | The previously opened file. |

    ### Returns
    | **Type** | **Description** |
    | :--- | :--- |
    | `FileEnumerable<T>` | The file, exactly as it was when passed as an argument. |
