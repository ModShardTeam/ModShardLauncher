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
    public enum WeaponsTier
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
    
    /// <summary>
    /// <para>Enum used in Weapons table. </para>
    /// Defines the slot the weapon can be equipped in.
    /// </summary>
    public enum WeaponsSlot
    {
        sword,
        axe,
        mace,
        dagger,
        [EnumMember(Value = "2hsword")]
        twohandedsword,
        spear,
        [EnumMember(Value = "2haxe")]
        twohandedaxe,
        [EnumMember(Value = "2hmace")]
        twohandedmace,
        bow,
        crossbow,
        sling,
        [EnumMember(Value = "2hStaff")]
        twohandedstaff,
        chain,
        lute
    }

    /// <summary>
    /// <para>Enum used in Weapons table. </para>
    /// Defines the rarity of the weapon.
    /// </summary>
    public enum WeaponsRarity
    {
        Common,
        Unique
        // Legendary // Doesn't seem to be used post-RtR 
    }

    /// <summary>
    /// <para>Enum used in Weapons table. </para>
    /// Defines the material the weapon is made out of.
    /// </summary>
    public enum WeaponsMaterial
    {
        wood,
        metal,
        leather
    }

    /// <summary>
    /// <para>Enum used in Weapons table. </para>
    /// Defines the rarity of the weapon in description.
    /// </summary>
    public enum WeaponsTags
    {
        // SPECIAL
        unique,
        magic,
        special,
        [EnumMember(Value = "special exc")]
        specialexc,
        WIP,
        
        // ALDOR
        aldor,
        [EnumMember(Value = "aldor common")]
        aldorcommon,
        [EnumMember(Value = "aldor uncommon")]
        aldoruncommon,
        [EnumMember(Value = "aldor rare")]
        aldorrare,
        [EnumMember(Value = "aldor magic")]
        aldormagic,
        
        // FOREIGN
        fjall,
        elven,
        skadia,
        nistra
    }
    
    public static void InjectTableWeapons(
        // Would love to use a hook but devs fucked up the table's categories' names
        string name,
        WeaponsTier Tier,
        string id,
        WeaponsSlot Slot,
        WeaponsRarity rarity,
        WeaponsMaterial Mat,
        WeaponsTags tags,
        byte Rng = 1,
        int? Price = null,
        byte Markup = 1,
        short? MaxDuration = null,
        short? Armor_Piercing = null,
        short? Armor_Damage = null,
        short? Bodypart_Damage = null,
        short? Slashing_Damage = null,
        short? Piercing_Damage = null,
        short? Blunt_Damage = null,
        short? Rending_Damage = null,
        short? Fire_Damage = null,
        short? Shock_Damage = null,
        short? Poison_Damage = null,
        short? Caustic_Damage = null,
        short? Frost_Damage = null,
        short? Arcane_Damage = null,
        short? Unholy_Damage = null,
        short? Sacred_Damage = null,
        short? Psionic_Damage = null,
        short? FMB = null,
        short? Hit_Chance = null,
        short? CRT = null,
        short? CRTD = null,
        short? CTA = null,
        short? PRR = null,
        short? Block_Power = null,
        short? Block_Recovery = null,
        byte? Bleeding_Chance = null,
        byte? Daze_Chance = null,
        byte? Stun_Chance = null,
        byte? Knockback_Chance = null,
        byte? Immob_Chance = null,
        byte? Stagger_Chance = null,
        short? MP = null,
        short? MP_Restoration = null,
        short? Cooldown_Reduction = null,
        short? Skills_Energy_Cost = null,
        short? Spells_Energy_Cost = null,
        short? Magic_Power = null,
        short? Miscast_Chance = null,
        short? Miracle_Chance = null,
        short? Miracle_Power = null,
        short? Bonus_Range = null,
        short? max_hp = null,
        short? Health_Restoration = null,
        short? Healing_Received = null,
        short? Crit_Avoid = null,
        short? Fatigue_Gain = null,
        short? Lifesteal = null,
        short? Manasteal = null,
        short? Damage_Received = null,
        short? Pyromantic_Power = null,
        short? Geomantic_Power = null,
        short? Venomantic_Power = null,
        short? Electroantic_Power = null,
        short? Cryomantic_Power = null,
        short? Arcanistic_Power = null,
        short? Astromantic_Power = null,
        short? Psimantic_Power = null,
        short? Balance = null, // Could be byte ?
        string? upgrade = null,
        bool fireproof = false,
        bool NoDrop = false
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_weapons";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{name};{GetEnumMemberValue(Tier)};{id};{GetEnumMemberValue(Slot)};{rarity};{Mat};{Price};{Markup};{MaxDuration};{Rng};;{Armor_Piercing};{Armor_Damage};{Bodypart_Damage};;{Slashing_Damage};{Piercing_Damage};{Blunt_Damage};{Rending_Damage};{Fire_Damage};{Shock_Damage};{Poison_Damage};{Caustic_Damage};{Frost_Damage};{Arcane_Damage};{Unholy_Damage};{Sacred_Damage};{Psionic_Damage};;{FMB};{Hit_Chance};{CRT};{CRTD};{CTA};{PRR};{Block_Power};{Block_Recovery};;{Bleeding_Chance};{Daze_Chance};{Stun_Chance};{Knockback_Chance};{Immob_Chance};{Stagger_Chance};;{MP};{MP_Restoration};{Cooldown_Reduction};{Skills_Energy_Cost};{Spells_Energy_Cost};{Magic_Power};{Miscast_Chance};{Miracle_Chance};{Miracle_Power};{Bonus_Range};;{max_hp};{Health_Restoration};{Healing_Received};{Crit_Avoid};{Fatigue_Gain};{Lifesteal};{Manasteal};{Damage_Received};;{Pyromantic_Power};{Geomantic_Power};{Venomantic_Power};{Electroantic_Power};{Cryomantic_Power};{Arcanistic_Power};{Astromantic_Power};{Psimantic_Power};;{Balance};{GetEnumMemberValue(tags)};{upgrade};{(fireproof ? 1 : "")};{(NoDrop ? 1 : "")};";
        
        // Add line to table
        table.Add(newline);
        ModLoader.SetTable(table, tableName);
        Log.Information($"Injected Weapon {id} into table {tableName}");
    }
}