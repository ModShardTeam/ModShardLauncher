using System;
using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

public class LocalizationBook : ILocalizationElement
{
    /// <summary>
    /// Id of the modifier
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Dictionary that contains a translation of the modifier as displayed in the log for each available languages.
    /// </summary>
    public Dictionary<ModLanguage, string> Name { get; set; } = new();
    public Dictionary<ModLanguage, string> Content { get; set; } = new();
    public Dictionary<ModLanguage, string> MidText { get; set; } = new();
    public Dictionary<ModLanguage, string> Description { get; set; } = new();
    public Dictionary<ModLanguage, string> Type { get; set; } = new();
    /// <summary>
    /// Return an instance of <see cref="LocalizationBook"/> with <see cref="Loc"/> filled by an input dictionary.
    /// It is expected to have at least an English key. It does not need to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationBook("mySpeechId", 
    ///     new Dictionary &lt; ModLanguage, string &gt; () { {Russian, "speechRu"}, {English, "speechEn"}, {Italian, "speechIt"} });
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationBook(string id, 
        Dictionary<ModLanguage, string> name, 
        Dictionary<ModLanguage, string> content, 
        Dictionary<ModLanguage, string> midText, 
        Dictionary<ModLanguage, string> description,
        Dictionary<ModLanguage, string> type)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
        Content = Localization.SetDictionary(content);
        MidText = Localization.SetDictionary(midText);
        Description = Localization.SetDictionary(description);
        Type = Localization.SetDictionary(type);
    }
    /// <summary>
    /// Return an instance of <see cref="LocalizationBook"/> with <see cref="Loc"/> filled by an input string delimited by semi-colon.
    /// It is expected to follow the convention order of the localization table.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationBook("mySpeechId", 
    ///     "speechRu;speechEn;speechCh");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="modifier"></param>
    public LocalizationBook(string id, 
        string name, 
        string content, 
        string midText, 
        string description,
        string type)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
        Content = Localization.SetDictionary(content);
        MidText = Localization.SetDictionary(midText);
        Description = Localization.SetDictionary(description);
        Type = Localization.SetDictionary(type);
    }
    /// <summary>
    /// Create a string delimited by semi-colon that follows the in-game convention order for localization of speechs.
    /// <example>
    /// For example:
    /// <code>
    /// LocalizationBook("mySpeechId", "speechRu;speechEn;speechCh").CreateLine();
    /// </code>
    /// returns the string "mySpeechId;speechRu;speechEn;speechCh;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;speechEn;".
    /// </example>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> CreateLine(string? selector)
    {
        switch(selector)
        {
        case "name":
        yield return $"{Id};{string.Concat(Name.Values.Select(x => @$"{x};"))}";
            break;
        case "content":
        yield return $"{Id};{string.Concat(Content.Values.Select(x => @$"{x};"))}";
            break;
        case "mid_text":
        yield return $"{Id};{string.Concat(MidText.Values.Select(x => @$"{x};"))}";
            break;
        case "desc":
        yield return $"{Id};{string.Concat(Description.Values.Select(x => @$"{x};"))}";
            break;
        case "type":
        yield return $"{Id};{string.Concat(Type.Values.Select(x => @$"{x};"))}";
            break;
        }
    }
}
public static partial class Msl
{
    /// <summary>
    /// Wrapper for the LocalizationBooks class
    /// </summary>
    /// <param name="modifiers"></param>
    public static Func<IEnumerable<string>, IEnumerable<string>> CreateInjectionBooksLocalization(params LocalizationBook[] books)
    {
        LocalizationBaseTable localizationBaseTable = new(
            ("book_name_end;", "name"), 
            ("book_content_end;", "content"),
            ("book_mid_text_end;", "mid_text"),
            ("book_desc_end;", "desc"),
            ("book_type_end;", "type")
        );
        return localizationBaseTable.CreateInjectionTable(books.Select(x => x as ILocalizationElement).ToList());
    }
    public static void InjectTableBooksLocalization(params LocalizationBook[] books)
    {
        Localization.InjectTable("gml_GlobalScript_table_Books", CreateInjectionBooksLocalization(books));
    }
}