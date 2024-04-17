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
    public (int, int) SliderValues { get; }
    private int[] DropDownValues { get; } = Array.Empty<int>();
    public int DefaultValue { get; }
    public UICompoment(string name, string associatedGlobal, UICompomentType compomentType, int defaultValue = 0)
    {
        CompomentType = compomentType;
        Name = name;
        AssociatedGlobal = associatedGlobal;
        DefaultValue = defaultValue != 0 ? 1 : 0;
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
        AssociatedGlobal = associatedGlobal;
        DefaultValue = dropDownValues[0];
        switch(CompomentType)
        {
            case UICompomentType.CheckBox:
            case UICompomentType.Slider:
                throw new ValueUnavailableException();

            default:
                break;
        }
    }
    public UICompoment(string name, string associatedGlobal, UICompomentType compomentType, (int, int) sliderValues)
    {
        CompomentType = compomentType;
        SliderValues = sliderValues;
        Name = name;
        AssociatedGlobal = associatedGlobal;
        DefaultValue = SliderValues.Item1;
        switch(CompomentType)
        {
            case UICompomentType.CheckBox:
            case UICompomentType.ComboBox:
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
                AddSlider(Name, AssociatedGlobal, SliderValues);
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
    private static void AddSlider(string name, string associatedGlobal, (int, int) sliderValues)
    {
        UndertaleGameObject slider = Msl.AddObject("o_msl_component_" + name, "s_music_slide", "o_slider", isVisible:true, isAwake:true);
        Msl.AddNewEvent(
            slider, 
            @$"
target = global.{associatedGlobal}
event_inherited()
positionMin = {sliderValues.Item1}
positionMax = {sliderValues.Item2}
scr_guiInteractiveStateUpdate(id, 14, 25)
scr_guiPositionOffsetUpdate(id, math_round(target))
scr_guiLayoutOffsetUpdate(id, 0, -2)", 
            EventType.Create, 0
        );
        Msl.AddNewEvent(slider, "global.{associatedGlobal} = target", EventType.Other, 10);
        Msl.AddNewEvent(slider, "event_inherited()", EventType.Other, 11);
        Msl.AddNewEvent(slider,  "event_inherited()", EventType.Other, 25);
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
        // scr_guiCreateSlider(rightContainer, o_msl_component_{component.Name}, (depth - 1), 0, 2, make_colour_rgb(149, 121, 106), rightContainerWidth, \"{component.Name}\", {component.Slider.Item2})
        AddFunction(@"function scr_guiCreateSlider(argument0, argument1, argument2, argument3, argument4, argument5, argument6, argument7, argument8)
{
        var _container = scr_guiCreateContainer(argument0, o_guiContainerEmpty, argument2 + 1)
        scr_guiSizeUpdate(_container, argument6, 14)
        scr_guiCreateText(_container, argument2, argument3, argument4, argument7, argument5)
        with (scr_guiCreateContainer(_container, o_music_bar, argument2, 118, 2))
        {
            slider = scr_guiCreateInteractive(id, argument1, argument2)
            valueLeft = math_round(slider.target)
            valueRight = argument8
        }

        return _container;
}", "scr_guiCreateSlider");

        UndertaleGameObject menu = AddObject("o_msl_menu_" + name, "s_settings_button_down", "o_settings_tab", isVisible:true, isAwake:true);
        AddNewEvent(menu, $"event_inherited()\ntext = \"{name}\"", EventType.Create, 0);
        string other10 = @"
            event_inherited();
            ini_open(""mod.ini"");
            {0};
            ini_close();
            with (guiParent)
            {{
                scr_guiContainerSpaceUpdate(rightContainer, 10, 10, 10, 10);
                scr_guiContainerChildrenSpaceUpdate(rightContainer, 0, 15);
                {1}
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
            with (o_checkbox) 
            {{
                event_user(1)
            }}
            with (o_combobox)
            {{
                event_user(1)
            }}
            with (o_music_slider)
            {{
                event_user(0)
            }}
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
                
                case UICompomentType.Slider:
                    injectedOther10 += $"\nscr_guiCreateSlider(rightContainer, o_msl_component_{component.Name}, (depth - 1), 0, 2, make_colour_rgb(149, 121, 106), rightContainerWidth, \"{component.Name}\", {component.SliderValues.Item2})";
                break;
            }
            injectedOther12 += $"\nini_write_real(\"{name}\", \"{component.Name}\", global.{component.AssociatedGlobal})";
            injectedOther13 += $"\nglobal.{component.AssociatedGlobal} = ini_read_real(\"{name}\", \"{component.Name}\", {component.DefaultValue})";
        }
        
        AddNewEvent(menu, string.Format(other10, injectedOther13, injectedOther10), EventType.Other, 10);
        AddNewEvent(menu, string.Format(other11), EventType.Other, 11);
        AddNewEvent(menu, string.Format(other12, injectedOther12), EventType.Other, 12);
        AddNewEvent(menu, string.Format(other13, injectedOther13), EventType.Other, 13);

        LoadGML("gml_Object_o_settings_menu_Create_0")
            .Apply(x => InsertNewMenu(x, "o_msl_menu_" + name))
            .Save();
    }

    
}