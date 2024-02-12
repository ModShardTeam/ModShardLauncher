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
    public class LocalizationItem
    {
        public string OName { get; set; }
        public Dictionary<ModLanguage, string> ConsumableName { get; set; } = new();
        public Dictionary<ModLanguage, string> ConsumableID { get; set; } = new();
        public Dictionary<ModLanguage, string> ConsumableDescription { get; set; } = new();
        public LocalizationItem(string oName)
        {
            OName = oName;
        }
        public LocalizationItem(string oName, Dictionary<ModLanguage, string> dictName, Dictionary<ModLanguage, string> dictID, Dictionary<ModLanguage, string> dictDescription)
        {
            OName = oName;
            Localization.SetDictionary(ConsumableName, dictName);
            Localization.SetDictionary(ConsumableID, dictID);
            Localization.SetDictionary(ConsumableDescription, dictDescription);
        }
        public LocalizationItem(string oName, string valuesName, string valuesID, string valuesDescription)
        {
            OName = oName;
            Localization.SetDictionary(ConsumableName, valuesName);
            Localization.SetDictionary(ConsumableID, valuesID);
            Localization.SetDictionary(ConsumableDescription, valuesDescription);
        }
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
        private IEnumerable<string> EditTable(string tableName)
        {
            return EditTable(Msl.ThrowIfNull(ModLoader.GetTable(tableName)));
        }
        public void InjectTable()
        {
            ModLoader.SetTable(EditTable("gml_GlobalScript_table_consumables").ToList(), "gml_GlobalScript_table_consumables");
        }
    }
}