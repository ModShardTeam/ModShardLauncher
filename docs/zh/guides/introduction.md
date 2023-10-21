# ModShardLauncher

## 什么是**ModShardLauncher**?

**ModShardLauncher**是一个用于给StoneShard(紫色晶石)这款游戏加载mod的工具。

在我开发这款工具之前, mod作者们都是使用**UTMT**, 即UndertaleModTool这款工具来开发mod并保存的. 这样开发的难点在于, 作者们只能制作源码mod, 也就是说, 不同的mod不能一起加载, 除非你自己把他们的内容整合到一起. 而且UTMT使用较为繁琐, 并不适合用于开发StoneShard的mod. 一旦作者进行更新, 所有mod将不再适用, 源文件需要重新被编辑和保存.

为了解决这些痛苦的问题, 我打算开发一款工具, 也就是**ModShardLauncher**.

## **ModShardLauncher**是如何工作的?

实际上, **UTMT**是用C#这门语言开发的. 因此, 引用它的源码就可以很方便的读取和保存data.win中的数据. 并且在C#强大的反射功能支持下, mod作者们可以通过该工具内置的打包器将所有mod代码以及贴图打包成 `.sml` 文件, 然后工具内置的读取器可以读取这种格式的文件, 并将其中的数据打包进新的data.win文件. 以达成多mod共存, 便捷开发的目的.

## 太棒辣 我现在就想开发一个自己的mod!

[那就从这里开始吧!](../guides/start-modding.md)