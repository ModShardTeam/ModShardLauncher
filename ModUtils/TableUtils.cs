using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using Microsoft.CodeAnalysis;
using ModShardLauncher.Mods;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    static public class Localization
    {
        static public readonly ImmutableList<ModLanguage> LanguageList = Enum.GetValues(typeof(ModLanguage)).Cast<ModLanguage>().ToImmutableList();
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
        static public void SetDictionary(Dictionary<ModLanguage, string> dict, Dictionary<ModLanguage, string> values)
        {
            string englishName = values[ModLanguage.English];
            foreach (ModLanguage language in LanguageList)
            {
                if (!dict.ContainsKey(language))
                {
                    if (!values.ContainsKey(language))
                    {
                        dict[language] = englishName;
                    }
                    else
                    {
                        dict[language] = values[language];
                    }
                }
            }
        }
        static public void SetDictionary(Dictionary<ModLanguage, string> dict, string value)
        {
            if (value.Contains(';'))
            {
                Dictionary<ModLanguage, string> tmp = ToDict(value);
                foreach (ModLanguage language in LanguageList)
                {
                    if (!dict.ContainsKey(language))
                    {
                        dict[language] = tmp[language];
                    }
                }
            } 
            else
            {
                foreach (ModLanguage language in LanguageList)
                {
                    if (!dict.ContainsKey(language))
                    {
                        dict[language] = value;
                    }
                }
            } 
        }
    }
}