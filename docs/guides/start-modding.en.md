# Modding Stoneshard with MSL

## Introduction

---

ModShardLauncher (or MSL) is a tool that has the ambition of becoming the **prefered method of modding Stoneshard**. </br></br>
It is very much at an **early stage of development** right now, and as such quite a few **key features are still missing**. </br>
Additionally, some of the included features are still **rough around the edges** and not as **friendly to beginners** as we'd like them to be. </br></br>

!!! warning "Important  "
    At this stage, modding Stoneshard with MSL **^^requires you to know how to mod the game^^** with [**UndertaleModTool**](https://github.com/krzys-h/UndertaleModTool). </br></br>
    This is because the focus of the project right now is on **including more features**, rather than making the existing ones **more accessible**. </br></br>
    Eventually however, the project will reach a stage where **no knowledge of GML** or inner workings of the game is necessary for **simple mods** like adding weapons, swapping textures, etc...

## Before we Start

---

Before creating a mod, you should have a **basic understanding** of **C#** and **Visual Studio**.

If you do not know how to use them, please **learn them first**, as MSL's API can be a bit complex at times. </br>
Here are some **resources** to get you started on that :

- [C# Tutorial](https://www.w3schools.com/cs/index.php)
- [Visual Studio Tutorial](https://www.youtube.com/watch?v=VcU2HGsxeII&t=34s)

## Tools

---

??? abstract "**IDEs**"
    In order to write your mod's code, you'll need a tool such as [**Visual Studio**](https://visualstudio.microsoft.com/), [**Jetbrains Rider**](https://www.jetbrains.com/rider/) or even [**Visual Studio Code**](https://code.visualstudio.com/). </br>
    !!! question "What do they do ?"
        These are professional development tools that can help you write code. </br>
        This guide will use **Visual Studio**, as it is both free and easier to use for beginners than **Rider**.

    While installing **Visual Studio**, make sure you choose the `.NET Desktop Development` workload. </br>
    You do not need to download any other workload.

??? abstract "**.NET 6.0 SDK**"
    Download the [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0), install it and when it's done, restart your computer.

??? abstract "**ModShardLauncher Template (Optional)**"
    You can download the official template for MSL Mods to avoid having to write boilerplate code. </br>
    The easiest way to install it is to open a terminal and run the following command :

    `dotnet new --install ModShardLauncher.Templates`

## Getting Started

---

Let's start our modding journey by starting `ModShardLauncher.exe`. </br>
This should create the `Mods` and `ModSources` folder if they didn't exist already. </br></br>
Once this is done, you can simply close MSL for now.

Now, let's create a new mod using the MSL Template. </br>
To do this, open up Visual Studio, click Create a new project, and in the search bar type `msl`.

![MSL Tempalte](../img/msl_template.png)

For MSL to recognize your mod you need to create the new project in MSL's `ModSources` folder. </br>
Make sure to also tick the `Place solution and project in the same directory` box.

![MSL Template Location](../img/msl_template_location.png)

Hit the `Create` button and you'll be good to go !

Alternatively, you can also create a new project using the mod template with the CLI. </br>
To do this, open a terminal, navigate to MSL's `ModSources` folder and type the following command :

`dotnet new msl --name "MyMod"`

Regardless of your method, you should now see a new `MyMod` folder in your `ModSources` folder. </br>
It should contain a solution, which you can open in your **IDE** of choice.

## Boilerplate Code

---

Now that our project is set up, you should have a working solution with a class containing some boilerplate code. </br>
Let's quickly go over what it does, and why it needs to be there.

```c# title="MyMod.cs" linenums="1"
using ModShardLauncher;
using ModShardLauncher.Mods; // (1)!

namespace MyMod
{
    public class MyMod : Mod // (2)!
    {
        public override string Author => "author"; // (3)!
        public override string Name => "mod_name"; // (4)!
        public override string Description => "mod_description"; // (5)!
        public override string Version => "1.0.0.0"; // (6)!

        public override void PatchMod() // (7)!
        {

        }
    }
}
```

1. The `using` directives allow us to use code from ModShardLauncher to write our mod.
2. This is the base class for your mod. </br> All of your code should be contained withing it.
3. This variable contains the name of the author of this mod. </br> Feel free to replace it with yours ! </br> It's visible in MSL's Mod list.
4. This variable contains your mod's name. </br> Feel free to change it ! </br> It's visible in MSL's Mod list.
5. This variable contains your mod's description. </br> Feel free to change it ! </br> It's visible in MSL's Mod list.
6. This variable contains your mod's version. </br> Don't forget to change it when you update your mod ! </br> It's visible in MSL's Mod list.
7. The `PatchMod` method is your mod's entry point. </br> Anything within it gets run by the compiler when your mod is patched into a `.win` file.

!!! tip "Tip"
    Click on the `+` button next to the code to learn what it does.

## Compiling our Mod

---

Right now our mod doesn't do, well, anything, but for the sake of learning let's compile it anyway. </br> </br>

- First, let's make sure we saved any change we made to `MyMod.cs` by pressing ++ctrl+s++. </br>
- Open MSL, click the folder icon at the top of the window and select your `vanilla.win` file.
- Head to the `ModSources` menu (by clicking the C# button on the left). </br>
- Your mod should be listed here with a `Compile` button next to it.

??? question "My mod isn't there !"
    If your mod isn't there, try restarting MSL. </br>
    If you created your project after opening MSL it won't show in the list until your restart it. </br></br>
    If your mod still doesn't appear after restarting, make sure you created your mod in the `ModSources` folder.

- Click the `Compile` button. </br> This generates a `.sml` file in the `Mods` folder, which is your mod file. </br> This is the file you can distribute to players.
- There's no feedback when clicking on the `Compile` button, so don't be alarmed, it's normal.
- Next head to the `Mods` menu, and tick the box next to your mod.
- Click the save button at the top of the window and save under the name `MyMod.win`.

That's it you've now compiled your mod ! </br>
You can now start Stoneshard, select `MyMod.win` and test your mod.

## Basic Injections

---

At this point we've got a basic mod, but it doesn't really do anything. </br>
Let's change that by injecting some code into the game's files. </br>

In the `MyMod.cs` class, let's add the following code :

``` c# title="MyMod.cs" linenums="1" hl_lines="15"
using ModShardLauncher;
using ModShardLauncher.Mods;

namespace MyMod
{
    public class MyMod : Mod
    {
        public override string Author => "author";
        public override string Name => "mod_name";
        public override string Description => "mod_description";
        public override string Version => "1.0.0.0";

        public override void PatchMod()
        {
            Msl.InsertGMLString("scr_smoothSaveAuto()", "gml_Object_o_player_KeyPress_116", 0);  // (1)!
        }
    }
}
```

1. This line inserts the code `scr_smoothSaveAuto()` into the `gml_Object_o_player_KeyPress_116` at line `0`. </br></br> We're injecting the gml code necessary to quicksave the game when the player presses the F5 key.

As you can see, using ModLoader.InsertGMLString allows us to inject a string of GML into an existing script. </br>
But MSL's capabilities don't stop there, we can read code from a .gml file you wrote and inject it into a script. </br></br>
For example, let's create a `Codes` folder in our project :

![Creating Codes Folder](../img/modding_codes.png)

Inside the `Codes` folder, let's create a `myCode.gml` file with the following code :

``` c title="myCode.gml" linenums="1"
scr_actionsLogUpdate("Quicksaving...")
scr_smoothSaveAuto()
scr_actionsLogUpdate("Game Saved.")
```

Now back in `MyMod.cs`, let's change our code to the following :

``` c# title="MyMod.cs" linenums="1" hl_lines="15"
using ModShardLauncher;
using ModShardLauncher.Mods;

namespace MyMod
{
    public class MyMod : Mod
    {
        public override string Author => "author";
        public override string Name => "mod_name";
        public override string Description => "mod_description";
        public override string Version => "1.0.0.0";

        public override void PatchMod()
        {
            Msl.InsertGMLString(ModFiles.GetCode("myCode.gml"), "gml_Object_o_player_KeyPress_116", 0);  
            // (1)!
        }
    }
}
```

1. We replaced the hardcoded string with `ModFiles.GetCode("mycode.gml")` which grabs the content of `myCode.gml` and injects it into the script.

Now if we compile this mod and press F5 in game, it should quicksave our game. </br>
That's great, but what if we want to edit a script that breaks when edited in UTMT ? </br></br>
The solution is the exact same as in UTMT, we can edit the assembly directly !

``` c# title="MyMod.cs" linenums="1" hl_lines="15"
using ModShardLauncher;
using ModShardLauncher.Mods;

namespace MyMod
{
    public class MyMod : Mod
    {
        public override string Author => "author";
        public override string Name => "mod_name";
        public override string Description => "mod_description";
        public override string Version => "1.0.0.0";

        public override void PatchMod()
        {
            Msl.InsertAssemblyString(":[0]\ncall.i gml_Script_scr_smoothSaveAuto(argc=0)\npopz.v", "gml_Object_o_player_KeyPress_116", 1 );
            // (1)!
        }
    }
}
```

1. This line inserts the assembly instructions corresponding to the quicksave code seen in previous examples.

!!! info "Important Note"
    The demonstrated functions are far from being the only options to inject our code into the game files. </br>
    This guide is meant for beginners, and as such tries to keep things relatively simple. </br></br>
    For more options, feel free to check out the [API](./api.md).

## Advanced Injections

---

For more advanced mods, you might require more accurate injections than simply inserting or replacing code or assembly instructions at specific lines. </br></br>
To that effect, MSL offers a collection of methods for precise injection, which also helps making your injections more resilient to updates by matching a specific line or set of line rather than using a line number. </br>

They are divided in 4 different stages, namely :

- Loading
- Matching
- Acting
- Saving

Here's an example of that :

``` c# title="Chained Methods"
Msl.LoadGML("gml_GlobalScript_scr_sessionDataInit") // Loading a script from the game's files
.MatchFrom("global.HP = -1") // Finding the line containing `global.HP = -1`
.ReplaceBy("global.HP = 50") // Replacing it with `gobal.HP = 50`
.Save() // Saving the file
```

!!! info "Important Note"
    Once again, this is only a simple example of the available functions, to learn more check out the [API](./api.md).

</br></br>
