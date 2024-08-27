using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

/// <summary>
/// Abstraction for the localization of sentences found in gml_GlobalScript_table_NPC_Lines.
/// </summary>
public class LocalizationSentence : ILocalizationElement
{
    /// <summary>
    /// Id of the sentence
    /// </summary>
    public string Id { get; set; }
    public string Tags { get; set; } = "any";
    public string Role { get; set; } = "any";
    public string Type { get; set; } = "any";
    public string Faction { get; set; } = "any";
    public string Settlement { get; set; } = "any";
    /// <summary>
    /// Dictionary that contains a translation of the sentence as displayed in dialog for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Sentence { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationSentence"/> with <see cref="Sentence"/> filled by an input dictionary.
    /// It is expected to have at least an English key. It does not need to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationSentence("mySentenceId", 
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "sentenceRu"}, {English, "sentenceEn"}, {Italian, "sentenceIt"} });
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="sentence"></param>
    public LocalizationSentence(string id, Dictionary<ModLanguage, string> sentence)
    {
        Id = id;
        Sentence = Localization.SetDictionary(sentence);
        
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationSentence"/> with <see cref="Sentence"/> filled by an input string delimited by semi-colon.
    /// It is expected to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationSentence("mySentenceId", 
    ///     "sentenceRu;sentenceEn;sentenceCh");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="sentence"></param>
    public LocalizationSentence(string id, string sentence)
    {
        Id = id;
        Sentence = Localization.SetDictionary(sentence);
    }
    /// <summary>
    /// Create a string delimited by semi-colon that follows the in-game convention order for localization of sentences.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationSentence("mySentenceId", "sentenceRu;sentenceEn;sentenceCh").CreateLine();
    /// </code>
    /// returns the string "mySentenceId;any;any;any;any;any;sentenceRu;sentenceEn;sentenceCh;sentenceEn;sentenceEn;sentenceEn;sentenceEn;sentenceEn;sentenceEn;sentenceEn;sentenceEn;sentenceEn;".
    /// </example>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> CreateLine(string? _)
    {
        string line = string.Format("{0};{1};{2};{3};{4};{5};", Id, Tags, Role, Type, Faction, Settlement);
        line += string.Concat(Sentence.Values.Select(x => @$"{x};"));
        
        yield return line;
    }
}
public static partial class Msl
{
    /// <summary>
    /// Wrapper for the LocalizationDialog class
    /// </summary>
    /// <param name="sentences"></param>
    public static void InjectTableDialogLocalization(params ILocalizationElement[] sentences)
    {
        LocalizationBaseTable localizationBaseTable = new("gml_GlobalScript_table_NPC_Lines",
            ("NPC - GREETINGS;", null)
        );
        localizationBaseTable.InjectTable(sentences.ToList());
    }
}