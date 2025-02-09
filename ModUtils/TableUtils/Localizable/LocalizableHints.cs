using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public static void InjectTableLocalizableHints(string id, LocalizedStrings text)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_hints";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{id};{text.Russian};{text.English};{text.Chinese};{text.German};{text.SpanishLatam};{text.French};{text.Italian};{text.Portuguese};{text.Polish};{text.Turkish};{text.Japanese};{text.Korean};";
        
        // Find hook in table
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("text_end"));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected Hint {id} into table {tableName}");
        }
        else
            Log.Error($"Failed to inject Hint {id} into table {tableName}");
    }
}