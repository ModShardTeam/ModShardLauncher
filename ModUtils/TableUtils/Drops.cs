using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Serilog;
using Serilog.Debugging;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class Msl
{
    private static string EqTypeParser(HashSet<DropsEqType> eqTypes)
    {
        string eqTypesStr = "";
        foreach (DropsEqType t in eqTypes)
            eqTypesStr += $"{GetEnumMemberValue(t)}, ";
        eqTypesStr = eqTypesStr.TrimEnd(',', ' ');
        return eqTypesStr;
    }
    
    public enum DropsHook
    {
        CRYPTS,
        CATACOMBS,
        BASTIONS,
        OTHER
    }

    public enum DropsTier
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
    public enum DropsSlotTags
    {
        raw = 1,
        common = 2
    }

    public enum DropsEqType
    {
        weapon,
        dagger,
        [EnumMember(Value = "2HStaff")]
        TwoHandedStaff,
        Chest,
        Legs,
        Waist,
        Arms,
        Head,
        jewelry,
        sword,
        mace,
        axe,
        spear,
        crossbow
    }
    
    [Flags]
    public enum DropsEqTags
    {
        aldor = 1,
        magic = 2,
        elven = 4,
        unique = 8
    }
    
    [Flags]
    public enum DropsEqRarity
    {
        common = 1,
        uncommon = 2,
        rare = 4,
        unique = 8
    }

    public record DropsSlot(string id, DropsSlotTags? tags, string count, byte chance);
    public record DropsEq(HashSet<DropsEqType> types, DropsEqTags tags, DropsEqRarity? rarity, string dur, byte chance);
    
    public static void InjectTableDrops(
        DropsHook hook,
        string id,
        DropsTier tier,
        HashSet<DropsTier>? tierMod = null,
        DropsSlot? slot_1 = null,
        DropsSlot? slot_2 = null,
        DropsSlot? slot_3 = null,
        DropsSlot? slot_4 = null,
        DropsSlot? slot_5 = null,
        DropsSlot? slot_6 = null,
        DropsSlot? slot_7 = null,
        DropsSlot? slot_8 = null,
        DropsSlot? slot_9 = null,
        DropsEq? eq_1 = null,
        DropsEq? eq_2 = null,
        DropsEq? eq_3 = null,
        DropsEq? eq_4 = null,
        DropsEq? eq_5 = null
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_drops";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Parse tier
        string tierModStr = "";
        if (tierMod != null)
        {
            foreach (SurfaceSpawnTier t in tierMod)
                tierModStr += $"{GetEnumMemberValue(t)}, ";
            tierModStr = tierModStr.TrimEnd(',', ' ');
        }
        
        // Prepare line
        string newline = $"{id};{GetEnumMemberValue(tier)};{tierModStr};{slot_1?.id};{slot_1?.tags};{slot_1?.count};{slot_1?.chance};;{slot_2?.id};{slot_2?.tags};{slot_2?.count};{slot_2?.chance};;{slot_3?.id};{slot_3?.tags};{slot_3?.count};{slot_3?.chance};;{slot_4?.id};{slot_4?.tags};{slot_4?.count};{slot_4?.chance};;{slot_5?.id};{slot_5?.tags};{slot_5?.count};{slot_5?.chance};;{slot_6?.id};{slot_6?.tags};{slot_6?.count};{slot_6?.chance};;{slot_7?.id};{slot_7?.tags};{slot_7?.count};{slot_7?.chance};;{slot_8?.id};{slot_8?.tags};{slot_8?.count};{slot_8?.chance};;{slot_9?.id};{slot_9?.tags};{slot_9?.count};{slot_9?.chance};;{(eq_1 != null ? EqTypeParser(eq_1.types) : "")};{eq_1?.tags};{eq_1?.rarity};{eq_1?.dur};{eq_1?.chance};;{(eq_2 != null ? EqTypeParser(eq_2.types) : "")};{eq_2?.rarity};{eq_2?.dur};{eq_2?.chance};;{(eq_3 != null ? EqTypeParser(eq_3.types) : "")};{eq_3?.rarity};{eq_3?.dur};{eq_3?.chance};;{(eq_4 != null ? EqTypeParser(eq_4.types) : "")};{eq_4?.rarity};{eq_4?.dur};{eq_4?.chance};;{(eq_5 != null ? EqTypeParser(eq_5.types) : "")};{eq_5?.rarity};{eq_5?.dur};{eq_5?.chance};";
        
        // Find hook
        string hookStr = "// " + hook;
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hookStr));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected Drop {id} into table {tableName} under {hook}");
        }
        else
        {
            Log.Error($"Cannot find hook {hook} in table {tableName}");
            throw new Exception($"Cannot find hook {hook} in table {tableName}");
        }
    }
}