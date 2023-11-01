# 开始吧!

[TOC]

## C#基础

如果你想要制作mod, 那请确保你有一定的C#基础, 不然你可能会看不懂工具的API是怎么用的.

## 下载工具

要想制作mod, 你首先需要一个写代码的工具.

比如**Visual Studio**, 这是一款很专业的开发工具, 你可以用它方便地开发mod.

[在这里下载](https://visualstudio.microsoft.com/)

在下载时, 确保你选择了 `.NET Desktop Development` 工作负载. 其他的负载可以不用下载.

最后, 到微软官网查找 `.NET 6.0 SDK` 来下载安装包. 然后重启你的电脑来应用这些更改.

另一个选择是**Visual Studio Code**, 在我看来其实用VSCode就足以制作StoneShard的mod了. 但是我们此处先不详细叙述.~~实际上是我没写完教程哒~~

## 创建你的第一个mod

首先, 你需要启动 **ModShardLauncher.exe** , 启动之后它会在根目录下创建两个文件夹,
一个是Mods, 另一个是ModSources.

然后在ModSources文件夹中创建一个新的文件夹, 你的第一个mod就从这里开始了.

### 创建mod!

创建mod的方式有很多, 我们先来拿**Visual Studio**举例.
首先运行VS, 点击创建新项目, 搜索类库并单击下一步. 如图: 
<center><img src="../../img/create_project_0.png" width=50%></center>

然后输入你mod的名字, 选择路径为刚才 **ModShardLauncher.exe** 创建的 Mods 文件夹.
<center><img src="../../img/create_project_1.png" width=50%></center>

最后选择 .Net 6.0即可
<center><img src="../../img/create_project_2.png" width=50%></center>

### 程序集引用!

首先我们需要引用工具的程序集, 即Dll文件.

先打开解决方案资源管理器.
<center><img src="../../img/mod_0.png" width=50%></center>

右键依赖项, 并单击添加项目引用.
<center><img src="../../img/mod_1.png" width=50%></center>

点击浏览.
<center><img src="../../img/mod_2.png" width=50%></center>

最后选择ModShardLauncher.dll并点击添加.
<center><img src="../../img/mod_3.png" width=50%></center>

### Mod主类!

如果你有C#基础, 那你一定对 **类**(Class) 有了解.我们接下来就要创建一个Mod的主类.

创建项目时, VS应该已经为我们创建了一个类, 名字叫做Class1. 我们要做的就是先添加对 `ModShardLauncher` 和 `ModShardLauncher.Mods` 这两个命名空间的引用. 然后将代码改成如下这样:
<center><img src="../../img/class_0.png" width=50%></center>

可以看到我们先是把类的访问级别从 `internal` 改为了 `public` , 这样一来Mod加载时就可以读取到这个类. 然后我们把类名改成了 `MyFirstMod` , 并让这个类继承 `Mod` 类.

### Mod信息!

接下来我们给Mod添加基础的信息.

在 `MyFirstMod` 类中添加如下代码:
```C#
public override string Name => "MyFirstMod";
public override string Author => "Mantodea";
public override string Description => "我的第一个mod";
```
这样我们就设置了mod的名称, 作者与描述信息.

### 编译Mod!

接下来我们启动**ModShardLauncher**.  可以看到我们的Mod源码已经被加载出来了.
<center><img src="../../img/compile_0.png" width=50%></center>

在编译mod之前, 我们需要先点击模组界面左上角的文件夹按钮, 并选择 **原版** 的data.win文件进行加载.

??? reason "为什么要使用原版文件?"

    这个工具是基于data.win文件内的各种信息的名称来工作的, 如果你已经加入了源码mod, 很可能会出现各种崩溃的情况.

??? reason "为什么要加载游戏文件才能编译?"

    为了获取游戏版本, 防止极小可能出现的版本不同而崩溃现象(确信)

然后我们就可以点击 `MyFirstMod` 栏位右下角的编译辣! (UI现在嘎嘎好看是不是)

编译成功后的结果: 
<center><img src="../../img/compile_1.png" width=50%></center>

## 创建你的第一把武器

就在刚刚, 你的第一个mod已经成功编译了! 接下来, 让我们为它添加一些看得到的东西吧.

### 创建武器类!

首先点击右侧的解决方案资源管理器, 然后右键你的项目, 点击添加, 最后点击新建项, 如图:
<center><img src="../../img/weapon_0.png" width=50%></center>

名字就输入这把武器的名称即可, 这里我们使用 `MyFirstWeapon` 作为它的名字.

接下来进入代码界面, 还是一样的操作, 将 `internal` 改为 `public` , 以便Mod加载时可以加载到这把武器的信息. 然后添加对 `ModShardLauncher` 和 `ModShardLauncher.Mods` 这两个命名空间的引用.  并让武器类继承 `Weapon` 类.

### 修改武器的信息!

玩过紫晶的人都知道, 紫晶里的武器属性很多. 挨个设置不仅麻烦, 还会很痛苦, 而且还有可能落下某些属性, 导致mod没法正常加载.

??? why "你知道的太多了"
    ~~实际上是因为毛子写的代码很傻逼.~~

那么有没有一种办法可以让我们简单快捷的设置一把武器的属性呢?

首先我们添加如下代码来重写 `SetDefaults` 方法.
```C#
public override void SetDefaults()
{
    
}
```
??? why "你知道的太多了"
    ~~有种tModLoader的风格, 我已经被荼毒了.~~

看英文可以知道, 这个方法用于在加载武器时设定它的属性.

接下来我们隆重介绍---- **`CloneDefaults`** 方法!

没错, 为了防止玩家累死(不是) 我们modder开发时经常忘记各种属性, 我提供了一个方法来让当前这把武器的除 `Name` 与 `ID` 两个属性之外的所有属性全部照抄另外一把原版武器----**`CloneDefaults`**! 因此, 只需把代码改成这样:
```C#
public override void SetDefaults()
{
    CloneDefaults("Homemade Blade");
    Name = "MyFirstWeapon";
    ID = "MyFirstMod1";
}
```

这样一来, 这把武器就变成了一把除了名字不一样其他全部一样的换皮土刀(有种NTR的感觉)

但是需要注意的是, `CloneDefaults` 并不会对武器的各种语言名称和介绍进行赋值, 这些仍然需要你手动修改. 因此我们再加上两行:

```C#
public override void SetDefaults()
{
    CloneDefaults("Homemade Blade");
    Name = "MyFirstWeapon";
    ID = "MyFirstMod1";
    Description[ModLanguage.Chinese] = "这是我的第一把武器";
    NameList[ModLanguage.Chinese] = "我的神剑咖喱棒";
}
```

这样一来我们的第一把武器算是初步完成了.

### 武器贴图!

StoneShard做mod最痛苦的一部分就是这里了. 贴图, 一把最基础的武器竟然需要六张贴图, 这无疑增加了modder的工作量.

如果你没得贴图, 你可以使用UTMT对原版的贴图进行导出, 然后放在Mod目录除 `.vs, bin 和 obj` 的任何地方, 打包时会自动将他们打包进data.win的.
<center><img src="../../img/weapon_1.png" width=50%></center>

如图所示, 从上到下依次是: 人物右手拿武器, 人物左手拿武器, 背包中的武器(有三张的原因是紫晶的武器有破损系统, 如果你不想画, 可以把完整版的武器复制三遍), 掉落的武器.

??? why "注意"
    武器在背包中的贴图长宽必须是27的倍数, 这是因为紫晶的背包一格为27*27

以上这张图只针对单手武器, 如果是双手武器, 则只需要char而不需要char_left, 因为双手武器只有一种拿取方式.

准备好所有这些东西后, 你就可以再次打开 **ModShardLauncher.exe** 重复之前的编译步骤.

### 加载Mod!

!!! notice "**注意!!!**"
    有一点很重要, 你需要把工具目录下一个叫做 `ModShard.dll` 的文件移动到游戏的根目录, 他是该工具内置的一个游戏插件. 否则你将无法启动游戏!

最后一步, 也就是加载Mod了, 在你编译完mod之后, 你会发现上方的模组界面中多出了你的mod, 选择它右下角的启用. 最后点击左上角的保存按钮, 就可以把mod数据打包进你刚才加载的那个原版data.win了. 把打包好的数据随便存在什么地方, 把原版的data.win挪走, 再进入游戏, 就会提示你选择数据文件. 选择我们刚才保存的那个数据文件即可.

### 进入游戏!

打开游戏后, 你会发现打开了一个除游戏之外的窗口, 那是刚才的插件打开的, 它的作用是类似于一个控制台, 可以在游戏过程中运行一些内置函数.(如果没打开记得联系我 一定是哪里出问题了)

正常进入游戏, 插件内置了一个give函数, 你可以在插件窗口的下方 `Script` 文本框中输入
```
give MyFirstWeapon
```
来获取刚才那把mod武器.

如果你想用这个功能获取其他武器, 请注意把武器名中的空格改为 `_` , 如:
```
give Homemade_Blade
```

执行了give函数后, 可以发现背包中就多了一把mod武器了.
<center><img src="../../img/weapon_2.png" width=50%></center>