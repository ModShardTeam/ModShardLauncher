#  Let's begin!

[TOC]

## Before we start

If you want to create a mod, PLEASE make sure you know how C# works, otherwise you may not understand how to use tool's API.

## Tools

You need a tool to write your mod code, such as **Visual Studio**. It's a professional development tool that can help you a lot.

[Download here](https://visualstudio.microsoft.com/)

While installation, make sure you choose `.NET Desktop Development` workload. You don't need to download other workloads.

At last, find `.NET 6.0 SDK` on official Microsoft website, install it and reboot your PC.

You can also use **Visual Studio Code** as your tool, but this tutorial won't introduce this tool. ~~Cuz I havent done it~~.

## Create your first mod!

Let's start our journey by launching **ModShardLauncher.exe**. Once you launch it, two files will be created in the folder, one is **Mods**, another is **ModSource**. Create new folder in **ModSource**.

### Create your mod!

There are many ways to create your mod, if you use Visual Studio, you can run it then click Create New Project. Search Class Library, click Next.

<center><img src="../img/create_project_0.png" width=50%></center>

Give a name to your mod, then select the path Mods that **ModShardLauncher.exe** just create.
<center><img src="../img/create_project_1.png" width=50%></center>

Choose **.Net 6.0**, and that's it! easy right?
<center><img src="../img/create_project_2.png" width=50%></center>

### Assembly Reference!

First we should add reference for the tool's assembly, just the .dll file.

Open the solution explorer.
<center><img src="../img/mod_0.png" width=50%></center>

Right click the "References", then click "Add Project Reference"
<center><img src="../img/mod_1.png" width=50%></center>

Click browse.
<center><img src="../img/mod_2.png" width=50%></center>

In the end, select ModShardLauncher.dll and then click add.
<center><img src="../img/mod_3.png" width=50%></center>

### Main class

Now we need to create a Main Class for the mod.

If you have basis for C#, then you must know about **Class**.Then we will create a Mod's main class.

While you create a new project, VS should create a class named Class1. We should add reference for `ModShardLauncher` and `ModShardLauncher.Mods` namespace. Then edit the code into:
<center><img src="../img/class_0.png" width=50%></center>

As you can see, we changed the prefix from `internal` to `public`, so that when loading the mods it can read this class. Then we change the name of class into `MyFirstMod`, and let this class extend `Mod` class.

### Information of your mod

Now we need to add the basic information to the mod.

Add code in `MyFirstMod`:
```C#
public override string Name => "MyFirstMod";
public override string Author => "Mantodea";
public override string Description => "My first mod";
```

Once you've done this, the information of the mod name, creator and brief description will be set.

### Compile the Mod

Launch **ModShardLauncher**. As you can see, the source code of the mod has been loaded.
<center><img src="../img/compile_0.png" width=50%></center>

Before we compile the mod, we need to click File - Open on top left, and choose data.win file in **Vanilla** file.

??? reason "Why vanilla file?"

    This tool works based on the names of objects in data.win file

??? reason "Why loading game file before compiling?"

    To access the right game virsion, and prevent the little chance of collapse if the game crashes when loading a different version.

Then we can double click `MyFirstMod` and compile the mod.

Result of compiling: 
<center><img src="../img/compile_1.png" width=50%></center>

## Create your first weapon

You have compiled your first mod just now! Let's add some content next.

### Create Weapon Class

Right click the Solution Explorer, right click your project, click 'add', and click 'add new'
<center><img src="../img/weapon_0.png" width=50%></center>

Name your weapon. In this tutorial I will name it as `MyFirstWeapon`. Same as above, change `internal` into `public` so that it can read the information of this weapon while loading the mod. Then add the reference for `ModShardLauncher` and `ModShardLauncher.Mod` namespace, and let the weapon extend the `Weapon` class.

### Change the information of your weapon!

In StoneShard, there are many properties for a single weapon. It is painful to set them all and if you forget to set some properties, mod may not be loaded as expected.

??? why "You know too much"
    ~~Actually cuz the source code of StoneShard is kinda messy.~~

Then, are there any ways that can help us easily change the properties of a weapon?

First, let's add some code to edit `SetDefaults` method
```C#
public override void SetDefaults()
{
    
}
```
??? why "You know too much"
    ~~looks like the codes are from tModLoader, right? ugh, well, you know, I'm a modder.~~

As you can see, this method can set weapon's properties while loading it.

Now, let's introduce **`CloneDefaults`** method.

That's right, to prevent modders from forgetting setting those properties, we offer a new method so that every properties will be copied from another vanilla weapon except `Name` and `ID`. Just like this:

```C#
public override void SetDefaults()
{
    CloneDefaults("Homemade Blade");
    Name = "MyFirstWeapon";
    ID = "MyFirstMod1";
}
```

Now, we have a new weapon with the same properties except the name.

Notice: `CloneDefaults` won't set value on the name and description in different languages. You need to change them by yourself, like this:

```C#
public override void SetDefaults()
{
    CloneDefaults("Homemade Blade");
    Name = "MyFirstWeapon";
    ID = "MyFirstMod1";
    Description[ModLanguage.English] = "this is my first weapon";
    NameList[ModLanguage.English] = "FuckingGreatSword";
}
```

That's the first step of creating a weapon.

### Sprite of your weapon

This is the most painful part. In StoneShard, you need 6 sprites for a weapon, which adds tons of work to modders.


If you don't have any sprites, you can export vanilla sprites by UTMT, and put them in the mod path except `.vs, bin and obj`. They will be patched in data.win automatically.

<center><img src="../img/weapon_1.png" width=50%></center>

From top to bottom is: weapon on right hand, weapon on left hand, weapon in backpack (there are 3 sprites for this, since the weapon in StoneShard can be broken. If you don't want to sprite it ,just copy 3 times), and loot weapon.

??? why "notice"
    
    The length of sprites of weapon in the bag must be 27n cuz an unit of StoneShard inventory is 27*27 pixels.

The sprites above are just for one-hand weapon, if for two-hand ones, you don't need char_left, cuz two-hand weapons can only be used by two hands.

Get ready for all these things, you can run **ModShardLauncher.exe** again and repeat the compile steps above.

### Loading your mod!

!!! warn "**notice !!!**"
    You need to move a `ModShard.dll` file under the tool's root path to root folder of the game. It is an inner extension of the tool. Otherwise you can't launch the game. 

The last step is loading the mod. Once you compile your mod, it will pop up in Mod page. Double click it and click Enable down below. Then click Patch on top left corner, and your mod data will be patched in vanilla data.win that is loaded before. You can save the patched data into any folder, move the vanilla data.win file away, start the game, and it will ask you to select a data file, choose the file that we save before.

### Launch your game!

After launching the game, there is a new window that is opened by the plugins. It is a control pannel that runs some inner script in game.

Enter the game as usual, the plugin set a inner give script, type in
```
give MyFirstWeapon
```
to get the mod weapon you created in the `Script` textbox under the plugin window.

If you want to get other weapon in this way, please edit space in the weapon name into `_`, such as:
```
give Homemade_Blade
```
Once it process the give script, you will find that there is a mod weapon in your backpack.
<center><img src="../img/weapon_2.png" width=50%></center>