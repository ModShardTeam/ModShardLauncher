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
    public enum ArmorTier
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
    
    public enum ArmorHook
    {
        SHIELDS,
        HELMETS,
        CHESTPIECES,
        GLOVES,
        BOOTS,
        BELTS,
        RINGS,
        NECKLACES,
        CLOAKS
    }
    public enum ArmorSlot
    {
        shield,
        Head,
        Chest,
        Arms,
        Legs,
        Waist,
        Ring,
        Amulet,
        Back
    }
    
    public enum ArmorClass
    {
        Light,
        Medium,
        Heavy
    }
    
    public enum ArmorRarity
    {
        Common,
        Unique
        // Legendary // seemingly removed with RtR
    }
    
    public enum ArmorMaterial
    {
        wood,
        leather,
        metal,
        cloth,
        silver,
        gold,
        gem
    }
    
    public enum ArmorTags
    {
        aldor,
        fjall,
        elven,
        special,
        unique,
        skadia,
        nistra,
        WIP,
        magic,
        [EnumMember(Value = "special exc")]
        specialexc
    }
    public static void InjectTableArmor(
        ArmorHook hook,
        string name,
        ArmorTier Tier,
        string id,
        ArmorSlot Slot,
        ArmorClass Class,
        ArmorRarity rarity,
        ArmorMaterial Mat,
        ArmorTags tags,
        ushort MaxDuration,
        int Price, // could be nullable ? Only shackles have no price 
        float Markup = 1.0f,
        byte? DEF = null,
        byte? PRR = null,
        byte? Block_Power = null,
        short? Block_Recovery = null,
        short? EVS = null,
        byte? Crit_Avoid = null,
        short? FMB = null,
        short? Hit_Chance = null,
        short? Weapon_Damage = null,
        short? Armor_Piercing = null,
        short? Armor_Damage = null,
        short? CRT = null,
        short? CRTD = null,
        short? CTA = null,
        short? Damage_Received = null,
        short? Fortitude = null,
        short? MP = null,
        short? MP_Restoration = null,
        short? Skills_Energy_Cost = null,
        short? Spells_Energy_Cost = null,
        short? Magic_Power = null,
        short? Miscast_Chance = null,
        short? Miracle_Chance = null,
        short? Miracle_Power = null,
        short? Cooldown_Reduction = null,
        short? VSN = null,
        short? max_hp = null,
        short? Health_Restoration = null,
        short? Healing_Received = null,
        short? Lifesteal = null,
        short? Manasteal = null,
        short? Bonus_Range = null,
        short? Received_XP = null,
        short? Damage_Returned = null,
        short? Bleeding_Resistance = null,
        short? Knockback_Resistance = null,
        short? Stun_Resistance = null,
        short? Pain_Resistance = null,
        short? Fatigue_Gain = null,
        short? Physical_Resistance = null,
        short? Nature_Resistance = null,
        short? Magic_Resistance = null,
        short? Slashing_Resistance = null,
        short? Piercing_Resistance = null,
        short? Blunt_Resistance = null,
        short? Rending_Resistance = null,
        short? Fire_Resistance = null,
        short? Shock_Resistance = null,
        short? Poison_Resistance = null,
        short? Caustic_Resistance = null,
        short? Frost_Resistance = null,
        short? Arcane_Resistance = null,
        short? Unholy_Resistance = null,
        short? Sacred_Resistance = null,
        short? Psionic_Resistance = null,
        short? Pyromantic_Power = null,
        short? Geomantic_Power = null,
        short? Venomantic_Power = null,
        short? Electromantic_Power = null,
        short? Cryomantic_Power = null,
        short? Arcanistic_Power = null,
        short? Astromantic_Power = null,
        short? Psimantic_Power = null,
        bool fireproof = false,
        bool IsOpen = false,
        bool NoDrop = false, // type unknown, assuming bool
        byte? fragment_cloth01 = null,
        byte? fragment_cloth02 = null,
        byte? fragment_cloth03 = null,
        byte? fragment_cloth04 = null,
        byte? fragment_leather01 = null,
        byte? fragment_leather02 = null,
        byte? fragment_leather03 = null,
        byte? fragment_leather04 = null,
        byte? fragment_metal01 = null,
        byte? fragment_metal02 = null,
        byte? fragment_metal03 = null,
        byte? fragment_metal04 = null,
        byte? fragment_gold = null,
        short? dur_per_frag = null
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_armor";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{name};{GetEnumMemberValue(Tier)};{id};{Slot};{Class};{rarity};{Mat};{Price};{Markup};{MaxDuration};{DEF};;{PRR};{Block_Power};{Block_Recovery};{EVS};{Crit_Avoid};{FMB};{Hit_Chance};{Weapon_Damage};{Armor_Piercing};{Armor_Damage};{CRT};{CRTD};{CTA};{Damage_Received};{Fortitude};;{MP};{MP_Restoration};{Skills_Energy_Cost};{Spells_Energy_Cost};{Magic_Power};{Miscast_Chance};{Miracle_Chance};{Miracle_Power};{Cooldown_Reduction};;{VSN};{max_hp};{Health_Restoration};{Healing_Received};{Lifesteal};{Manasteal};{Bonus_Range};{Received_XP};{Damage_Returned};;{Bleeding_Resistance};{Knockback_Resistance};{Stun_Resistance};{Pain_Resistance};{Fatigue_Gain};;{Physical_Resistance};{Nature_Resistance};{Magic_Resistance};;{Slashing_Resistance};{Piercing_Resistance};{Blunt_Resistance};{Rending_Resistance};{Fire_Resistance};{Shock_Resistance};{Poison_Resistance};{Caustic_Resistance};{Frost_Resistance};{Arcane_Resistance};{Unholy_Resistance};{Sacred_Resistance};{Psionic_Resistance};;{Pyromantic_Power};{Geomantic_Power};{Venomantic_Power};{Electromantic_Power};{Cryomantic_Power};{Arcanistic_Power};{Astromantic_Power};{Psimantic_Power};;{GetEnumMemberValue(tags)};{(fireproof ? "1" : "")};{(IsOpen ? "1" : "")};{(NoDrop ? "1" : "")};{fragment_cloth01};{fragment_cloth02};{fragment_cloth03};{fragment_cloth04};{fragment_leather01};{fragment_leather02};{fragment_leather03};{fragment_leather04};{fragment_metal01};{fragment_metal02};{fragment_metal03};{fragment_metal04};{fragment_gold};{dur_per_frag};";
        
        // Find Meta Category in table
        string hookStr = "";
        
        if (hook == ArmorHook.CHESTPIECES)
            hookStr = "// " + GetEnumMemberValue(hook); // Necessary workaround for chestpieces as the devs messed up
        else
            hookStr = "[ " + GetEnumMemberValue(hook) + " ]";
        
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hookStr));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected Armor {id} into table {tableName} under {hook}");
        }
        else
        {
            Log.Error($"Cannot find hook {hook} in table {tableName}");
            throw new Exception($"Cannot find hook {hook} in table {tableName}");
        }
    }
}