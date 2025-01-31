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
    public enum SurfaceSpawnSlot
    {
        Hostile,
        Neutral
    }
    
    public enum SurfaceSpawnTier
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

    // Bitmask enum, use like this: `SurfaceSpawnBiome Biome = SurfaceSpawnBiome.Forest | SurfaceSpawnBiome.Steppe` to have multiple choices 
    [Flags]
    public enum SurfaceSpawnBiome
    {
        Any = 1,
        Forest = 2,
        Pineforest = 4,
        Field = 8,
        Pinefield = 16,
        Steppe = 32
    }

    public enum SurfaceSpawnFaction
    {
        Brigand,
        Beast,       
        // Anything below is not confirmed, just assuming it would work
        Undead,
        Vampire,
        GrandMagistrate,
        RottenWillow,
        Mercenary,
        Neutral
    }
    
    public static void InjectTableSurfaceSpawn(
        SurfaceSpawnSlot Slot,
        // SurfaceSpawnTier Tier,
        HashSet<SurfaceSpawnTier> Tier,
        SurfaceSpawnBiome Biome,
        SurfaceSpawnFaction Faction,
        byte Spawn_Chance,
        string Enemy1,
        string? Enemy2 = null,
        string? Enemy3 = null,
        string? Enemy4 = null,
        string? Enemy5 = null,
        string? Enemy6 = null
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_surface_spawn";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Parse tier
        string tierStr = "";
        foreach (SurfaceSpawnTier tier in Tier)
            tierStr += $"{GetEnumMemberValue(tier)}, ";
        tierStr = tierStr.TrimEnd(',', ' ');

        // Prepare line
        string newline = $"{Slot};{tierStr};{Biome};{Faction};{Spawn_Chance};{Enemy1};{Enemy2};{Enemy3};{Enemy4};{Enemy5};{Enemy6};";
        
        // Add line to table
        table.Add(newline);
        ModLoader.SetTable(table, tableName);
        Log.Information($"Injected a Spawn into table {tableName}");
    }
}