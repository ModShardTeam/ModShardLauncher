using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

public partial class Msl
{
    public static void InjectTablePotion(string name, string effectScript, params string[] blockedEffects)
    {
        // Weird table. It has no header, and seems to work like this:
        // Name of the potion | script with actual effect | any number of incompatible effects that cannot coexist with the actual effect
        //
        // Example:
        // MyPotion;myEffectScript;blockedEffectScript1;blockedEffectScript2;blockedEffectScript3;blockedEffectScript4... 
        //
        // This seems to be used when rolling potions in the game, to ensure that the potion doesn't have conflicting effects.
        // Needs more investigation to figure out how many blocked effects are allowed, max vanilla potion has 26.
        
        
        // Table filename
        const string tableName = "gml_GlobalScript_table_Potion";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{name};{effectScript};{string.Join(";", blockedEffects)};";
        
        // Adding potentially missing ; at the end of the line. Could be unnecessary ?
        while (newline.Count(t => t == ';') < 28)
            newline += ';';
        
        // Add line to end of table
        table.Add(newline);
        ModLoader.SetTable(table, tableName);
        Log.Information($"Injected {name} into {tableName} table.");
    }
}