using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Serilog;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public partial class Msl
{
    public enum RecipesCookHook
    {
        [EnumMember(Value = "FRYING & SALTING")]
        FryingAndSalting,
        SOUPS,
        [EnumMember(Value = "MAIN COURSES")]
        MainCourses,
        SALADS,
        DESSERTS
    }
    
    public enum RecipesCookCategory
    {
        other,
        soup,
        main,
        side,
        dessert
    }

    public enum RecipesCookAdditive
    {
        X,
        V,
        butter
    }

    public enum RecipesCookSource
    {
        [EnumMember(Value = "Base Recipe")]
        BaseRecipe,
        Trade,
        [EnumMember(Value = "Random Find")]
        RandomFind,
        [EnumMember(Value = "Reputation Reward")]
        ReputationReward
    }
    
    public static void InjectTableRecipesCook(
        RecipesCookHook hook,
        string NAME,
        RecipesCookCategory CAT,
        string? Recipe1 = null,
        string? Recipe2 = null,
        string? Recipe3 = null,
        string? Recipe4 = null,
        string? Recipe5 = null,
        RecipesCookAdditive ADDITIVES = RecipesCookAdditive.X,
        bool COOKPOT = false,
        bool PLATE = false,
        bool DEEP_PLATE = false,
        bool SATIETY = false,
        short? MORALE = null,
        short? SANITY = null,
        RecipesCookSource SOURCE = RecipesCookSource.BaseRecipe
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_recipes_cook";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{NAME};{CAT};{Recipe1 ?? "-"};{Recipe2 ?? "-"};{Recipe3 ?? "-"};{Recipe4 ?? "-"};{Recipe5 ?? "-"};{GetEnumMemberValue(ADDITIVES)};{(COOKPOT ? "V" : "X")};{(PLATE ? "V" : "X")};{(DEEP_PLATE ? "V" : "X")};{(SATIETY ? "V" : "X")};{MORALE};{SANITY};{GetEnumMemberValue(SOURCE)};";
        
        // Find hook
        string hookStr = "// " + GetEnumMemberValue(hook);
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hookStr));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected Armor {NAME} into table {tableName} under {hook}");
        }
        else
        {
            Log.Error($"Cannot find hook {hook} in table {tableName}");
            throw new Exception($"Cannot find hook {hook} in table {tableName}");
        }
    }
}