using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

/// <summary>
/// Abstraction the localization of items found in gml_GlobalScript_table_consumables.
/// </summary>
public class LocalizationItem : ILocalizationMultiTableElement
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
    /// Return an instance of <see cref="LocalizationItem"/> with empty <see cref="Name"/>, <see cref="Effect"/> and <see cref="Description"/>.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationItem("myTestItem");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    public LocalizationItem(string id)
    {
        Id = id;
    }
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
    public IEnumerable<string> CreateLine(string selector)
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
public class LocalizationItems : ILocalizationMultiTableElementCollection
{
    /// <summary>
    /// List of <see cref="LocalizationItem"/>
    /// </summary>
    public List<ILocalizationMultiTableElement> Locs { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationItem"/> with an arbitrary number of <see cref="LocalizationItem"/>.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationItem(
    ///     new LocalizationItem("mySpeechId1"), 
    ///     new LocalizationItem("mySpeechId2"));
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="modifiers"></param>
    public LocalizationItems(params LocalizationItem[] items)
    {
        foreach (LocalizationItem modifier in items)
        {   
            Locs.Add(modifier);
        }
    }
    public IEnumerable<string> CreateLines(string selector)
    {
        return Locs.SelectMany(x => x.CreateLine(selector));
    }
    /// <summary>
    /// Browse a table with an iterator, and at a special line, for each <see cref="LocalizationItem"/>,
    /// insert a new line constructed by the dictionary <see cref="Loc"/> in the gml_GlobalScript_table_speech table. 
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public void InjectTable()
    {
        Localization.InjectTable("gml_GlobalScript_table_consumables", 
            (
                anchor:"consum_name;",
                elements: CreateLines("name")
            ),
            (
                anchor:"consum_mid;",
                elements: CreateLines("effect")
            ),
            (
                anchor:"consum_desc;",
                elements: CreateLines("description")
            )
        );
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
    public static void InjectTableItemsLocalization(string id, params LocalizationItem[] items)
    {
        LocalizationItems localizationItems = new(items);
        localizationItems.InjectTable();
    }
}