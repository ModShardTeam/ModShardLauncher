# API

该页面介绍了本工具可以使用的API.

[TOC]

## ModLoader

### ModLoader.AddObject(string name)

根据给出的名称, 向数据文件中添加一个Game Object, 并返回这个Object.

### ModLoader.GetObject(string name)

根据给出的名称, 从数据文件中查找一个Game Object, 并返回这个Object. 如果找不到则返回null.

### ModLoader.SetObject(string name, UndertaleGameObject o)

根据给出的名称和Object, 给数据文件中对应的Game Object赋值.

### ModLoader.AddCode(string Code, string name)

根据给出的名称和Code, 向数据文件中添加一个Code, 并返回这个Code.
!!! notice ""
    注意不要使用AddCode来添加新的function, 若想添加新的function, 请使用AddFunction.

### ModLoader.AddFunction(string Code, string name)

根据给出的名称和Code, 向数据文件中添加一个包含Function的Code, 并返回这个Code
!!! notice ""
    请使用本方法添加Function, 否则数据文件将损坏.

### ModLoader.GetTable(string name)

根据给出的名称, 从数据文件中查找一个Table, 将其转换为List<string\>并返回这个List.

### GetDecompiledCode(string name)

根据给出的名称, 从数据文件中查找一个Code, 并返回这个Code的Decompile版本代码.

### GetDisassemblydCode(string name)

根据给出的名称, 从数据文件中查找一个Code, 并返回这个Code的Disassembly版本代码.(说实话, 我并不知道这个方法有什么卵用)