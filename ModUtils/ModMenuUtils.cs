using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace ModShardLauncher;

public enum UICompomentType
{
    ComboBox,
    CheckBox,
    Slider,
}
public class UICompoment
{
    public UICompomentType CompomentType { get; }
    public string Name { get; }
    public string AssociatedGlobal { get; }
    private (int, int, int) sliderValues;
    private int[] DropDownValues { get; } = Array.Empty<int>();
    public UICompoment(string name, string associatedGlobal, UICompomentType compomentType)
    {
        CompomentType = compomentType;
        Name = name;
        this.AssociatedGlobal = associatedGlobal;
        switch(CompomentType)
        {
            case UICompomentType.ComboBox:
            case UICompomentType.Slider:
                throw new ValueUnavailableException();

            default:
                break;
        }
    }
    public UICompoment(string name, string associatedGlobal, UICompomentType compomentType, int[] dropDownValues)
    {
        CompomentType = compomentType;
        DropDownValues = dropDownValues;
        Name = name;
        this.AssociatedGlobal = associatedGlobal;
        switch(CompomentType)
        {
            case UICompomentType.CheckBox:
            case UICompomentType.Slider:
                throw new ValueUnavailableException();

            default:
                break;
        }
    }
    public void Apply()
    {
        switch(CompomentType)
        {
            case UICompomentType.CheckBox:
                AddCheckBox(Name, AssociatedGlobal);
                break;
                
            case UICompomentType.ComboBox:
                AddDropDown(Name, AssociatedGlobal, DropDownValues);
                break;

            case UICompomentType.Slider:
                break;
        }
    }
    private static void AddCheckBox(string name, string associatedGlobal)
    {
        UndertaleGameObject checkbox = Msl.AddObject("o_msl_component_" + name, "s_point", "o_checkbox", isVisible:true, isAwake:true);
        Msl.AddNewEvent(checkbox, $"event_inherited()\ntext = \"{name}\"\nselected = global.{associatedGlobal}", EventType.Create, 0);
        Msl.AddNewEvent(checkbox, $"event_inherited()\nglobal.{associatedGlobal} = selected", EventType.Other, 11);
    }
    private static void AddDropDown(string name, string associatedGlobal, int[] dropDownValues)
    {
        UndertaleGameObject checkbox = Msl.AddObject("o_msl_component_" + name, "s_point", "o_combobox", isVisible:true, isAwake:true);
 
        string tmp = "";
        foreach(int value in dropDownValues)
        {
            tmp += $"\nds_list_add(optionsNamesList, {value})";
            tmp += $"\nds_list_add(optionsValuesList, {value})";
        }
        Msl.AddNewEvent(checkbox, $"global.{associatedGlobal} = ds_list_find_value(optionsValuesList, optionIndex)\nevent_user(14)", EventType.Other, 11);
        string other24 = $@"
            event_inherited()
            {tmp}
            if (variable_global_exists(""{associatedGlobal}""))
            {{
                optionIndex = ds_list_find_index(optionsValuesList, global.{associatedGlobal})
            }}
            if ((optionIndex == -1))
            {{
                optionIndex = 0;
                global.{associatedGlobal} = ds_list_find_value(optionsValuesList, 0)
            }}
        ";
        Msl.AddNewEvent(checkbox, other24, EventType.Other, 24);
    }
}
public static partial class Msl
{
    private static IEnumerable<string> InsertNewMenu(IEnumerable<string> lines, string name)
    {
        foreach(string line in lines)
        {
            if (line.Contains("var _tabButtonsArray = "))
            {
                yield return line[..line.IndexOf("]")] + $", {name}];";
            }
            else
            {
                yield return line;
            }
        }
    }
    public static void AddMenu(string name, params UICompoment[] components)
    {
        UndertaleGameObject menu = AddObject("o_msl_menu_" + name, "s_settings_button_down", "o_settings_tab", isVisible:true, isAwake:true);
        AddNewEvent(menu, $"event_inherited()\ntext = \"{name}\"", EventType.Create, 0);
        string other10 = @"
            event_inherited();
            with (guiParent)
            {{
                scr_guiContainerSpaceUpdate(rightContainer, 10, 10, 10, 10);
                scr_guiContainerChildrenSpaceUpdate(rightContainer, 0, 15);
                {0}
            }}
        ";
        string injectedOther10 = "";

        string other11 = $@"
            event_inherited();
            ini_open(""mod.ini"");
            ini_section_delete(""{name}"");
            ini_close();
            event_user(3);
        ";

        string other12 = @"
            event_inherited();
            ini_open(""mod.ini"");
            {0};
            ini_close();
        ";
        string injectedOther12 = "";

        string other13 = @"
            event_inherited();
            ini_open(""mod.ini"");
            {0};
            ini_close();
        ";
        string injectedOther13 = "";

        foreach(UICompoment component in components)
        {
            component.Apply();
            switch(component.CompomentType)
            {
                case UICompomentType.CheckBox:
                    injectedOther10 += $"\nscr_guiCreateCheckbox(rightContainer, o_msl_component_{component.Name}, (depth - 1));";
                break;

                case UICompomentType.ComboBox:
                    injectedOther10 += $"\nscr_guiCreateCombobox(rightContainer, o_msl_component_{component.Name}, (depth - 1), 0, 0, \"{component.Name}\")";
                break;
            }
            injectedOther12 += $"\nini_write_real(\"{name}\", \"{component.Name}\", global.{component.AssociatedGlobal})";
            injectedOther13 += $"\nglobal.{component.AssociatedGlobal} = ini_read_real(\"{name}\", \"{component.Name}\", 1)";
        }
        
        AddNewEvent(menu, string.Format(other10, injectedOther10), EventType.Other, 10);
        AddNewEvent(menu, string.Format(other11), EventType.Other, 11);
        AddNewEvent(menu, string.Format(other12, injectedOther12), EventType.Other, 12);
        AddNewEvent(menu, string.Format(other13, injectedOther13), EventType.Other, 13);

        LoadGML("gml_Object_o_settings_menu_Create_0")
            .Apply(x => InsertNewMenu(x, "o_msl_menu_" + name))
            .Save();
    }

    
}