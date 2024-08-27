using System;
using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

public class LocalizationModifier : ILocalizationMultiTableElement
{
    /// <summary>
    /// Id of the modifier
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Dictionary that contains a translation of the modifier as displayed in the log for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Name { get; set; } = new();
    public Dictionary<ModLanguage, string> Description { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationModifier"/> with an empty <see cref="Loc"/>.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationModifier("mySpeechId");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    public LocalizationModifier(string id)
    {
        Id = id;
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationModifier"/> with <see cref="Loc"/> filled by an input dictionary.
    /// It is expected to have at least an English key. It does not need to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationModifier("mySpeechId", 
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "speechRu"}, {English, "speechEn"}, {Italian, "speechIt"} });
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationModifier(string id, Dictionary<ModLanguage, string> name, Dictionary<ModLanguage, string> description)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
        Description = Localization.SetDictionary(description);
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationModifier"/> with <see cref="Loc"/> filled by an input string delimited by semi-colon.
    /// It is expected to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationModifier("mySpeechId", 
    ///     "speechRu;speechEn;speechCh");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationModifier(string id, string name, string description)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
        Description = Localization.SetDictionary(description);
    }
    /// <summary>
    /// Create a string delimited by semi-colon that follows the in-game convention order for localization of speechs.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationModifier("mySpeechId", "speechRu;speechEn;speechCh").CreateLine();
    /// </code>
    /// returns the string "mySpeechId;speechRu;speechEn;speechCh;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;".
    /// </example>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> CreateLine(string selector)
    {
        switch(selector)
        {
        case "name":
        yield return string.Concat(Name.Values.Select(x => @$"{x};"));
            break;
        case "description":
        yield return string.Concat(Description.Values.Select(x => @$"{x};"));
            break;
        }
    }
}
/// <summary>
/// Abstraction for carrying a list of modifiers.
/// </summary>
public class LocalizationModifiers : ILocalizationMultiTableElementCollection
{
    /// <summary>
    /// List of <see cref="LocalizationModifier"/>
    /// </summary>
    public List<ILocalizationMultiTableElement> Locs { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationModifier"/> with an arbitrary number of <see cref="LocalizationModifier"/>.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationModifier(
    ///     new LocalizationModifier("mySpeechId1"), 
    ///     new LocalizationModifier("mySpeechId2"));
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="modifiers"></param>
    public LocalizationModifiers(params LocalizationModifier[] modifiers)
    {
        foreach (LocalizationModifier modifier in modifiers)
        {   
            Locs.Add(modifier);
        }
    }
    public IEnumerable<string> CreateLines(string selector)
    {
        return Locs.SelectMany(x => x.CreateLine(selector));
    }
    /// <summary>
    /// Browse a table with an iterator, and at a special line, for each <see cref="LocalizationModifier"/>,
    /// insert a new line constructed by the dictionary <see cref="Loc"/> in the gml_GlobalScript_table_speech table. 
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public void InjectTable()
    {
        Localization.InjectTable("gml_GlobalScript_table_Modifiers", 
            (
                anchor:"buff_name;\",",
                elements: CreateLines("name")
            ),
            (
                anchor:"buff_desc;\",",
                elements: CreateLines("description")
            )
        );
    }
}
public static partial class Msl
{
    /// <summary>
    /// Wrapper for the LocalizationSpeeches class
    /// </summary>
    /// <param name="modifiers"></param>
    public static void InjectTableSpeechesLocalization(params LocalizationModifier[] modifiers)
    {
        LocalizationModifiers localizationModifiers = new(modifiers);
        localizationModifiers.InjectTable();
    }
}