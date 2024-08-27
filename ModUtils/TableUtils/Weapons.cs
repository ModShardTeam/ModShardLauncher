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
    /// <summary>
    /// <para>Enum used in Weapons table. </para>
    /// Defines the group to insert the skill in.
    /// </summary>
    public enum WeaponsMetaGroup
    {
        [EnumMember(Value = "SWORDS - BLADES")]
        BLADES,
        [EnumMember(Value = "SWORDS - CLEAVERS")]
        CLEAVERS,
        [EnumMember(Value = "SWORDS - SABERS")]
        SABERS,
        [EnumMember(Value = "SWORDS - ORCISH")]
        ORCISH,
        [EnumMember(Value = "SWORDS - LEGENDARY")]
        SWORDLEGENDARY,
        HATCHETS,
        AXES,
        MACES,
        HAMMERS,
        FLAILS,
        DAGGERS,
        [EnumMember(Value = "2H SWORDS")]
        TWOHANDEDSWORDS,
        [EnumMember(Value = "BASTARDS")]
        BASTARDSWORDS,
        SPEARS,
        HALBERDS,
        FORKS,
        POLEAXES,
        GLAIVES,
        LONGAXES,
        POLEMACES,
        [EnumMember(Value = "HAMMERS & CLUBS")]
        HAMMERSANDCLUBS,
        [EnumMember(Value = "2H FLAILS")]
        TWOHANDEDFLAILS,
        BOWS,
        LONGBOWS,
        CROSSBOWS,
        STAVES,
        TOOLS
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
        Unique,
        Legendary
    }

    /// <summary>
    /// <para>Enum used in Weapons table. </para>
    /// Defines the material the weapon is made out of.
    /// </summary>
    public enum WeaponsMaterial
    {
        wood,
        metal
    }

    /// <summary>
    /// <para>Enum used in Weapons table. </para>
    /// Defines the rarity of the weapon in description.
    /// </summary>
    public enum WeaponsTags
    {
        special,
        [EnumMember(Value = "special exc")]
        specialexc,
        [EnumMember(Value = "aldor common")]
        aldorcommon,
        [EnumMember(Value = "aldor uncommon")]
        aldoruncommon,
        [EnumMember(Value = "aldor rare")]
        aldorrare,
        [EnumMember(Value = "brynn common")]
        brynncommon,
        [EnumMember(Value = "brynn uncommon")]
        brynnuncommon,
        [EnumMember(Value = "skadia common")]
        skadiacommon,
        [EnumMember(Value = "skadian common")]
        skadiancommon,
        [EnumMember(Value = "elven common")]
        elvencommon,
        [EnumMember(Value = "woodfolk common")]
        woodfolkcommon,
        [EnumMember(Value = "fjall common")]
        fjallcommon,
        [EnumMember(Value = "nistra common")]
        nistracommon,
        [EnumMember(Value = "orc common")]
        orccommon,
        [EnumMember(Value = "aldwynn uncommon")]
        aldwynnuncommon,
        [EnumMember(Value = "foreign uncommon")]
        foreignuncommon,
        [EnumMember(Value = "maen uncommon")]
        maenuncommon,
        [EnumMember(Value = "aldor magic")]
        aldormagic
    }
    
    public static void InjectTableWeapons(
        WeaponsMetaGroup metaGroup,
        string name,
        string id,
        WeaponsSlot Slot,
        WeaponsRarity rarity,
        WeaponsMaterial Mat,
        WeaponsTags tags,
        int MaxDuration,
        int LVL,
        int E,
        int Price,
        int Rng,
        int Balance,
        int? Weapon_Damage = null,
        int? Armor_Damage = null,
        int? Armor_Piercing = null,
        int? Bodypart_Damage = null,
        int? Slashing_Damage = null,
        int? Piercing_Damage = null,
        int? Blunt_Damage = null,
        int? Rending_Damage = null,
        int? Fire_Damage = null,
        int? Shock_Damage = null,
        int? Poison_Damage = null,
        int? Caustic_Damage = null,
        int? Frost_Damage = null,
        int? Arcane_Damage = null,
        int? Unholy_Damage = null,
        int? Sacred_Damage = null,
        int? Psionic_Damage = null,
        int? Hit_Chance = null,
        int? CRT = null,
        int? CRTD = null,
        int? PRR = null,
        int? Block_Power = null,
        int? CTA = null,
        int? FMB = null,
        int? EVS = null,
        int? Bleeding_Chance = null,
        int? Daze_Chance = null,
        int? Stun_Chance = null,
        int? Knockback_Chance = null,
        int? Immob_Chance = null,
        int? Stagger_Chance = null,
        int? MP = null,
        int? MP_Restoration = null,
        int? Cooldown_Reduction = null,
        int? Skills_Energy_Cost = null,
        int? Spells_Energy_Cost = null,
        int? Magic_Power = null,
        int? Miscast_Chance = null,
        int? Miracle_Chance = null,
        int? Miracle_Power = null,
        int? Backfire_Damage = null,
        int? Pyromantic_Power = null,
        int? Geomantic_Power = null,
        int? Venomantic_Power = null,
        int? Electroantic_Power = null,
        int? Cryomantic_Power = null,
        int? Arcanistic_Power = null,
        int? Astromantic_Power = null,
        int? Psimantic_Power = null,
        int? Chronomantic_Power = null,
        int? Health_Restoration = null,
        int? Lifesteal = null,
        int? Manasteal = null,
        int? Bonus_Range = null,
        int? Range_Modifier = null,
        int? Damage_Received = null,
        int? Damage_Returned = null,
        int? Healing_Received = null,
        int? STL = null,
        int? Noise_Produced = null,
        int? Offhand_Efficiency = null,
        int? Slaying_Chance = null,
        bool NoDrop = false
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_weapons";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{name};{id};{GetEnumMemberValue(Slot)};{GetEnumMemberValue(rarity)};{GetEnumMemberValue(Mat)};{MaxDuration};{LVL};{E};{Price};{Rng};{Weapon_Damage};{Armor_Damage};{Armor_Piercing};{Bodypart_Damage};{Slashing_Damage};{Piercing_Damage};{Blunt_Damage};{Rending_Damage};{Fire_Damage};{Shock_Damage};{Poison_Damage};{Caustic_Damage};{Frost_Damage};{Arcane_Damage};{Unholy_Damage};{Sacred_Damage};{Psionic_Damage};{Hit_Chance};{CRT};{CRTD};{PRR};{Block_Power};{CTA};{FMB};{EVS};{Bleeding_Chance};{Daze_Chance};{Stun_Chance};{Knockback_Chance};{Immob_Chance};{Stagger_Chance};{MP};{MP_Restoration};{Cooldown_Reduction};{Skills_Energy_Cost};{Spells_Energy_Cost};{Magic_Power};{Miscast_Chance};{Miracle_Chance};{Miracle_Power};{Backfire_Damage};{Pyromantic_Power};{Geomantic_Power};{Venomantic_Power};{Electroantic_Power};{Cryomantic_Power};{Arcanistic_Power};{Astromantic_Power};{Psimantic_Power};{Chronomantic_Power};{Health_Restoration};{Lifesteal};{Manasteal};{Bonus_Range};{Range_Modifier};{Damage_Received};{Damage_Returned};{Healing_Received};{STL};{Noise_Produced};{Balance};{Offhand_Efficiency};{Slaying_Chance};{GetEnumMemberValue(tags)};{(NoDrop ? "1" : "")};";
        
        // Find Meta Category in table
        string metaGroupStr = ThrowIfNull(GetEnumMemberValue(metaGroup));
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(metaGroupStr));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected Skill Stat {id} into Meta Group {metaGroup}");
        }
        else
        {
            Log.Error($"Cannot find Meta Group {metaGroup} in Skills Stat table");
            throw new Exception("Meta Group not found in Skills Stat table");
        }
    }
}