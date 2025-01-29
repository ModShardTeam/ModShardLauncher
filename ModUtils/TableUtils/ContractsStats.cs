using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Serilog;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]

public partial class Msl
{
    public enum ContractsStatsCategory
    {
        kill,
        destroy,
        clear,
        interact,
        find
    }

    public enum ContractsStatsDungeonType
    {
        Crypt,
        Catacombs,
        Bastion
    }
    
    public static void InjectTableContractsStats(
        string id,
        byte Amount,
        ContractsStatsCategory Category,
        string Faction, // Could be an enum ? Leaving as string for modded factions for now
        string Village_Type, // Could be an enum ? Leaving as string for modded villages and multiple choices
        ContractsStatsDungeonType DungeonType,
        string Script,
        byte Contract_Deadline,
        byte Contract_Expiration,
        short Contract_Reward,
        short Contract_Reputation,
        short Contract_Reputation_Loss,
        byte Village_Aftermath,
        string Dungeon_Aftermath, // Could be an enum ? Leaving as string for multiple choices
        string Contract_NPC,
        byte Contract_Respawn, // Unknown type, assuming byte for now
        byte Mod_time, // Unknown type, assuming byte for now
        bool Can_Generate,
        float k
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_contracts_stats";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{id};{Amount};{Category};{Faction};{Village_Type};{DungeonType};{Script};{Contract_Deadline};{Contract_Expiration};{Contract_Reward};{Contract_Reputation};{Contract_Reputation_Loss};{Village_Aftermath};{Dungeon_Aftermath};{Contract_NPC};{Contract_Respawn};{Mod_time};{Can_Generate};{k};";
        
        // Find hook
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("bastion_HostageRottenWillow"));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected contract {id} into {tableName}");
        }
        else
        {
            Log.Error($"Hook not found in {tableName}. {id} was not injected.");
            throw new Exception($"Hook not found in {tableName}. {id} was not injected.");
        }
            
    }
}