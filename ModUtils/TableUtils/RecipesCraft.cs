using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Serilog;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class Msl
{
    public enum RecipesCraftHook
    {
        BASIC,
        ADVANCED,
        OTHER
    }
    public enum  RecipesCraftCategory
    {
        basic,
        advanced,
        other
    }

    public enum RecipesCraftSource
    {
        Trade,
        [EnumMember(Value = "Base Recipe")]
        BaseRecipe,
        [EnumMember(Value = "Random Find")]
        RandomFind,
        [EnumMember(Value = "Pol Find")]
        PolFind
    }
    
    public static void InjectTableRecipesCraft(
        RecipesCraftHook hook,
        string NAME,
        RecipesCraftCategory CAT,
        ushort XP,
        ushort AMOUNT,
        RecipesCraftSource SOURCE,
        string Recipe1,
        string? Recipe2 = null,
        string? Recipe3 = null,
        string? Recipe4 = null,
        string? Recipe5 = null,
        string? Recipe6 = null
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_recipes_craft";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare new line
        string newline = $"{NAME};{CAT};{Recipe1};{Recipe2 ?? "-"};{Recipe3 ?? "-"};{Recipe4 ?? "-"};{Recipe5 ?? "-"};{Recipe6 ?? "-"};{AMOUNT};{XP};{GetEnumMemberValue(SOURCE)}";
        
        // Find hook
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hook.ToString()));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected craft recipe {NAME} into {tableName} under {hook}");
        }
        else
        {
            Log.Error($"Cannot find hook {hook} in table {tableName}");
            throw new Exception($"Hook {hook} not found in table {tableName}");
        }
    }
}