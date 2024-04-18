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
    private string[] DropDownValues { get; } = Array.Empty<string>();
    public string DefaultValue { get; }
    public UICompoment(string name, string associatedGlobal, UICompomentType compomentType, string defaultValue = "")
    {
        CompomentType = compomentType;
        Name = name;
        AssociatedGlobal = associatedGlobal;
        DefaultValue = defaultValue != "" ? "1" : "0";
        switch(CompomentType)
        {
            case UICompomentType.ComboBox:
            case UICompomentType.Slider:
                throw new ValueUnavailableException();

            default:
                break;
        }
    }
    public UICompoment(string name, string associatedGlobal, UICompomentType compomentType, string[] dropDownValues)
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
        DefaultValue = SliderValues.Item1.ToString();
        switch(CompomentType)
        {
            case UICompomentType.CheckBox:
            case UICompomentType.ComboBox:
                throw new ValueUnavailableException();

            default:
                break;
        }
    }
    public void Apply(string sectionName, int index)
    {
        switch(CompomentType)
        {
            case UICompomentType.CheckBox:
                AddCheckBox(Name, AssociatedGlobal, DefaultValue, sectionName, index);
                break;
                
            case UICompomentType.ComboBox:
                AddDropDown(AssociatedGlobal, DropDownValues, index);
                break;

            case UICompomentType.Slider:
                AddSlider(Name, AssociatedGlobal, SliderValues, DefaultValue, sectionName, index);
                break;
        }
    }
    private static void AddCheckBox(string name, string associatedGlobal, string defaultValue, string sectionName, int index)
    {
        UndertaleGameObject checkbox = Msl.AddObject($"o_msl_component_{index}", "s_point", "o_checkbox", isVisible:true, isAwake:true);
        Msl.AddNewEvent(checkbox, @$"event_inherited()
ini_open(""msl_menu_mod.ini"");
global.{associatedGlobal} = ini_read_real(""{sectionName}"", ""{name}"", {defaultValue})
ini_close();
text = ""{name}""
selected = global.{associatedGlobal}", EventType.Create, 0);
        Msl.AddNewEvent(checkbox, $"event_inherited()\nglobal.{associatedGlobal} = selected", EventType.Other, 11);
    }
    private static void AddDropDown(string associatedGlobal, string[] dropDownValues, int index)
    {
        UndertaleGameObject checkbox = Msl.AddObject($"o_msl_component_{index}", "s_point", "o_combobox", isVisible:true, isAwake:true);
 
        string tmp = "";
        foreach(string value in dropDownValues)
        {
            tmp += $"\nds_list_add(optionsNamesList, \"{value}\")";
            tmp += $"\nds_list_add(optionsValuesList, \"{value}\")";
        }
        Msl.AddNewEvent(checkbox, $"global.{associatedGlobal} = ds_list_find_value(optionsValuesList, optionIndex)\nevent_user(14)", EventType.Other, 11);
        string other24 = $@"event_inherited()
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
    private static void AddSlider(string name, string associatedGlobal, (int, int) sliderValues, string defaultValue, string sectionName, int index)
    {
        UndertaleGameObject slider = Msl.AddObject($"o_msl_component_{index}", "s_music_slide", "o_music_slider", isVisible:true, isAwake:true);
        Msl.AddNewEvent(
            slider, 
            @$"ini_open(""msl_menu_mod.ini"");
global.{associatedGlobal} = ini_read_real(""{sectionName}"", ""{name}"", {defaultValue})
ini_close();
target = global.{associatedGlobal}
event_inherited()
positionMin = {sliderValues.Item1}
positionMax = {sliderValues.Item2}
scr_guiInteractiveStateUpdate(id, 14, 25)
scr_guiPositionOffsetUpdate(id, math_round(target))
scr_guiLayoutOffsetUpdate(id, 0, -2)", 
            EventType.Create, 0
        );
        Msl.AddNewEvent(slider, $"global.{associatedGlobal} = target", EventType.Other, 10);
        Msl.AddNewEvent(slider, "event_inherited()", EventType.Other, 11);
        Msl.AddNewEvent(slider,  "event_inherited()", EventType.Other, 25);
    }
}
internal class Menu
{
    public string Name { get; }
    public UICompoment[] Components { get; }
    public Menu(string name, UICompoment[] components)
    {
        Name = name;
        Components = components;
    }
}
public static partial class Msl
{
    internal static void CreateMenu(List<Menu> menus)
    {
        // needed function for sliders
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

        UndertaleGameObject menu = AddObject("o_msl_menu_mod", "s_settings_button_down", "o_settings_tab", isVisible:true, isAwake:true);
        AddNewEvent(menu, $"event_inherited()\ntext = \"MOD\"", EventType.Create, 0);

        string injectedOther10 = "";
        string injectedOther11 = "";
        string injectedOther12 = "";
        string injectedOther13 = "";

        int index = 0;
        foreach(Menu m in menus)
        {
        
            injectedOther10 += $@"_sectionContainer = scr_guiCreateContainer(rightContainer, o_guiContainerEmpty, depth, 0, 0, 2)
with (scr_guiCreateSimple(_sectionContainer, o_settings_menu_header, (depth - 1)))
{{
    text = ""{m.Name}""
    image_xscale = _headerWidth
    image_yscale = 26
    scr_guiSizeUpdate(id, image_xscale, image_yscale)
}}
_buttonsContainer = scr_guiCreateContainer(_sectionContainer, o_guiContainerEmpty, depth, 0, 0, 2)
scr_guiContainerSpaceUpdate(_buttonsContainer, 5, 0, 0, 0)
scr_guiContainerChildrenSpaceUpdate(_buttonsContainer, 0, 5)";

            injectedOther11 += $"ini_section_delete(\"{m.Name}\")";

            foreach(UICompoment component in m.Components)
            {
                component.Apply(m.Name, index);
                switch(component.CompomentType)
                {
                    case UICompomentType.CheckBox:
                        injectedOther10 += $"\nscr_guiCreateCheckbox(_buttonsContainer, o_msl_component_{index}, (depth - 1));";
                        injectedOther12 += $"\nini_write_real(\"{m.Name}\", \"{component.Name}\", global.{component.AssociatedGlobal})";
                        injectedOther13 += $"\nglobal.{component.AssociatedGlobal} = ini_read_real(\"{m.Name}\", \"{component.Name}\", {component.DefaultValue})";
                    break;

                    case UICompomentType.ComboBox:
                        injectedOther10 += $"\nscr_guiCreateCombobox(_buttonsContainer, o_msl_component_{index}, (depth - 1), 0, 0, \"{component.Name}\")";
                        injectedOther12 += $"\nini_write_string(\"{m.Name}\", \"{component.Name}\", global.{component.AssociatedGlobal})";
                        injectedOther13 += $"\nglobal.{component.AssociatedGlobal} = ini_read_string(\"{m.Name}\", \"{component.Name}\", \"{component.DefaultValue}\")";
                    break;
                    
                    case UICompomentType.Slider:
                        injectedOther10 += $"\nscr_guiCreateSlider(_buttonsContainer, o_msl_component_{index}, (depth - 1), 0, 2, make_colour_rgb(149, 121, 106), rightContainerWidth, \"{component.Name}\", {component.SliderValues.Item2})";  
                        injectedOther12 += $"\nini_write_real(\"{m.Name}\", \"{component.Name}\", global.{component.AssociatedGlobal})";
                        injectedOther13 += $"\nglobal.{component.AssociatedGlobal} = ini_read_real(\"{m.Name}\", \"{component.Name}\", {component.DefaultValue})";
                    break;
                }
                index++;
            }
        }

        string other10 = $@"event_inherited();
with (guiParent)
{{
    scr_guiContainerSpaceUpdate(rightContainer, 5, 5, 5, 5)
    scr_guiContainerChildrenSpaceUpdate(rightContainer, 0, 10)
    var _headerWidth = (rightContainerWidth - 10)
    var _sectionContainer;
    var _buttonsContainer;
    {injectedOther10}
}}
        ";
        AddNewEvent(menu, other10, EventType.Other, 10);

        string other11 = $@"event_inherited();
ini_open(""msl_menu_mod.ini"");
{injectedOther11}
ini_close();
event_user(3);
        ";
        AddNewEvent(menu, other11, EventType.Other, 11);

        string other12 = $@"event_inherited();
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
ini_open(""msl_menu_mod.ini"");
{injectedOther12}
ini_close();
        ";
        AddNewEvent(menu, other12, EventType.Other, 12);
        
        string other13 = $@"event_inherited();
ini_open(""msl_menu_mod.ini"");
{injectedOther13}
ini_close();
";
        AddNewEvent(menu, other13, EventType.Other, 13);

        LoadGML("gml_Object_o_settings_menu_Create_0")
            .Apply(x => InsertNewMenu(x, "o_msl_menu_mod"))
            .Save();
    }
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
        // add global if needed
        foreach(UICompoment component in components)
        {
            AssemblyWrapper.CheckRefVariableOrCreate(component.AssociatedGlobal, UndertaleInstruction.InstanceType.Global);
        }
        ModLoader.AddMenu(name, components);
    }

    
}