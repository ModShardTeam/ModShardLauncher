using System;
using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

public class LocalizationSpeech : ILocalizationSingleTableElement
{
    /// <summary>
    /// Id of the speech
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Dictionary that contains a translation of the speech as displayed in the log for each available languages.
    /// </summary>
    public List<Dictionary<ModLanguage, string>> Speeches { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationSpeech"/> with an empty <see cref="Loc"/>.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationSpeech("mySpeechId");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    public LocalizationSpeech(string id)
    {
        Id = id;
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationSpeech"/> with <see cref="Loc"/> filled by an input dictionary.
    /// It is expected to have at least an English key. It does not need to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationSpeech("mySpeechId", 
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "speechRu"}, {English, "speechEn"}, {Italian, "speechIt"} });
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="speech"></param>
    public LocalizationSpeech(string id, params Dictionary<ModLanguage, string>[] speeches)
    {
        Id = id;
        Speeches = speeches.Select(x => Localization.SetDictionary(x)).ToList();
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationSpeech"/> with <see cref="Loc"/> filled by an input string delimited by semi-colon.
    /// It is expected to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationSpeech("mySpeechId", 
    ///     "speechRu;speechEn;speechCh");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="speech"></param>
    public LocalizationSpeech(string id, string[] speeches)
    {
        Id = id;
        Speeches = speeches.Select(x => Localization.SetDictionary(x)).ToList();
    }
    /// <summary>
    /// Create a string delimited by semi-colon that follows the in-game convention order for localization of speechs.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationSpeech("mySpeechId", "speechRu;speechEn;speechCh").CreateLine();
    /// </code>
    /// returns the string "mySpeechId;speechRu;speechEn;speechCh;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;".
    /// </example>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> CreateLine()
    {
        yield return string.Concat(Enumerable.Repeat(@$"{Id};", Msl.ModLanguageSize));

        foreach(Dictionary<ModLanguage, string> speech in Speeches)
        {
            yield return string.Concat(speech.Values.Select(x => @$"{x};"));
        }

        yield return string.Concat(Enumerable.Repeat(@$"{Id}_end;", Msl.ModLanguageSize));
    }
}
/// <summary>
/// Abstraction for carrying a list of speeches.
/// </summary>
public class LocalizationSpeeches : ILocalizationSingleTableElementCollection
{
    /// <summary>
    /// List of <see cref="LocalizationSpeech"/>
    /// </summary>
    public List<ILocalizationSingleTableElement> Locs { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationSpeech"/> with an arbitrary number of <see cref="LocalizationSpeech"/>.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationSpeech(
    ///     new LocalizationSpeech("mySpeechId1"), 
    ///     new LocalizationSpeech("mySpeechId2"));
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="speeches"></param>
    public LocalizationSpeeches(params LocalizationSpeech[] speeches)
    {
        foreach (LocalizationSpeech speech in speeches)
        {   
            Locs.Add(speech);
        }
        
    }
    public IEnumerable<string> CreateLines()
    {
        return Locs.SelectMany(x => x.CreateLine());
    }
    /// <summary>
    /// Browse a table with an iterator, and at a special line, for each <see cref="LocalizationSpeech"/>,
    /// insert a new line constructed by the dictionary <see cref="Loc"/> in the gml_GlobalScript_table_speech table. 
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public void InjectTable()
    {
        Localization.InjectTable("gml_GlobalScript_table_speech", (
                anchor:"FORBIDDEN MAGIC;",
                elements: CreateLines()
            )
        );
    }
}
public static partial class Msl
{
    /// <summary>
    /// Wrapper for the LocalizationSpeeches class
    /// </summary>
    /// <param name="speeches"></param>
    public static void InjectTableSpeechesLocalization(params LocalizationSpeech[] speeches)
    {
        LocalizationSpeeches localizationSpeeches = new(speeches);
        localizationSpeeches.InjectTable();
    }
}