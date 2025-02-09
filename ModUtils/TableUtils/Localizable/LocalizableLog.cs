using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public static void InjectTableLocalizableLog(
        string id, 
        LocalizedStrings? text = null, 
        LocalizedStrings? word = null,
        LocalizedStrings? actions = null,
        LocalizedStrings? damages = null,
        LocalizedStrings? symbols = null
            )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_log";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Required setup
        int ind = 0;
        string? foundLine = null;

        #region TEXT
        if (text != null)
        {
            // Prepare line
            string newline = $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};";
            
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("text_end"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, newline);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Log Text into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Log Text into table {tableName}");
        }
        #endregion
        
        #region WORD
        if (word != null)
        {
            // Prepare line
            string newline = $"{id};{word.Russian};{word.English};{word.Chinese};{word.German};{word.SpanishLatam};{word.French};{word.Italian};{word.Portuguese};{word.Polish};{word.Turkish};{word.Japanese};{word.Korean};";
            
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("words_end"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, newline);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Log Word into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Log Word into table {tableName}");
        }
        #endregion

        #region ACTIONS
        if (actions != null)
        {
            // Prepare line
            string newline = $"{id};{actions.Russian};{actions.English};{actions.Chinese};{actions.German};{actions.SpanishLatam};{actions.French};{actions.Italian};{actions.Portuguese};{actions.Polish};{actions.Turkish};{actions.Japanese};{actions.Korean};";
            
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("actions_end"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, newline);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Log Actions into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Log Actions into table {tableName}");
        }
        #endregion

        #region DAMAGES
        if (damages != null)
        {
            // Prepare line
            string newline = $"{id};{damages.Russian};{damages.English};{damages.Chinese};{damages.German};{damages.SpanishLatam};{damages.French};{damages.Italian};{damages.Portuguese};{damages.Polish};{damages.Turkish};{damages.Japanese};{damages.Korean};";
            
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("damages_end"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, newline);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Log Damages into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Log Damages into table {tableName}");
        }
        #endregion

        #region SYMBOLS
        if (symbols != null)
        {
            // Prepare line
            string newline = $"{id};{symbols.Russian};{symbols.English};{symbols.Chinese};{symbols.German};{symbols.SpanishLatam};{symbols.French};{symbols.Italian};{symbols.Portuguese};{symbols.Polish};{symbols.Turkish};{symbols.Japanese};{symbols.Korean};";
            
            // Find hook in table
            (ind, foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("symbols_end"));
            
            // Add line to table
            if (foundLine != null)
            {
                table.Insert(ind, newline);
                ModLoader.SetTable(table, tableName);
                Log.Information($"Injected Log Symbols into table {tableName}");
            }
            else
                Log.Error($"Failed to inject Log Symbols into table {tableName}");
        }
        #endregion
    }
}