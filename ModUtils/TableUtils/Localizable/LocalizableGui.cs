using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public class LocalizableGuiBuilder
    {
        private string _id;
        private Dictionary<string, LocalizedStrings> _localizedStrings = new();
        
        // Helper method to add a key-value pair to the dictionary
        private LocalizableGuiBuilder Add(string key, LocalizedStrings text)
        {
            _localizedStrings[key] = text;
            return this;
        }
        
        // Helper method to get the hook for a key
        internal static string GetHookForKey(string key)
        {
            return key switch
            {
                "text" => "text_end",
                _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // Helper method to format a line for a key
        internal static string FormatLineForKey(string key, string id, LocalizedStrings text)
        {
            return key switch
            {
                "text" => $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};{text.Czech};{text.SpanishSpain};",
                _ => throw new KeyNotFoundException($"Key {key} not found.")
            };
        }
        
        // API methods to set the localized strings
        public LocalizableGuiBuilder WithId(string id)
        {
            _id = id;
            return this;
        }
        public LocalizableGuiBuilder WithText(LocalizedStrings text) => Add("text", text);
        
        // API method called to finish the builder and call the injector
        public void Inject()
        {
            if (_localizedStrings.Count > 0)
                DoInjectTableLocalizableGui(_id, _localizedStrings);
            else
            {
                Log.Error("Failed to inject localizable effect: Nothing to inject.");
                throw new ArgumentException("Failed to inject localizable effect: Nothing to inject.");
            }
        }
    }
    
    // API exposed to modders
    public static LocalizableGuiBuilder InjectTableLocalizableGui() => new();
    
    // Method actually responsible for the injection
    private static void DoInjectTableLocalizableGui(string id, Dictionary<string, LocalizedStrings> localizedStrings)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_gui";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));

        foreach ((string key, LocalizedStrings text) in localizedStrings)
        {
            // Get the hook for the key
            string hook = LocalizableGuiBuilder.GetHookForKey(key);
            
            // Find hook in table
            (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook));
            if (foundLine == null)
                Log.Error($"Failed to inject {key} into table {tableName}: Hook '{hook}' not found");
            
            // Prepare line
            string newline = LocalizableGuiBuilder.FormatLineForKey(key, id, text);
            
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