using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Serilog;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public partial class Msl
{
    public enum EnemyBalanceType
    {
        undead,
        human,
        spectre,
        beast,
        vampire,
        neutral,
        spawner,
        arachnid,
        elf
    }

    public enum EnemyBalanceFaction
    {
        Undead,
        Brigand,
        Vampire,
        Neutral,
        omnivore,
        carnivore,
        herbivore,
        GrandMagistrate,
        RottenWillow
    }

    public enum EnemyBalancePattern
    {
        Melee,
        Ranger,
        Mage
    }

    public enum EnemyBalanceSpawnType1
    {
        Fighter,
        Ranger,
        Mage,
        Boss,
        Assassin,
        Tank,
        Expendable,
        Support
    }

    public enum EnemyBalanceSpawnType2
    {
        [EnumMember(Value = "")]
        None,
        Support,
        Assassin,
        Fighter,
        Mage,
        Tank
    }

    public enum EnemyBalanceWeapon
    {
        sword,
        axe,
        mace,
        spear,
        bow,
        claws,
        dagger,
        staff,
        crossbow,
        [EnumMember(Value = "2hsword")]
        twohandedsword,
        [EnumMember(Value = "2hspear")]
        twohandedspear,
        [EnumMember(Value = "2hmace")]
        twohandedmace,
        [EnumMember(Value = "2haxe")]
        twohandedaxe,
        fangs,
        antlers,
        sting,
        fists
    }

    public enum EnemyBalanceArmor
    {
        Light,
        Medium,
        Heavy
    }

    public enum EnemyBalanceSize
    {
        medium,
        small,
        large,
        tiny,
        giant
    }

    public enum EnemyBalanceMatter
    {
        bones,
        flesh,
        ectoplasm,
        chitin,
        ooze
    }
    
    public static void InjectTableEnemyBalance(
        string name,
        int LVL,
        string ID,
        EnemyBalanceType type,
        EnemyBalanceFaction faction,
        EnemyBalancePattern pattern,
        EnemyBalanceSpawnType1 spawn_type1,
        EnemyBalanceWeapon weapon,
        EnemyBalanceArmor armor,
        EnemyBalanceMatter matter,
        int SU,
        int? EF = null,
        int? XP = 0,
        int HP = 100,
        int MP = 100,
        EnemyBalanceSpawnType2 spawn_type2 = EnemyBalanceSpawnType2.None,
        EnemyBalanceSize size = EnemyBalanceSize.medium,
        int? Head_DEF = null,
        int? Body_DEF = null,
        int? Arms_DEF = null,
        int? Legs_DEF = null,
        int Hit_Chance = 70,
        int? EVS = null,
        int? PRR = null,
        int? Block_Power = null,
        int? Block_Recovery = null,
        int? CRT = null,
        int? CRTD = null,
        int? CTA = null,
        int? FMB = null,
        int? Miscast_Chance = null,
        int? Fortitude = null,
        int? STL = null,
        int Health_Restoration = 10,
        int MP_Restoration = 20,
        int? Cooldown_Reduction = null,
        int? Knockback_Resistance = null,
        int? Stun_Resistance = null,
        int? Bleeding_Resistance = null,
        int? Bleeding_Chance = null,
        int? Stun_Chance = null,
        int? Daze_Chance = null,
        int? Knockback_Chance = null,
        int? Immob_Chance = null,
        int? Stagger_Chance = null,
        int STR = 10,
        int AGL = 10,
        int Vitality = 10,
        int PRC = 10,
        int WIL = 10,
        int? Damage_Returned = null,
        int VIS = 10,
        int? Bonus_Range = null,
        int? Lifesteal = null,
        int? Manasteal = null,
        int? Healing_Received = null,
        int? Avoiding_Chance = null,
        int Head = 1,
        int Torso = 1,
        int Left_Leg = 1 ,
        int Right_Leg = 1,
        int Left_Hand = 1,
        int Right_Hand = 1,
        int? IP = null,
        int? Pain_Resistance = null,
        int? Morale = 0,
        int? Threat_Time = null,
        int? Bodypart_Damage = null,
        int? Magic_Power = null,
        int? Armor_Piercing = null,
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
        int? Physical_Resistance = null,
        int? Natural_Resistance = null,
        int? Magical_Resistance = null,
        int? Slashing_Resistance = null,
        int? Piercing_Resistance = null,
        int? Blunt_Resistance = null,
        int? Rending_Resistance = null,
        int? Fire_Resistance = null,
        int? Shock_Resistance = null,
        int? Poison_Resistance = null,
        int? Caustic_Resistance = null,
        int? Frost_Resistance = null,
        int? Arcane_Resistance = null,
        int? Unholy_Resistance = null,
        int? Sacred_Resistance = null,
        int? Psionic_Resistance = null,
        bool canBlock = false,
        bool canDisarm = true,
        bool canSwim = true,
        int Swimming_cost = 2
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_enemy_balance";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{name};{LVL};{ID};{GetEnumMemberValue(type)};{GetEnumMemberValue(faction)};{GetEnumMemberValue(pattern)};{GetEnumMemberValue(spawn_type1)};{GetEnumMemberValue(spawn_type2)};{GetEnumMemberValue(weapon)};{GetEnumMemberValue(armor)};{SU};{GetEnumMemberValue(size)};{GetEnumMemberValue(matter)};{EF};;{XP};{HP};{MP};{Head_DEF};{Body_DEF};{Arms_DEF};{Legs_DEF};;{Hit_Chance};{EVS};{PRR};{Block_Power};{Block_Recovery};{CRT};{CRTD};{CTA};{FMB};{Miscast_Chance};{Fortitude};{STL};{Health_Restoration};{MP_Restoration};{Cooldown_Reduction};{Knockback_Resistance};{Stun_Resistance};{Bleeding_Resistance};;{Bleeding_Chance};{Stun_Chance};{Daze_Chance};{Knockback_Chance};{Immob_Chance};{Stagger_Chance};;{STR};{AGL};{Vitality};{PRC};{WIL};;{Damage_Returned};{VIS};{Bonus_Range};{Lifesteal};{Manasteal};{Healing_Received};{Avoiding_Chance};;{Head};{Torso};{Left_Leg};{Right_Leg};{Left_Hand};{Right_Hand};{IP};{Pain_Resistance};{Morale};{Threat_Time};;{Bodypart_Damage};{Magic_Power};{Armor_Piercing};{Slashing_Damage};{Piercing_Damage};{Blunt_Damage};{Rending_Damage};{Fire_Damage};{Shock_Damage};{Poison_Damage};{Caustic_Damage};{Frost_Damage};{Arcane_Damage};{Unholy_Damage};{Sacred_Damage};{Psionic_Damage};;{Physical_Resistance};{Natural_Resistance};{Magical_Resistance};;{Slashing_Resistance};{Piercing_Resistance};{Blunt_Resistance};{Rending_Resistance};{Fire_Resistance};{Shock_Resistance};{Poison_Resistance};{Caustic_Resistance};{Frost_Resistance};{Arcane_Resistance};{Unholy_Resistance};{Sacred_Resistance};{Psionic_Resistance};;{(canBlock ? "1": "")};{(canDisarm ? "1": "")};{(canSwim ? "1": "")};{Swimming_cost};";
        
        // Add line to end of table
        table.Add(newline);
        ModLoader.SetTable(table, tableName);
        Log.Information($"Injected {name} into {tableName} table.");
    }
}