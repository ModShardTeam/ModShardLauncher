using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public class LocalizableEffectsBuilder
    {
        private string _id;
        private Dictionary<string, LocalizedStrings> _localizedStrings = new();
        
        // Helper method to add a key-value pair to the dictionary
        private LocalizableEffectsBuilder Add(string key, LocalizedStrings text)
        {
            _localizedStrings[key] = text;
            return this;
        }
        
        // Helper method to get the hook for a key
        internal static string GetHookForKey(string key)
        {
            return key switch
            {
                "buffName" => "buff_name_end",
                "buffType" => "buff_type_end",
                "buffDesc" => "buff_desc_end",
                _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // Helper method to format a line for a key
        internal static string FormatLineForKey(string key, string id, LocalizedStrings text)
        {
            return key switch
            {
                "buffName" or "buffType" or "buffDesc" => $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};",
                _ => throw new KeyNotFoundException($"Key {key} not found.")
            };
        }
        
        // API methods to set the localized strings
        public LocalizableEffectsBuilder WithId(string id)
        {
            _id = id;
            return this;
        }
        public LocalizableEffectsBuilder WithBuffName(LocalizedStrings text) => Add("buffName", text);
        public LocalizableEffectsBuilder WithBuffType(LocalizedStrings text) => Add("buffType", text);
        public LocalizableEffectsBuilder WithBuffDesc(LocalizedStrings text) => Add("buffDesc", text);
        
        // API method called to finish the builder and call the injector
        public void Inject()
        {
            if (_localizedStrings.Count > 0)
                DoInjectLocalizableEffects(_id, _localizedStrings);
            else
            {
                Log.Error("Failed to inject localizable effect: Nothing to inject.");
                throw new ArgumentException("Failed to inject localizable effect: Nothing to inject.");
            }
        }
    }
    
    // API method exposed to modders
    public static LocalizableEffectsBuilder InjectTableLocalizableEffects() => new();
    
    // Method actually responsible for the injection
    private static void DoInjectLocalizableEffects(string id, Dictionary<string, LocalizedStrings> localizedStrings)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_effects";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));

        foreach ((string key, LocalizedStrings text) in localizedStrings)
        {
            // Get hook for the key
            string hook = LocalizableEffectsBuilder.GetHookForKey(key);
            
            // Find hook in table
            (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook));
            if (foundLine == null)
                Log.Error($"Failed to inject {key} into table {tableName}: Hook '{hook}' not found.");;
            
            // Prepare line
            string newline = LocalizableEffectsBuilder.FormatLineForKey(key, id, text);
            
            // Add line to table
            table.Insert(ind, newline);
            Log.Information($"Injected {key} into table '{tableName}' at hook '{hook}'.");
        }
        ModLoader.SetTable(table, tableName);
    }
}