using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.RegularExpressions;
using ModShardLauncher.Mods;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;

namespace ModShardLauncher;

public class ContextMenu
{
    public string Name { get; set; }
    public int Id;
    public Dictionary<ModLanguage, string> Localisation { get; set; }
    public UndertaleFunction  ScriptFunction { get; set; }
    public UndertaleFunction? ConditionFunction { get; set; }
    public ContextMenu(string name, Dictionary<ModLanguage, string> localisation, string scriptFunction, string? conditionFunction = null)
    {
        Name = name;
        Localisation = Localization.SetDictionary(localisation);
        if (conditionFunction != null) ConditionFunction = DataLoader.data.Functions.ByName(conditionFunction);
        ScriptFunction = DataLoader.data.Functions.First(x => x.Name.Content.Contains(scriptFunction));
    }
    public ContextMenu(string name, string localisation, string scriptFunction, string? conditionFunction = null)
    {
        Name = name;
        Localisation = Localization.SetDictionary(localisation);
        try
        {
            if (conditionFunction != null) ConditionFunction = DataLoader.data.Functions.ByName(conditionFunction);
            ScriptFunction = DataLoader.data.Functions.First(x => x.Name.Content.Contains(scriptFunction));
        }
        catch
        {
            throw;
        }
    }
    public LocalizationTextContext ToLocalization(int id)
    {
        return new LocalizationTextContext(id.ToString(), Name);
    }
}
internal static class ContextMenuUtils
{
    internal static int ReadLastContextIndex()
    {
        List<UndertaleInstruction> code = Msl.GetUMTCodeFromFile("gml_GlobalScript_table_text").Instructions;
        int count = -1;
        foreach(UndertaleInstruction instruction in code.Where(x => x.Type1 == UndertaleInstruction.DataType.String))
        {
            if (instruction.Value is UndertaleResourceById<UndertaleString, UndertaleChunkSTRG> valById)
            {
                if (valById.Resource.Content.Contains("context_menu_end;"))
                {
                    count = 0;
                }
                else if (valById.Resource.Content.Contains("context_menu;"))
                {
                    return count;
                }
                else if (count >= 0)
                {
                    count++;
                }
            }
        }
        return count;
    }
    public static Func<IEnumerable<string>, IEnumerable<string>> CreateContextInjector(params ContextMenu[] res)
    {
        IEnumerable<string> func(IEnumerable<string> input)
        {
            bool fill_found = false;
            bool fill_case_found = false;
            bool jmptbl_injected = false;
            string jmp_fill = "";
            bool only_once = false;

            int label = 998;
            string block1 = string.Join('\n', 
                res.Select(x => @$"dup.v 0
push.s ""{x.Name}""
cmp.s.v EQ
bt [{label+=2}]")
            );

            label = 999;
            string block2 = string.Join('\n', 
                res.Select(x => @$":[{++label}]
call.i {x.ConditionFunction?.Name.Content ?? "gml_Script_msl_always_true"}(argc=0)
conv.v.b
bf [{++label}]
pushi.e {x.Id}
conv.i.v
pushglb.v global.context_menu
call.i ds_list_find_value(argc=2)
push.s ""{x.Name}""
conv.s.v
push.v self.context_name
call.i ds_list_add(argc=3)
popz.v
pushi.e 0
conv.i.v
pushi.e 1
conv.i.v
push.v self.context_desc
call.i ds_list_add(argc=3)
popz.v
:[{label}]
b {{0}}")
            );

            foreach(string item in input)
            {
                yield return item;

                if (!fill_found && item.Contains("Fill_Flask"))
                {
                    fill_found = true;
                }
                else if (fill_found && !jmptbl_injected && item.Contains("bt"))
                {
                    jmptbl_injected = true;
                    jmp_fill = new Regex(@"\[\d+\]").Match(item).Value;
                    
                    yield return block1;
                }
                else if (jmp_fill != "" && item.Contains(jmp_fill))
                {
                    fill_case_found = true;
                }
                else if (!only_once && fill_case_found && item.Contains("b ["))
                {
                    only_once = true;
                    string jmp_end = new Regex(@"\[\d+\]").Match(item).Value;
                    yield return string.Format(block2, jmp_end);
                }
            }
        }
        return func;
    }
    public static Func<IEnumerable<string>, IEnumerable<string>> CreateMouseInjector(params ContextMenu[] res)
    {
        IEnumerable<string> func(IEnumerable<string> input)
        {
            bool fill_found = false;
            bool fill_case_found = false;
            bool jmptbl_injected = false;
            string jmp_fill = "";
            bool only_once = false;

            int label = 1000;
            string block1 = string.Join('\n', 
                res.Select(x => @$"dup.v 0
push.s ""{x.Name}""
cmp.s.v EQ
bt [{label++}]")
            );

            label = 1000;
            string block2 = string.Join('\n', 
                res.Select(x => @$":[{label++}]
call.i {x.ScriptFunction.Name.Content}(argc=0)
popz.v
b {{0}}")
            );

            foreach(string item in input)
            {
                yield return item;

                if (!fill_found && item.Contains("Eat"))
                {
                    fill_found = true;
                }
                else if (fill_found && !jmptbl_injected && item.Contains("bt"))
                {
                    jmptbl_injected = true;
                    jmp_fill = new Regex(@"\[\d+\]").Match(item).Value;
                    
                    yield return block1;
                }
                else if (jmp_fill != "" && item.Contains(jmp_fill))
                {
                    fill_case_found = true;
                }
                else if (!only_once && fill_case_found && item.Contains("b ["))
                {
                    only_once = true;
                    string jmp_end = new Regex(@"\[\d+\]").Match(item).Value;
                    yield return string.Format(block2, jmp_end);
                }
            }
        }
        return func;
    }
}
public static partial class Msl
{   
    public static ContextMenu[] AddNewContext(params ContextMenu[] menus)
    {
        int id;

        InjectTableTextContextsLocalization(menus.Select(x => { 
            id = ++DataLoader.LastCountContext;
            x.Id = id;
            return x.ToLocalization(id);
        }).ToArray());

        LoadAssemblyAsString("gml_GlobalScript_scr_create_context_menu")
            .Apply(ContextMenuUtils.CreateContextInjector(menus))
            .Save();

        LoadAssemblyAsString("gml_Object_o_context_button_Mouse_4")
            .Apply(ContextMenuUtils.CreateMouseInjector(menus))
            .Peek()
            .Save();
        
        return menus;
    }
}