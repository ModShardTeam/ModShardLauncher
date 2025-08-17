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
    public enum SkillsStatsHook
    {
        BASIC,
        RANGED,
        SWORDS,
        [EnumMember(Value = "2H SWORDS")]
        TWOHANDEDSWORDS,
        [EnumMember(Value = "2H MACES")]
        TWOHANDEDMACES,
        [EnumMember(Value = "2H AXES")]
        TWOHANDEDAXES,
        AXES,
        MACES,
        STAVES,
        SHIELDS,
        DAGGERS,
        [EnumMember(Value = "DUAL WIELDING")]
        DUALWIELDING,
        SPEARS,
        COMBAT,
        ATHLETICS,
        SURVIVAL,
        PYRO,
        GEO,
        ELECTRO,
        ARMOR,
        [EnumMember(Value = "MAGIC MASTERY")]
        MAGICMASTERY,
        UNDEAD,
        [EnumMember(Value = "PROLOGUE STATUES")]
        PROLOGUESTATUES,
        [EnumMember(Value = "PROLOGUE ARCHON")]
        PROLOGUEARCHON,
        PROSELYTES,
        BRIGANDS,
        [EnumMember(Value = "ANCIENT TROLL")]
        ANCIENTTROLL,
        BEASTS,
        MANTICORE
    }
    
    public enum SkillsStatsTarget
    {
        [EnumMember(Value = "No Target")]
        NoTarget,
        [EnumMember(Value = "Target Object")]
        TargetObject,
        [EnumMember(Value = "Target Point")]
        TargetPoint,
        [EnumMember(Value = "Target Area")]
        TargetArea,
        [EnumMember(Value = "Target Ally")]
        TargetAlly
    }
    
    public enum SkillsStatsPattern
    {
        normal,
        five,
        line,
        circle,
        pyramid,
        pyramid_shift
    }

    public enum SkillsStatsValidator
    {
        [EnumMember(Value = "")]
        none,
        AVOID_TILEMARKS,
        DASH
    }
    
    public enum SkillsStatsClass
    {
        skill,
        spell,
        attack,
    }
    
    public enum SkillsStatsBranch
    {
        none, // For some reason the string none has to be written, rather than leaving the field empty. Inconsistent but it is what it is.
        ranged,
        sword,
        [EnumMember(Value = "2hsword")]
        two_handed_sword,
        [EnumMember(Value = "2hmace")]
        two_handed_mace,
        [EnumMember(Value = "2haxe")]
        two_handed_axe,
        axe,
        mace,
        staff,
        shield,
        dagger,
        dual,
        spear,
        combat,
        athletic,
        pyromancy,
        geomancy,
        electromancy,
        armor,
        magic_mastery,
        necromancy,
        sanguimancy
    }
    
    public enum SkillsStatsMetacategory
    {
        [EnumMember(Value = "")]
        none,
        weapon,
        utility
    }
    
    public static void InjectTableSkillsStats(
    SkillsStatsHook hook,
    string id,
    string? Object = null,
    SkillsStatsTarget Target = SkillsStatsTarget.NoTarget,
    string Range = "0",
    ushort KD = 0,
    ushort MP = 0,
    ushort Reserv = 0,
    ushort Duration = 0,
    byte AOE_Lenght = 0,
    byte AOE_Width = 0,
    bool is_movement = false,
    SkillsStatsPattern Pattern = SkillsStatsPattern.normal,
    SkillsStatsValidator Validators = SkillsStatsValidator.none,
    SkillsStatsClass Class = SkillsStatsClass.skill,
    bool Bonus_Range = false, // could be byte ? Not sure as only values are 0 and 1
    string? Starcast = null,
    SkillsStatsBranch Branch = SkillsStatsBranch.none,
    bool is_knockback = false,
    bool Crime = false,
    SkillsStatsMetacategory metacategory = SkillsStatsMetacategory.none,
    short FMB = 0,
    string AP = "x",
    bool Attack = false,
    bool Stance = false,
    bool Charge = false,
    bool Maneuver = false,
    bool Spell = false
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_skills_stats";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{id};{Object};{GetEnumMemberValue(Target)};{Range};{KD};{MP};{Reserv};{Duration};{AOE_Lenght};{AOE_Width};{(is_movement ? "1" : "0")};{Pattern};{GetEnumMemberValue(Validators)};{Class};{(Bonus_Range ? "1" : "0")};{Starcast};{GetEnumMemberValue(Branch)};{(is_knockback ? "1" : "0")};{(Crime ? "1" : "")};{GetEnumMemberValue(metacategory)};{FMB};{AP};{(Attack ? "1" : "")};{(Stance ? "1" : "")};{(Charge ? "1" : "")};{(Maneuver ? "1" : "")};{(Spell ? "1" : "")};";
        
        // Find Hook
        string hookStr = "// " + GetEnumMemberValue(hook);
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(hookStr));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected Skill Stat {id} into {tableName} under {hook}");
        }
        else
        {
            Log.Error($"Cannot find Hook {hook} in table {tableName}");
            throw new Exception($"Hook {hook} not found in table {tableName}");
        }
    }
}