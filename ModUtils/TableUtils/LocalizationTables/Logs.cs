using System;
using System.Collections.Generic;
using System.Linq;
using ModShardLauncher;
using ModShardLauncher.Mods;

namespace ModShardLauncher;

public class LocalizationLogText : ILocalizationElement
{
    public string Id { get; set; }
    public Dictionary<ModLanguage, string> Name { get; set; } = new();
    public LocalizationLogText(string id, Dictionary<ModLanguage, string> name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
    }
    public LocalizationLogText(string id, string name)
    {
        Id = id;
        Name = Localization.SetDictionary(name);
    }
    public IEnumerable<string> CreateLine(string? selector)
    {
        yield return $"{Id};{string.Concat(Name.Values.Select(x => @$"{x};"))}";
    }
}

public static partial class Msl
{
    public static Func<IEnumerable<string>, IEnumerable<string>> CreateInjectionLogTextLocalization(params LocalizationLogText[] texts)
    {
        LocalizationBaseTable localizationBaseTable = new(
            ("text_end;", null)
        );
        return localizationBaseTable.CreateInjectionTable(texts.Select(x => x as ILocalizationElement).ToList());
    }
    public static void InjectTableLogTextLocalization(params LocalizationLogText[] texts)
    {
        Localization.InjectTable("gml_GlobalScript_table_log", CreateInjectionLogTextLocalization(texts));
    }

    public static Func<IEnumerable<string>, IEnumerable<string>> CreateInjectionLogWordsLocalization(params LocalizationLogText[] texts)
    {
        LocalizationBaseTable localizationBaseTable = new(
            ("words_end;", null)
        );
        return localizationBaseTable.CreateInjectionTable(texts.Select(x => x as ILocalizationElement).ToList());
    }
    public static void InjectTableLogWordsLocalization(params LocalizationLogText[] texts)
    {
        Localization.InjectTable("gml_GlobalScript_table_log", CreateInjectionLogWordsLocalization(texts));
    }

    public static Func<IEnumerable<string>, IEnumerable<string>> CreateInjectionLogActionsLocalization(params LocalizationLogText[] texts)
    {
        LocalizationBaseTable localizationBaseTable = new(
            ("actions_end;", null)
        );
        return localizationBaseTable.CreateInjectionTable(texts.Select(x => x as ILocalizationElement).ToList());
    }
    public static void InjectTableLogActionsLocalization(params LocalizationLogText[] texts)
    {
        Localization.InjectTable("gml_GlobalScript_table_log", CreateInjectionLogActionsLocalization(texts));
    }

    public static Func<IEnumerable<string>, IEnumerable<string>> CreateInjectionLogDamagesLocalization(params LocalizationLogText[] texts)
    {
        LocalizationBaseTable localizationBaseTable = new(
            ("damages_end;", null)
        );
        return localizationBaseTable.CreateInjectionTable(texts.Select(x => x as ILocalizationElement).ToList());
    }
    public static void InjectTableLogDamagesLocalization(params LocalizationLogText[] texts)
    {
        Localization.InjectTable("gml_GlobalScript_table_log", CreateInjectionLogDamagesLocalization(texts));
    }

    public static Func<IEnumerable<string>, IEnumerable<string>> CreateInjectionLogSymbolsLocalization(params LocalizationLogText[] texts)
    {
        LocalizationBaseTable localizationBaseTable = new(
            ("symbols_end;", null)
        );
        return localizationBaseTable.CreateInjectionTable(texts.Select(x => x as ILocalizationElement).ToList());
    }
    public static void InjectTableLogSymbolsLocalization(params LocalizationLogText[] texts)
    {
        Localization.InjectTable("gml_GlobalScript_table_log", CreateInjectionLogSymbolsLocalization(texts));
    }
}