using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public class LocalizableBooksBuilder
    {
        private string _id;
        private Dictionary<string, LocalizedStrings> _localizedStrings = new();
        
        // Helper method to add a key-value pair to the dictionary
        private LocalizableBooksBuilder Add(string key, LocalizedStrings text)
        {
            _localizedStrings[key] = text;
            return this;
        }

        // Helper method to get the hook for a key
        internal static string GetHookForKey(string key)
        {
            return key switch
            {
                "bookName" => "book_name_end",
                "bookContent" => "book_content_end",
                "bookMidText" => "book_mid_text_end",
                "bookDesc" => "book_desc_end",
                "bookType" => "book_type_end",
                "paintingName" => "painting_name_end",
                "paintingInfo" => "painting_info_end",
                "paintingAuthor" => "painting_author_end",
                "paintingDesc" => "painting_desc_end", // Devs fucked up this key, this needs to be injected at ind + 1
                 _ => throw new KeyNotFoundException($"Key '{key}' not found.")
            };
        }
        
        // Helper method to format a line for a key
        internal static string FormatLineForKey(string key, string id, LocalizedStrings text)
        {
            return key switch
            {
                "bookName" 
                 or "bookContent"
                 or "bookMidText"
                 or "bookDesc"
                 or "bookType"
                 or "paintingName"
                 or "paintingInfo"
                 or "paintingAuthor"
                 or "paintingDesc"
                    => $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};{text.Czech};{text.SpanishSpain};",
                _ => throw new KeyNotFoundException($"Key {key} not found.")
            };
        }
        
        // API methods to set the localized strings
        public LocalizableBooksBuilder WithId(string id)
        {
            _id = id;
            return this;
        }
        public LocalizableBooksBuilder WithBookName(LocalizedStrings text) => Add("bookName", text);
        public LocalizableBooksBuilder WithBookContent(LocalizedStrings text) => Add("bookContent", text);
        public LocalizableBooksBuilder WithBookMidText(LocalizedStrings text) => Add("bookMidText", text);
        public LocalizableBooksBuilder WithBookDesc(LocalizedStrings text) => Add("bookDesc", text);
        public LocalizableBooksBuilder WithBookType(LocalizedStrings text) => Add("bookType", text);
        public LocalizableBooksBuilder WithPaintingName(LocalizedStrings text) => Add("paintingName", text);
        public LocalizableBooksBuilder WithPaintingInfo(LocalizedStrings text) => Add("paintingInfo", text);
        public LocalizableBooksBuilder WithPaintingAuthor(LocalizedStrings text) => Add("paintingAuthor", text);
        public LocalizableBooksBuilder WithPaintingDesc(LocalizedStrings text) => Add("paintingDesc", text);

        // API method called to finish the builder and call the injector
        public void Inject()
        {
            if (_localizedStrings.Count > 0)
                DoInjectTableLocalizableBooks(_id, _localizedStrings);
            else
            {
                Log.Error("Failed to inject localizable book: Nothing to inject.");
                throw new ArgumentException("Failed to inject localizable book: Nothing to inject.");
            }
        }
    }

    // API method exposed to modders
    public static LocalizableBooksBuilder InjectTableLocalizableBooks() => new();
    
    // Method actually responsible for the injection
    private static void DoInjectTableLocalizableBooks(string id, Dictionary<string, LocalizedStrings> localizedStrings)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_books";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));

        foreach ((string key, LocalizedStrings text) in localizedStrings)
        {
            // Get hook for the key
            string hook = LocalizableBooksBuilder.GetHookForKey(key);
            
            // Find hook in table
            (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook));
            if (foundLine == null)
                Log.Error($"Failed to inject {key} into table {tableName}: Hook {hook} not found.");
            
            // Prepare line
            string newline = LocalizableBooksBuilder.FormatLineForKey(key, id, text);
            
            // Add line to table
            if (key == "paintingDesc") // Dirty hack to inject this key at ind + 1 because the key is fucked up
                table.Insert(ind + 1, newline);
            else
                table.Insert(ind, newline);
            Log.Information($"Injected {key} into table {tableName} at hook {hook}.");
        }
        ModLoader.SetTable(table, tableName);
    }
}