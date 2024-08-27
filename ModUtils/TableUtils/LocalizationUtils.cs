using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ModShardLauncher.Mods;
using Serilog;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;

namespace ModShardLauncher;

/// <summary>
/// Utility class for localization.
/// </summary>
static public class Localization
{
    /// <summary>
    /// Immutable List of all possible languages defined by ModLanguage
    /// </summary>
    static public readonly ImmutableList<ModLanguage> LanguageList = Enum.GetValues(typeof(ModLanguage)).Cast<ModLanguage>().ToImmutableList();
    /// <summary>
    /// Convert a string delimited by semi-colons into a dictionnary of ModLanguage.
    /// It assumes the string follows the game convention order (first Russian, then English, and so on), and will use by default the second language (English) as the default one.
    /// You can change the default language by changing <paramref name="indexDefault"/>.
    /// <example>
    /// For example:
    /// <code>
    /// Localization.ToDict("test");
    /// </code>
    /// returns the dictionnary { Russian: "test", English: "test", Chinese: "test", German: "test", Spanish: "test", French: "test", Italian: "test", Portuguese: "test", Polish: "test", Turkish: "test", Japanese: "test", Korean: "test"}.
    /// <code>
    /// Localization.ToDict("testRu;testEn;testCh");
    /// </code>
    /// returns the dictionnary { Russian: "testRu", English: "testEn", Chinese: "testCh", German: "testEn", Spanish: "testEn", French: "testEn", Italian: "testEn", Portuguese: "testEn", Polish: "testEn", Turkish: "testEn", Japanese: "testEn", Korean: "testEn"}.
    /// </example>
    /// </summary>
    /// <param name="values"></param>
    /// <param name="indexDefault"></param>
    /// <returns></returns>
    static private Dictionary<ModLanguage, string> ToDict(string values, int indexDefault = 1)
    {
        string[] strings = values.Split(";");
        Dictionary<ModLanguage, string> tmp = LanguageList.Zip(strings).ToDictionary(x => x.First, x => x.Second);

        if (strings.Length <= indexDefault) indexDefault = 0;
        string defaultName = strings[indexDefault];

        foreach (ModLanguage language in LanguageList)
        {
            if (!tmp.ContainsKey(language))
            {
                tmp[language] = defaultName;
            }
        }

        return tmp;
    }
    /// <summary>
    /// Fill a destination ModLanguage dictionary with the values contained in a source dictionary.
    /// The source dictionary is assumed to always have an English key which will be used as default if there is missing values in the source dictionary.
    /// <example>
    /// For example:
    /// <code>
    /// Localization.SetDictionary(new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "testRu"} , {English, "testEn"}, {Italian, "testIt"} });
    /// </code>
    /// returns the dictionnary { Russian: "testRu", English: "testEn", Chinese: "testEn", German: "testEn", Spanish: "testEn", French: "testEn", Italian: "testIt", Portuguese: "testEn", Polish: "testEn", Turkish: "testEn", Japanese: "testEn", Korean: "testEn"}.
    /// </example>
    /// </summary>
    /// <param name="source"></param>
    static public Dictionary<ModLanguage, string> SetDictionary(Dictionary<ModLanguage, string> source)
    {
        Dictionary<ModLanguage, string> dest = new();
        string englishName = source[ModLanguage.English];
        foreach (ModLanguage language in LanguageList)
        { 
            if (!source.ContainsKey(language))
            {
                dest[language] = englishName;
            }
            else
            {
                dest[language] = source[language];
            }
        }
        return dest;
    }
    /// <summary>
    /// Fill a destination ModLanguage dictionary with the values contained in a source string delimited by semi-colon. 
    /// If no semi-colon are found, it assumes there is only one language filled out, so every keys are set with the same value.
    /// Else it uses the <see cref="ToDict"/> function to create a dictionary from the string.
    /// <example>
    /// For example:
    /// <code>
    /// Localization.SetDictionary("testRu;testEn;testCh;testGe");
    /// </code>
    /// returns the dictionnary { Russian: "testRu", English: "testEn", Chinese: "testCh", German: "testGe", Spanish: "testEn", French: "testEn", Italian: "testEn", Portuguese: "testEn", Polish: "testEn", Turkish: "testEn", Japanese: "testEn", Korean: "testEn"}.
    /// </example>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    static public Dictionary<ModLanguage, string> SetDictionary(string source)
    {
        Dictionary<ModLanguage, string> dest = new();
        if (source.Contains(';'))
        {
            dest = ToDict(source);
        } 
        else
        {
            foreach (ModLanguage language in LanguageList)
            {
                dest[language] = source;
            }
        }
        return dest;
    }
    static public void InjectTable(string tableName, params (string anchor, IEnumerable<string> elements)[] datas)
    {
        IEnumerable<string> func(IEnumerable<string> input)
        {
            foreach (string item in input)
            {
                foreach(string element in datas.Where(_ => item.Contains(_.anchor)).SelectMany(_ => _.elements).Reverse())
                {
                    yield return $"push.s \"{element}\"";
                    yield return "conv.s.v";
                }
                
                yield return item;
            }
        }

        Msl.LoadAssemblyAsString(tableName)
            .Apply(func)
            .Save();
    }
}
public interface ILocalizationElement
{
    IEnumerable<string> CreateLine(string? selector);
}
public class LocalizationBaseTable
{
    public string TableName;
    public List<(string anchor, string? selector)> Anchors = new();
    public LocalizationBaseTable(string tableName, params (string, string?)[] anchors)
    {
        TableName = tableName;
        foreach ((string, string?) anchor in anchors)
        {   
            Anchors.Add(anchor);
        }
    }
    public static IEnumerable<string> CreateLines(List<ILocalizationElement> Locs, string? selector)
    {
        return Locs.SelectMany(x => x.CreateLine(selector));
    }
    public void InjectTable(List<ILocalizationElement> Locs)
    {
        Localization.InjectTable(
            TableName, 
            Anchors.Select(x => (x.anchor, CreateLines(Locs, x.selector))).ToArray()
        );
    }
}
public partial class Msl
{  
    static public void ExportTable(string tableName, string outputName)
    {
        DirectoryInfo dir = new (DataLoader.exportPath);
        if (!dir.Exists) dir.Create();

        try
        {
            UndertaleCode code = GetUMTCodeFromFile(tableName); // can fail InvalidOperationException

            string table = code.Disassemble(ModLoader.Data.Variables, ModLoader.Data.CodeLocals.For(code));
            IEnumerable<System.Text.RegularExpressions.Match> matches = Regex.Matches(table, @"push.s ""(.+)""@\d+").Reverse();

            using var stream = File.OpenWrite(Path.Join(dir.FullName, outputName));
            using StreamWriter writer = new(stream);
            foreach(System.Text.RegularExpressions.Match match in matches)
            {
                writer.WriteLine(match.Groups[1].Value);
            }
        }
        catch (InvalidOperationException)
        {
            Log.Warning($"{tableName} is not a valid table.");
        }
    }
}