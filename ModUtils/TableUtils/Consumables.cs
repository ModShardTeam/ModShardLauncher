using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

/// <summary>
/// Abstraction the localization of items found in gml_GlobalScript_table_consumables.
/// </summary>
public class LocalizationItem : ILocalizationElement
{
    /// <summary>
    /// Name of the object in the localization table.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Dictionary that contains a translation of the item name as displayed in-game for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Name { get; set; } = new();
    /// <summary>
    /// Dictionary that contains a translation of the item effect as displayed in-game for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Effect { get; set; } = new();
    /// <summary>
    /// Dictionary that contains a translation of the item description as displayed in-game for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Description { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationItem"/> with <see cref="Name"/>, <see cref="Effect"/> and <see cref="Description"/> filled by input dictionaries.
    /// It is expected to have at least an English key for each dictionary. It does not need to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationItem("myTestItem", 
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "testRu"}, {English, "testEn"}, {Italian, "testIt"} },
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "effectRu"}, {English, "effectEn"}, {Italian, "effectIt"} },
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "descRu"}, {English, "descEn"}, {Italian, "descIt"} } );
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// <param name="description"></param>
    public LocalizationItem(string id, Dictionary<ModLanguage, string> name, Dictionary<ModLanguage, string> effect, Dictionary<ModLanguage, string> description)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
        Effect = Localization.SetDictionary(effect);
        Description = Localization.SetDictionary(description);
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationItem"/> with <see cref="Name"/>, <see cref="Effect"/> and <see cref="Description"/> filled by input strings delimited by semi-colon.
    /// It is expected to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationItem("myTestItem", 
    ///     "testRu;testEn;testCh",
    ///     "effectRu;effectEn;effectCh",
    ///     "descRu;descEn;descIt");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// <param name="description"></param>
    public LocalizationItem(string id, string name, string effect, string description)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
        Effect = Localization.SetDictionary(effect);
        Description = Localization.SetDictionary(description);
    }
    /// <summary>
    /// Create a string delimited by semi-colon that follows the in-game convention order for localization of items.
    /// <example>
    /// For example:
    /// <code>
    /// CreateLine("testItem", new Dictionary &lt; ModLanguage, string &gt; () {{Russian, "testRu"}, {English, "testEn"}, {Chinese, "testCh"}, {German, "testGe"}, {Spanish, "testSp"}, 
    /// {French, "testFr"}, {Italian, "testIt"}, {Portuguese, "testPr"}, {Polish, "testPl"}, {Turkish, "testTu"}, {Japanese, "testJp"}, {Korean, "testKr"}} );
    /// </code>
    /// returns the string "testItem;testRu;testEn;testCh;testGe;testSp;testFr;testIt;testPr;testPl;testTu;testJp;testKr;//;".
    /// </example>
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    public IEnumerable<string> CreateLine(string? selector)
    {
        switch(selector)
        {
        case "name":
        yield return $"{Id};{string.Concat(Name.Values.Select(x => @$"{x};"))}//;";
            break;
        case "effect":
        yield return $"{Id};{string.Concat(Effect.Values.Select(x => @$"{x};"))}//;";
            break;
        case "description":
        yield return $"{Id};{string.Concat(Description.Values.Select(x => @$"{x};"))}//;";
            break;
        }
    }
}
public partial class Msl
{
    /// <summary>
    /// Wrapper for the LocalizationItem class using dictionnaries
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// <param name="description"></param>
    public static void InjectTableItemsLocalization(string id, params ILocalizationElement[] items)
    {
        LocalizationBaseTable localizationBaseTable = new("gml_GlobalScript_table_consumables",
            ("consum_name;", "name"), ("consum_mid;", "effect"), ("consum_desc;", "description")
        );
        localizationBaseTable.InjectTable(items.ToList());
    }
}