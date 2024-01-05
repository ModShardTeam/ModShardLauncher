# ModShardLauncher

## What is **ModShardLauncher**?

---

**ModShardLauncher** is a tool to patch **mods** into StoneShard original data files. <br>

In the past, we modders used a tool called **UndertaleModTool (UTMT)** to edit the source code and save it. <br>
But if the game received even the most insignificant of updates, all mods would break. <br> 
Additionally, multiple mods couldn't work together unless you actually combined them by hand. <br>

To deal with these issues and limitations, I wanted to make a tool. <br>
That's what **ModShardLauncher** is.

## How does **ModShardLauncher** work?

---

Did you know that **UTMT** was made in C# ? <br>

Using **UTMT**'s source code, **ModShardLauncher** can load data files. <br>
And with C#'s reflection, **ModShardLauncher** can load .dll files as mods and patch the '**modthings**' into data files, then save them.

## I want to start modding now!

---

[Just check out the guides here !](../guides/start-modding.md)