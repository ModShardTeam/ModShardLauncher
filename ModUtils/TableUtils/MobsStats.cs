using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Serilog;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class Msl
{
    public enum MobsStatsTier
    {
        [EnumMember(Value = "0")]
        Tier0,
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

    public enum MobsStatsType
    {
        undead,
        human,
        spectre,
        spawner,
        vampire,
        beast,
        arachnid,
        elf,
        dwarf
    }
    
    public enum MobsStatsFaction
    {
        Undead,
        Vampire,
        Brigand,
        omnivore,
        carnivore,
        herbivore,
        GrandMagistrate,
        RottenWillow,
        Mercenary,
        Neutral
    }
    
    public enum MobsStatsPattern
    {
        Melee,
        Ranger,
        Mage
    }

    public enum MobsStatsCategory
    {
        [EnumMember(Value = "")]
        none,
        Expendable,
        Tech,
        Fighter,
        Ranger,
        Mage,
        Support,
        Tank,
        Assassin,
        Boss
    }

    public enum MobsStatsWeapon
    {
        claws,
        axe,
        dagger,
        spear,
        sword,
        mace,
        bow,
        crossbow,
        [EnumMember(Value = "2hmace")]
        twohandedmace,
        [EnumMember(Value = "2haxe")]
        twohandedaxe,
        [EnumMember(Value = "2hsword")]
        twohandedsword,
        [EnumMember(Value = "2hStaff")]
        twohandedstaff,
        fangs,
        sting,
        antlers,
        fists
    }

    public enum MobsStatsArmor
    {
        Light,
        Medium,
        Heavy
    }
    
    public enum MobsStatsSize
    {
        tiny,
        small,
        medium,
        large,
        giant
    }

    public enum MobsStatsMatter
    {
        flesh,
        bones,
        ectoplasm,
        ooze,
        chitin,
        other
    }
    
    public static void InjectTableMobsStats(
        string name,
        MobsStatsTier tier,
        MobsStatsType type,
        MobsStatsFaction faction,
        MobsStatsPattern pattern,
        MobsStatsWeapon weapon,
        MobsStatsArmor armor,
        MobsStatsSize size,
        short HP,
        byte Hit_Chance,
        byte STR,
        byte AGL,
        byte Vitality,
        byte PRC,
        byte WIL,
        short XP,
        MobsStatsMatter matter,
        MobsStatsCategory category1 = MobsStatsCategory.none,
        MobsStatsCategory category2 = MobsStatsCategory.none,
        string? ID = null,
        short? MP = null,
        ushort? Morale = null,
        byte? VIS = null,
        byte? Head_DEF = null,
        byte? Body_DEF = null,
        byte? Arms_DEF = null,
        byte? Legs_DEF = null,
        short? EVS = null,
        byte? PRR = null,
        byte? Block_Power = null,
        byte? Block_Recovery = null,
        byte? Crit_Avoid = null,
        byte? CRT = null,
        byte? CRTD = null,
        short? CTA = null,
        short? FMB = null,
        short? Magic_Power = null,
        short? Miscast_Chance = null,
        byte? Miracle_Chance = null,
        byte? Miracle_Power = null,
        byte? MP_Restoration = null,
        short? Cooldown_Reduction = null,
        short? Fortitude = null,
        short? Health_Restoration = null,
        byte? Healing_Received = null,
        byte? Lifesteal = null,
        byte? Manasteal = null,
        short? Bleeding_Resistance = null,
        short? Knockback_Resistance = null,
        short? Stun_Resistance = null,
        short? Pain_Resistance = null,
        short? Bleeding_Chance = null,
        short? Daze_Chance = null,
        short? Stun_Chance = null,
        short? Knockback_Chance = null,
        short? Immob_Chance = null,
        short? Stagger_Chance = null,
        float? STRk = null,
        float? AGLk = null,
        float? Vitalityk = null,
        float? PRCk = null,
        float? WILk = null,
        float? Checksum = null,
        short? Bonus_Range = null,
        byte? Avoiding_Chance = null,
        byte? Damage_Returned = null,
        short? Damage_Received = null, // unknown type, assuming short
        byte? Head = null,
        byte? Torso = null,
        byte? Left_Leg = null,
        byte? Right_Leg = null,
        byte? Left_Hand = null,
        byte? Right_Hand = null,
        ushort? IP = null,
        byte? Threat_Time = null,
        byte? Bodypart_Damage = null,
        byte? Armor_Piercing = null,
        byte? DMG_Sum = null,
        byte? Slashing_Damage = null,
        byte? Piercing_Damage = null,
        byte? Blunt_Damage = null,
        byte? Rending_Damage = null,
        byte? Fire_Damage = null,
        byte? Shock_Damage = null,
        byte? Poison_Damage = null,
        byte? Caustic_Damage = null,
        byte? Frost_Damage = null,
        byte? Arcane_Damage = null,
        byte? Unholy_Damage = null,
        byte? Sacred_Damage = null,
        byte? Psionic_Damage = null,
        short? Physical_Resistance = null,
        short? Natural_Resistance = null,
        short? Magical_Resistance = null,
        short? Slashing_Resistance = null,
        short? Piercing_Resistance = null,
        short? Blunt_Resistance = null,
        short? Rending_Resistance = null,
        short? Fire_Resistance = null,
        short? Shock_Resistance = null,
        short? Poison_Resistance = null,
        short? Frost_Resistance = null,
        short? Caustic_Resistance = null,
        short? Arcane_Resistance = null,
        short? Unholy_Resistance = null,
        short? Sacred_Resistance = null,
        short? Psionic_Resistance = null,
        bool canBlock = false,
        bool canDisarm = false,
        bool canSwim = false,
        byte Swimming_Cost = 1,
        byte? achievement = null
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_mobs_stats";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{name};{GetEnumMemberValue(tier)};{ID};{type};{faction};{pattern};;{GetEnumMemberValue(category1)};{GetEnumMemberValue(category2)};{GetEnumMemberValue(weapon)};{armor};{size};{matter};{VIS};;{XP};{HP};{MP};{Head_DEF};{Body_DEF};{Arms_DEF};{Legs_DEF};;{Hit_Chance};{EVS};{PRR};{Block_Power};{Block_Recovery};{Crit_Avoid};{CRT};{CRTD};{CTA};{FMB};;{Magic_Power};{Miscast_Chance};{Miracle_Chance};{Miracle_Power};;{MP_Restoration};{Cooldown_Reduction};{Fortitude};{Health_Restoration};{Healing_Received};{Lifesteal};{Manasteal};;{Bleeding_Resistance};{Knockback_Resistance};{Stun_Resistance};{Pain_Resistance};;{Bleeding_Chance};{Daze_Chance};{Stun_Chance};{Knockback_Chance};{Immob_Chance};{Stagger_Chance};;{STRk};{AGLk};{Vitalityk};{PRCk};{WILk};{Checksum};;{STR};{AGL};{Vitality};{PRC};{WIL};;{Bonus_Range};{Avoiding_Chance};{Damage_Returned};{Damage_Received};;{Head};{Torso};{Left_Leg};{Right_Leg};{Left_Hand};{Right_Hand};;{IP};{Morale};{Threat_Time};;{Bodypart_Damage};{Armor_Piercing};{DMG_Sum};{Slashing_Damage};{Piercing_Damage};{Blunt_Damage};{Rending_Damage};{Fire_Damage};{Shock_Damage};{Poison_Damage};{Caustic_Damage};{Frost_Damage};{Arcane_Damage};{Unholy_Damage};{Sacred_Damage};{Psionic_Damage};;{Physical_Resistance};{Natural_Resistance};{Magical_Resistance};;{Slashing_Resistance};{Piercing_Resistance};{Blunt_Resistance};{Rending_Resistance};{Fire_Resistance};{Shock_Resistance};{Poison_Resistance};{Frost_Resistance};{Caustic_Resistance};{Arcane_Resistance};{Unholy_Resistance};{Sacred_Resistance};{Psionic_Resistance};;{(canBlock ? "1": "")};{(canDisarm ? "1": "")};{(canSwim ? "1": "")};{Swimming_Cost};{achievement};";
        
        // Add line to table
        table.Add(newline);
        ModLoader.SetTable(table, tableName);
        Log.Information($"Injected Mob Stat {name} into table");
    }
}