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
    public enum DungeonsSpawnHook
    {
        GENERIC,
        UNDEAD
    }
    
    public enum DungeonsSpawnTemplateType
    {
        Normal,
        Advanced,
        Special
    }
    
    public enum DungeonsSpawnTier
    {
        [EnumMember(Value = "1")]
        Tier1,
        [EnumMember(Value = "2")]
        Tier2,
        [EnumMember(Value = "3")]
        Tier3,
        [EnumMember(Value = "4")]
        Tier4,
        [EnumMember(Value = "5")]
        Tier5    
    }

    [Flags]
    public enum DungeonsSpawnFaction
    {
        Undead = 1,
        Vampire = 2,
        Brigand = 4
    }
    
    public static void InjectTableDungeonsSpawn(
        DungeonsSpawnHook hook,
        string id, // Could be byte, not sure how it's used
        DungeonsSpawnTemplateType Template_Type,
        HashSet<DungeonsSpawnTier> Dungeon_Tier,
        DungeonsSpawnFaction Faction,
        string Enemy1,
        string? Enemy2 = null,
        string? Enemy3 = null,
        string? Enemy4 = null,
        string? Enemy5 = null,
        string? Enemy6 = null
            )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_dungeons_spawn";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Parse Dungeon_Tier
        string dungeonTierStr = "";
        foreach (DungeonsSpawnTier tier in Dungeon_Tier)
            dungeonTierStr += $"{GetEnumMemberValue(tier)}, ";
        dungeonTierStr = dungeonTierStr.TrimEnd(',', ' ');
        
        // Prepare new line
        string newline = $"{id};{Template_Type};{dungeonTierStr};{Faction};{Enemy1};{Enemy2};{Enemy3};{Enemy4};{Enemy5};{Enemy6};";
        
        // Find hook
        string hookStr = "// " + GetEnumMemberValue(hook);
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hookStr));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected Dungeon Spawn {id} into {tableName} under {hook}");
        }
        else
        {
            Log.Error($"Cannot find Hook {hook} in table {tableName}");
            throw new Exception($"Hook {hook} not found in table {tableName}");
        }
    }
}