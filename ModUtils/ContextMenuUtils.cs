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
    public Dictionary<ModLanguage, string> Localisation { get; set; }
    public ContextMenu(string name, Dictionary<ModLanguage, string> localisation)
    {
        Name = name;
        Localisation = Localization.SetDictionary(localisation);
    }
    public ContextMenu(string name, string localisation)
    {
        Name = name;
        Localisation = Localization.SetDictionary(localisation);
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
    public static Func<IEnumerable<string>, IEnumerable<string>> CreateContextInjector(Dictionary<string, int> res)
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
push.s ""{x.Key}""
cmp.s.v EQ
bt [{label++}]")
            );

            label = 1000;
            string block2 = string.Join('\n', 
                res.Select(x => @$":[{label++}]
pushi.e {x.Value}
conv.i.v
pushglb.v global.context_menu
call.i ds_list_find_value(argc=2)
push.s ""{x.Key}""
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
}
public static partial class Msl
{   
    public static Dictionary<string, int> AddNewContext(params ContextMenu[] menus)
    {
        Dictionary<string, int> result = new();
        int id;

        InjectTableTextContextsLocalization(menus.Select(x => { 
            id = ++DataLoader.LastCountContext;
            result.Add(x.Name, id); 
            return x.ToLocalization(id);
        }).ToArray());

        LoadAssemblyAsString("gml_GlobalScript_scr_create_context_menu")
            .Apply(ContextMenuUtils.CreateContextInjector(result))
            .Save();
        
        return result;
    }
}