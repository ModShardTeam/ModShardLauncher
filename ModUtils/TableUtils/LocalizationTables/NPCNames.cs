using System;
using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

/// <summary>
/// Abstraction the localization of items found in gml_GlobalScript_table_consumables.
/// </summary>
public class LocalizationName : ILocalizationElement
{
    /// <summary>
    /// Dictionary that contains a translation of the item name as displayed in-game for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Name { get; set; } = new();
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
    public LocalizationName(Dictionary<ModLanguage, string> name)
    {
        Name = Localization.SetDictionary(name);
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
    public LocalizationName(string name)
    {
        Name = Localization.SetDictionary(name);
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
        yield return $";{string.Concat(Name.Values.Select(x => @$"{x};"))}";
    }
}
public class LocalizationQuestName : ILocalizationElement
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
    public LocalizationQuestName(string id, Dictionary<ModLanguage, string> name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
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
    public LocalizationQuestName(string id, string name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
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
        yield return $"{Id};{string.Concat(Name.Values.Select(x => @$"{x};"))}";
    }
}
public class LocalizationOccupationName : ILocalizationElement
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
    public LocalizationOccupationName(string id, Dictionary<ModLanguage, string> name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
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
    public LocalizationOccupationName(string id, string name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
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
        yield return $"{Id};{string.Concat(Name.Values.Select(x => @$"{x};"))}";
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
    public static Func<IEnumerable<string>, IEnumerable<string>> CreateInjectionNamesLocalization(params LocalizationName[] names)
    {
        LocalizationBaseTable localizationBaseTable = new(
            ("Names;", "name")
        );
        return localizationBaseTable.CreateInjectionTable(names.Select(x => x as ILocalizationElement).ToList());
    }
    public static void InjectTableNamesLocalization(params LocalizationName[] names)
    {
        Localization.InjectTable("gml_GlobalScript_table_NPC_names", CreateInjectionNamesLocalization(names));
    }
    public static Func<IEnumerable<string>, IEnumerable<string>> CreateInjectionQuestNamesLocalization(params LocalizationQuestName[] questNames)
    {
        LocalizationBaseTable localizationBaseTable = new(
            ("Constant_Name;", "name")
        );
        return localizationBaseTable.CreateInjectionTable(questNames.Select(x => x as ILocalizationElement).ToList());
    }
    public static void InjectTableQuestNamesLocalization(params LocalizationQuestName[] questNames)
    {
        Localization.InjectTable("gml_GlobalScript_table_NPC_names", CreateInjectionQuestNamesLocalization(questNames));
    }
    public static Func<IEnumerable<string>, IEnumerable<string>> CreateInjectionOccupationNamesLocalization(params LocalizationOccupationName[] occupationNames)
    {
        LocalizationBaseTable localizationBaseTable = new(
            ("NPC_info;", "name")
        );
        return localizationBaseTable.CreateInjectionTable(occupationNames.Select(x => x as ILocalizationElement).ToList());
    }
    public static void InjectTableOccupationNamesLocalization(params LocalizationOccupationName[] occupationNames)
    {
        Localization.InjectTable("gml_GlobalScript_table_NPC_names", CreateInjectionOccupationNamesLocalization(occupationNames));
    }
}