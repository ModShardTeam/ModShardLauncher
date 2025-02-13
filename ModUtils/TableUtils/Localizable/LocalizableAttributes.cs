using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public class LocalizableAttributesBuilder
    {
        private string _id;
        private Dictionary<string, LocalizedStrings> _localizedStrings = new();
        
        // Helper method to add a key-value pair to the dictionary
        private LocalizableAttributesBuilder Add(string key, LocalizedStrings text)
        {
            _localizedStrings[key] = text;
            return this;
        }
        
        // Helper method to get the hook for a key
        internal static string GetHookForKey(string key)
        {
            return key switch
            {
                "attributeText" => "attribute_text_end",
                "attributeTradeHover" => "attribute_trade_hover_end",
                "attributeBreakdown" => "attribute_breakdown_end",
                _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // Helper method to format a line for a key
        internal static string FormatLineForKey(string key, string id, LocalizedStrings text)
        {
            return key switch
            {
                "attributeText" or 
                "attributeTradeHover" or 
                "attributeBreakdown" => $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};",
                _ => throw new KeyNotFoundException($"Key {key} not found.")
            };
        }
        
        // API methods to set the localized strings
        public LocalizableAttributesBuilder WithId(string id)
        {
            _id = id;
            return this;
        }
        public LocalizableAttributesBuilder WithAttributeText(LocalizedStrings text) => Add("attributeText", text);
        public LocalizableAttributesBuilder WithAttributeTradeHover(LocalizedStrings text) => Add("attributeTradeHover", text);
        public LocalizableAttributesBuilder WithAttributeBreakdown(LocalizedStrings text) => Add("attributeBreakdown", text);
        
        // API method called to finish the builder and call the injector
        public void Inject()
        {
            if (_localizedStrings.Count > 0)
                DoInjectTableLocalizableAttributes(_id, _localizedStrings);
            else
            {
                Log.Error("Failed to inject localizable attribute: Nothing to inject.");
                throw new ArgumentException("Failed to inject localizable attribute: Nothing to inject.");
            }
        }
    }
    
    // API exposed to modders
    public static LocalizableAttributesBuilder InjectTableLocalizableAttributes() => new();
    
    // Method actually responsible for the injection
    private static void DoInjectTableLocalizableAttributes(string id, Dictionary<string, LocalizedStrings> localizedStrings)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_attributes";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));

        foreach ((string key, LocalizedStrings text) in localizedStrings)
        {
            // Get the hook for the key
            string hook = LocalizableAttributesBuilder.GetHookForKey(key);
            
            // Find hook in table
            (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook));
            if (foundLine == null)
                Log.Error($"Failed to inject {key} into table {tableName}: Hook '{hook}' not found");
            
            // Prepare line
            string newline = LocalizableAttributesBuilder.FormatLineForKey(key, id, text);
            
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