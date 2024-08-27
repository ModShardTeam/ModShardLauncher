using System;
using System.Collections.Generic;
using System.Linq;
using ModShardLauncher;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

public class LocalizationTextTree : ILocalizationElement
{
    /// <summary>
    /// Id of the modifier
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Dictionary that contains a translation of the modifier as displayed in the log for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Tier { get; set; } = new();
    public Dictionary<ModLanguage, string> Hover { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationTextTree"/> with <see cref="Loc"/> filled by an input dictionary.
    /// It is expected to have at least an English key. It does not need to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationTextTree("mySpeechId", 
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "speechRu"}, {English, "speechEn"}, {Italian, "speechIt"} });
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationTextTree(string id, Dictionary<ModLanguage, string> tier, Dictionary<ModLanguage, string> hover)
    {
        Id = id;
        Tier = Localization.SetDictionary(tier);
        Hover = Localization.SetDictionary(hover);
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationTextTree"/> with <see cref="Loc"/> filled by an input string delimited by semi-colon.
    /// It is expected to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationTextTree("mySpeechId", 
    ///     "speechRu;speechEn;speechCh");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationTextTree(string id, string tier, string hover)
    {
        Id = id;
        Tier = Localization.SetDictionary(tier);
        Hover = Localization.SetDictionary(hover);
    }
    /// <summary>
    /// Create a string delimited by semi-colon that follows the in-game convention order for localization of speechs.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationTextTree("mySpeechId", "speechRu;speechEn;speechCh").CreateLine();
    /// </code>
    /// returns the string "mySpeechId;speechRu;speechEn;speechCh;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;".
    /// </example>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> CreateLine(string? selector)
    {
        switch(selector)
        {
        case "tier":
        yield return $"{Id};{string.Concat(Tier.Values.Select(x => @$"{x};"))}";
            break;
        case "hover":
        yield return $"{Id};{string.Concat(Hover.Values.Select(x => @$"{x};"))}";
            break;
        }
    }
}
public class LocalizationTextRarity : ILocalizationElement
{
    /// <summary>
    /// Id of the modifier
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Dictionary that contains a translation of the modifier as displayed in the log for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Name { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationTextRarity"/> with <see cref="Loc"/> filled by an input dictionary.
    /// It is expected to have at least an English key. It does not need to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationTextRarity("mySpeechId", 
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "speechRu"}, {English, "speechEn"}, {Italian, "speechIt"} });
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationTextRarity(string id, Dictionary<ModLanguage, string> name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationTextRarity"/> with <see cref="Loc"/> filled by an input string delimited by semi-colon.
    /// It is expected to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationTextRarity("mySpeechId", 
    ///     "speechRu;speechEn;speechCh");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationTextRarity(string id, string name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
    }
    /// <summary>
    /// Create a string delimited by semi-colon that follows the in-game convention order for localization of speechs.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationTextTree("mySpeechId", "speechRu;speechEn;speechCh").CreateLine();
    /// </code>
    /// returns the string "mySpeechId;speechRu;speechEn;speechCh;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;".
    /// </example>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> CreateLine(string? selector)
    {
        yield return $"{Id};{string.Concat(Name.Values.Select(x => @$"{x};"))}";
    }
}
public class LocalizationTextContext : ILocalizationElement
{
    /// <summary>
    /// Id of the modifier
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Dictionary that contains a translation of the modifier as displayed in the log for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Name { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationTextContext"/> with <see cref="Loc"/> filled by an input dictionary.
    /// It is expected to have at least an English key. It does not need to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationTextContext("mySpeechId", 
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "speechRu"}, {English, "speechEn"}, {Italian, "speechIt"} });
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationTextContext(string id, Dictionary<ModLanguage, string> name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationTextContext"/> with <see cref="Loc"/> filled by an input string delimited by semi-colon.
    /// It is expected to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationTextContext("mySpeechId", 
    ///     "speechRu;speechEn;speechCh");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationTextContext(string id, string name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
    }
    /// <summary>
    /// Create a string delimited by semi-colon that follows the in-game convention order for localization of speechs.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationTextTree("mySpeechId", "speechRu;speechEn;speechCh").CreateLine();
    /// </code>
    /// returns the string "mySpeechId;speechRu;speechEn;speechCh;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;".
    /// </example>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> CreateLine(string? selector)
    {
        yield return $"{Id};{string.Concat(Name.Values.Select(x => @$"{x};"))}";
    }
}
// TODO : psychic injection
public static partial class Msl
{
    /// <summary>
    /// Wrapper for the LocalizationTextTrees class
    /// </summary>
    /// <param tier="modifiers"></param>
    public static void InjectTableTextTreesLocalization(params ILocalizationElement[] modifiers)
    {
        LocalizationBaseTable localizationBaseTable = new("gml_GlobalScript_table_text",
            ("Tier_name;", "tier"), ("skilltree_hover;", "hover")
        );
        localizationBaseTable.InjectTable(modifiers.ToList());
    }
    public static void InjectTableTextRaritysLocalization(params ILocalizationElement[] modifiers)
    {
        LocalizationBaseTable localizationBaseTable = new("gml_GlobalScript_table_text",
            ("rarity;", null)
        );
        localizationBaseTable.InjectTable(modifiers.ToList());
    }
    public static void InjectTableTextContextsLocalization(params ILocalizationElement[] modifiers)
    {
        LocalizationBaseTable localizationBaseTable = new("gml_GlobalScript_table_text",
            ("context_menu;", null)
        );
        localizationBaseTable.InjectTable(modifiers.ToList());
    }
}