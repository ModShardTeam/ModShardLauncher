using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public static void InjectTableLocalizableControls(int id, LocalizedStrings name)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_controls";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{id};{name.Russian};{name.English};{name.Chinese};{name.German};{name.SpanishLatam};{name.French};{name.Italian};{name.Portuguese};{name.Polish};{name.Turkish};{name.Japanese};{name.Korean};";
        
        // Find hook in table
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("keyboard_end"));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected Control {id} into table {tableName}");
        }
        else
            Log.Error($"Failed to inject Control {id} into table {tableName}");
    }
}