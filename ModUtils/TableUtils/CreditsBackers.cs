using System.Collections.Generic;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public static void InjectTableCreditsBackers(string? name = null, string? nickname = null)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_credits_backers";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{name};{nickname};";
        
        // Add line to end of table
        table.Add(newline);
        ModLoader.SetTable(table, tableName);
        Log.Information($"Injected {name}:{nickname} into {tableName} table.");
    }
}