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
    public enum ContractCategory
    {
        rescue,
        killing,
        search,
        destroy,
        clear,
        interact,
        find
    }

    public enum ContractFaction
    {
        o_Bandit,
        o_Undead,
        o_vamp
    }

    public enum ContractDungeonType
    {
        Bastion,
        Crypt,
        Catacombs
    }

    public enum ContractVillageType
    {
        all,
        r_Osbrook,
        r_Brynn_NW
    }
    
    public static void InjectTableContract(
        string? name,
        string id,
        ContractCategory Category,
        ContractFaction Faction,
        int Time,
        string? Script,
        int Gold,
        ContractDungeonType DungeonType,
        int? Position = null,
        int Amount = 1,
        ContractVillageType Village_Type = ContractVillageType.all,
        int RepVillage = 400,
        int RepFaction = 80,
        int BadRepVillage = -400,
        int BadRepFaction = -160,
        int? BadDangerMod = null,
        int? ModTime = null
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_Contract";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{Position};{id};{Amount};{GetEnumMemberValue(Category)};{GetEnumMemberValue(Faction)};{Time};{GetEnumMemberValue(DungeonType)};{Script};{Gold};{GetEnumMemberValue(Village_Type)};{RepVillage};{RepFaction};{BadRepVillage};{BadRepFaction};{BadDangerMod};;{name};;;{ModTime};";
        
        // Find hook
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains("NEW CONTRACTS"));
        
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