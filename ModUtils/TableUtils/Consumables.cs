using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

/// <summary>
/// Abstraction the localization of items found in gml_GlobalScript_table_consumables.
/// </summary>
public class LocalizationItem
{
    /// <summary>
    /// Name of the object in the localization table.
    /// </summary>
    public string OName { get; set; }
    /// <summary>
    /// Dictionary that contains a translation of the item name as displayed in-game for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> ConsumableName { get; set; } = new();
    /// <summary>
    /// Dictionary that contains a translation of the item effect as displayed in-game for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> ConsumableID { get; set; } = new();
    /// <summary>
    /// Dictionary that contains a translation of the item description as displayed in-game for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> ConsumableDescription { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationItem"/> with empty <see cref="ConsumableName"/>, <see cref="ConsumableID"/> and <see cref="ConsumableDescription"/>.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationItem("myTestItem");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="oName"></param>
    public LocalizationItem(string oName)
    {
        OName = oName;
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationItem"/> with <see cref="ConsumableName"/>, <see cref="ConsumableID"/> and <see cref="ConsumableDescription"/> filled by input dictionaries.
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
    /// <param name="oName"></param>
    /// <param name="dictName"></param>
    /// <param name="dictID"></param>
    /// <param name="dictDescription"></param>
    public LocalizationItem(string oName, Dictionary<ModLanguage, string> dictName, Dictionary<ModLanguage, string> dictID, Dictionary<ModLanguage, string> dictDescription)
    {
        OName = oName;
        ConsumableName = Localization.SetDictionary(dictName);
        ConsumableID = Localization.SetDictionary(dictID);
        ConsumableDescription = Localization.SetDictionary(dictDescription);
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationItem"/> with <see cref="ConsumableName"/>, <see cref="ConsumableID"/> and <see cref="ConsumableDescription"/> filled by input strings delimited by semi-colon.
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
    /// <param name="oName"></param>
    /// <param name="valuesName"></param>
    /// <param name="valuesID"></param>
    /// <param name="valuesDescription"></param>
    public LocalizationItem(string oName, string valuesName, string valuesID, string valuesDescription)
    {
        OName = oName;
        ConsumableName = Localization.SetDictionary(valuesName);
        ConsumableID = Localization.SetDictionary(valuesID);
        ConsumableDescription = Localization.SetDictionary(valuesDescription);
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
    /// <param name="oName"></param>
    /// <param name="dict"></param>
    /// <returns></returns>
    static private string CreateLine(string oName, Dictionary<ModLanguage, string> dict)
    {
        string line = oName;
        foreach (KeyValuePair<ModLanguage, string> kp in dict)
        {
            line += ";";
            line += kp.Value;
        }
        return line + ";//;";
    }
    /// <summary>
    /// Browse a table with an iterator, and at special lines, yield a new line constructed by the dictionaries <see cref="ConsumableName"/>, <see cref="ConsumableID"/> and <see cref="ConsumableDescription"/>.
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private IEnumerable<string> EditTable(IEnumerable<string> table)
    {
        foreach (string line in table)
        {
            if (line.Contains("consum_name_end"))
            {
                yield return CreateLine(OName, ConsumableName);
            }
            else if (line.Contains("consum_mid_end"))
            {
                yield return CreateLine(OName, ConsumableID);
            }
            else if (line.Contains("consum_desc_end"))
            {
                yield return CreateLine(OName, ConsumableDescription);
            }
            yield return line;
        }
    }
    /// <summary>
    /// Browse a table with an iterator, and at special lines, 
    /// insert a new line constructed by the dictionaries <see cref="ConsumableName"/>, <see cref="ConsumableID"/> and <see cref="ConsumableDescription"/> in the gml_GlobalScript_table_consumables table.
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public void InjectTable()
    {
        List<string> table = Msl.ThrowIfNull(ModLoader.GetTable("gml_GlobalScript_table_consumables"));
        ModLoader.SetTable(EditTable(table).ToList(), "gml_GlobalScript_table_consumables");
    }
}
public partial class Msl
{
    /// <summary>
    /// Wrapper for the LocalizationItem class using dictionnaries
    /// </summary>
    /// <param name="oName"></param>
    /// <param name="dictName"></param>
    /// <param name="dictID"></param>
    /// <param name="dictDescription"></param>
    public static void InjectTableItemLocalization(string oName, Dictionary<ModLanguage, string> dictName, Dictionary<ModLanguage, string> dictID, Dictionary<ModLanguage, string> dictDescription)
    {
        LocalizationItem localizationItem = new(oName, dictName, dictID, dictDescription);
        localizationItem.InjectTable();
    }
    /// <summary>
    /// Wrapper for the LocalizationItem class using strings
    /// </summary>
    /// <param name="oName"></param>
    /// <param name="valuesName"></param>
    /// <param name="valuesID"></param>
    /// <param name="valuesDescription"></param>
    public static void InjectTableItemLocalization(string oName, string valuesName, string valuesID, string valuesDescription)
    {
        LocalizationItem localizationItem = new(oName, valuesName, valuesID, valuesDescription);
        localizationItem.InjectTable();
    }
}