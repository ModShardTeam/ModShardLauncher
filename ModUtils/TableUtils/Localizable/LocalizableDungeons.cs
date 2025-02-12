using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public class LocalizableDungeonsBuilder
    {
        private string _id;
        private Dictionary<string, LocalizedStrings> _localizedStrings = new();
        
        // Helper method to add a key-value pair to the dictionary
        private LocalizableDungeonsBuilder Add(string key, LocalizedStrings text)
        {
            _localizedStrings[key] = text;
            return this;
        }
        
        // Helper method to get the hook for a key
        internal static string GetHookForKey(string key)
        {
            return key switch
            {
                "cryptPrefix" => "crypt_prefix_end",
                "bastionPrefix" => "bastion_prefix_end",
                "bastionName" => "bastion_name_end",
                "cryptName" => "crypt_name_end",
                "cryptPreset" => "crypt_preset_end",
                "bastionPreset" => "bastion_preset_end",
                "catacombsPreset" => "catacombs_preset_end",
                "cavesPreset" => "caves_preset_end",
                "locationName" => "location_name_end",
                "dungeonModifier" => "dungeon_modifier_end",
                _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // Helper method to format a line for a key
        internal static string FormatLineForKey(string key, string id, LocalizedStrings text)
        {
            return key switch
            {
                "cryptPrefix" or 
                "bastionPrefix" or 
                "bastionName" or 
                "cryptName" or 
                "locationName" => $";{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};",
                
                "cryptPreset" or
                "bastionPreset" or
                "catacombsPreset" or
                "cavesPreset" or
                "dungeonModifier" => $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};",
                
                _ => throw new KeyNotFoundException($"Key {key} not found.")
            };
        }
        
        // API methods to set the localized strings
        public LocalizableDungeonsBuilder WithId(string id)
        {
            _id = id;
            return this;
        }
        public LocalizableDungeonsBuilder WithCryptPrefix(LocalizedStrings text) => Add("cryptPrefix", text);
        public LocalizableDungeonsBuilder WithBastionPrefix(LocalizedStrings text) => Add("bastionPrefix", text);
        public LocalizableDungeonsBuilder WithBastionName(LocalizedStrings text) => Add("bastionName", text);
        public LocalizableDungeonsBuilder WithCryptName(LocalizedStrings text) => Add("cryptName", text);
        public LocalizableDungeonsBuilder WithCryptPreset(LocalizedStrings text) => Add("cryptPreset", text);
        public LocalizableDungeonsBuilder WithBastionPreset(LocalizedStrings text) => Add("bastionPreset", text);
        public LocalizableDungeonsBuilder WithCatacombsPreset(LocalizedStrings text) => Add("catacombsPreset", text);
        public LocalizableDungeonsBuilder WithCavesPreset(LocalizedStrings text) => Add("cavesPreset", text);
        public LocalizableDungeonsBuilder WithLocationName(LocalizedStrings text) => Add("locationName", text);
        public LocalizableDungeonsBuilder WithDungeonModifier(LocalizedStrings text) => Add("dungeonModifier", text);
        
        // API method called to finish the builder and call the injector
        public void Inject()
        {
            if (_localizedStrings.Count > 0)
                DoInjectTableLocalizableDungeons(_id, _localizedStrings);
            else
            {
                Log.Error("Failed to inject localizable effect: Nothing to inject.");
                throw new ArgumentException("Failed to inject localizable effect: Nothing to inject.");
            }
        }
    }
    
    // API exposed to modders
    public static LocalizableDungeonsBuilder InjectTableLocalizableDungeons() => new();
    
    // Method actually responsible for the injection
    private static void DoInjectTableLocalizableDungeons(string id, Dictionary<string, LocalizedStrings> localizedStrings)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_dungeons";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));

        foreach ((string key, LocalizedStrings text) in localizedStrings)
        {
            // Get hook for the key
            string hook = LocalizableDungeonsBuilder.GetHookForKey(key);
            
            // Find hook in table
            (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook));
            if (foundLine == null)
                Log.Error($"Failed to inject {key} into table {tableName}: Hook '{hook}' not found");
            
            // Prepare line
            string newline = LocalizableDungeonsBuilder.FormatLineForKey(key, id, text);
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, newline);
                Log.Information($"Injected {key} into table '{tableName}' at hook '{hook}'.");
            }
        }
        ModLoader.SetTable(table, tableName);
    }
}