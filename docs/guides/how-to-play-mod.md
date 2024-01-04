

# So how can I install mods with this tool?

!!! info "Rewrite needed"
    This guide should be rewritten soon. <br>
    It is still accurate, but it is not as clear as it could be.

## Downloading ModShardLauncher

---

First let's download [**ModShardLauncher**](https://github.com/DDDDDragon/ModShardLauncher/releases) on Github.

Once downloaded, you should run it once to generate the required folders. <br>
You should be able to see two new folders in ModShardLauncher's directory : `Mods` and `ModSources`.

Now let's have a look at the tool itself.

## ModShardLauncher's UI

---

<center><img src="../img/tool_UI.png" width=50%></center>

First, let's introduce the buttons on the left side. <br> <br>
The top one is the `Menu` button. <br> 
Clicking this button extends the panel to show the different pages' names :

<center><img src="../img/tool_UI2.png" width=50%></center>

The second button is the `Mods` button. <br> 
It opens a new page where you can see which mods you currently have installed :

<center><img src="../img/tool_UI3.png" width=50%></center>
<center style="color:Gray">
Here's an exemple of this page with a loaded mod (here SpeedShard by Zizani)
</center>

The third button is the `ModSources` button. <br> 
There you can find the source code for mods you're making. <br>
If you're a player, and not a modder, you can ignore this page as you won't need it.

<center><img src="../img/tool_UI4.png" width=50%></center>

And the last one is the `Settings` button. <br> 
It opens the settings page, where you can configure ModShardLauncher, and one day mods as well.

<center><img src="../img/tool_UI5.png" width=50%></center>

## Patching mods into the game

---

Now that we know how the UI works, let's patch some mods into the game's files.

???+ info "Keeping a backup"
    It is strongly advised to rename the `data.win` file in the `Stoneshard` folder to `vanilla.win`.

    ---

    This is done so the game lets you load either your **modded** or your **vanilla** game files. <br> 
    It also helps make sure you keep your **original** game files **safe**.

!!! warning "Mod version"
    Make sure that the mod's **game version** is the **same as yours**. <br>
    While mods ***may*** work with different game versions, it is **not guaranteed**.



- First, click the "**folder**" button at the top of this page. <br>
Navigate to your `Stoneshard` folder and select your vanilla `data.win`.

<center><img src="../img/tool_UI6.png" width=50%></center>
<center style="color:Gray">
A loading window should appear.
</center>

- Next, head to the `Mods` page and select the mod(s) you want to enable. <br>
Just tick the box next to a mod to enable it. <br>


- Once it's done, click on the ``Save`` button to **save** your **modded game files**. <br>
    You can **save** it under **any name** you want, but it is **strongly recommended** to **not overwrite** your **original** `data.win`.


- For the final step, move the `ModShard.dll` file to the `Stoneshard` folder. <br>

!!! warning "ModShard.dll"
    This file is **required** for the game to **load mods**. <br>
    Trying to load a **modded data file** without it will cause the game to **crash**.


## Launch your game!

---

After launching the game, a new window should open alongside Stoneshard. <br>
You can safely **ignore** this window, as it is only used as a **debugging tool** / **console**.

Once you're in the game, you should be able to see the mod's changes. <br>