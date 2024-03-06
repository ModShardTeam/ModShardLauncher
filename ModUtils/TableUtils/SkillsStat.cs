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
    /// <para>Enum used in Skill Stats table. </para>
    /// Defines the group to insert the skill in.
    /// </summary>
    public enum SkillsStatMetaGroup
    {
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
        ATHLETIC,
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
    /// <summary>
    /// <para>Enum used in Skill Stats table. </para>
    /// Defines the type of targeting the skill uses.
    /// </summary>
    public enum SkillsStatTarget
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
    /// <summary>
    /// <para>Enum used in Skill Stats table. </para>
    /// Defines the pattern the skill uses.
    /// </summary>
    public enum SkillsStatPattern
    {
        normal,
        five,
        line,
        circle,
        pyramid,
        pyramid_shift
    }
    /// <summary>
    /// <para>Enum used in Skill Stats table. </para>
    /// Defines the class of the skill.
    /// </summary>
    public enum SkillsStatClass
    {
        skill,
        spell,
        attack,
    }
    /// <summary>
    /// <para>Enum used in Skill Stats table. </para>
    /// The skill tree this skill belongs to.
    /// </summary>
    public enum SkillsStatBranch
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
    /// <summary>
    /// <para>Enum used in Skill Stats table. </para>
    /// The metacategory the skill belongs to.
    /// </summary>
    public enum SkillsStatMetacategory
    {
        [EnumMember(Value = "")]
        none,
        weapon,
        utility
    }
    
    /// <summary>
    /// Adds a line to the Skills Stat table with the given parameters.
    /// </summary>
    /// <param name="metaGroup">The group in which to inject the skill.</param>
    /// <param name="id">The unique id of the skill.</param>
    /// <param name="Object">The name of the object this skill is attached to.</param>
    /// <param name="Target">The targeting mode for this skill.</param>
    /// <param name="Range">The range of this skill. (Can be a number, `range` or `vis`).</param>
    /// <param name="KD">The cooldown for this skill.</param>
    /// <param name="MP">The amount of energy used by this skill.</param>
    /// <param name="Reserv"></param>
    /// <param name="Duration">The duration of the buff if the Object is a buff.</param>
    /// <param name="AOE_Lenght">The length of the area when using Target Area.</param>
    /// <param name="AOE_Width">The width of the area when using Target Area.</param>
    /// <param name="is_movement">Whether this skill is a movement skill.</param>
    /// <param name="Pattern">The pattern this skill uses.</param>
    /// <param name="Class">The class of the skill.</param>
    /// <param name="Bonus_Range">Whether this skill receives bonus range.</param>
    /// <param name="Starcast">The effect to play when using the skill.</param>
    /// <param name="Branch">The skill tree the skill belongs to.</param>
    /// <param name="is_knockback">Whether this skill can cause knockback.</param>
    /// <param name="Crime">Whether using this skill is considered a crime.</param>
    /// <param name="metacategory">The metacategory this skill belongs to.</param>
    /// <param name="FMB">The backfire chance when using this skill if it's a spell.</param>
    /// <param name="AP">The amount of armor piercing the skill has. (Can be `x` for default)</param>
    /// <param name="Attack">Whether this skill is an attack.</param>
    /// <param name="Stance">Whether this skill is a stance.</param>
    /// <param name="Charge">Whether this skill is a charge.</param>
    /// <param name="Maneuver">Whether this skill is a maneuver.</param>
    /// <param name="Spell">Whether this skill is a spell.</param>
    public static void InjectTableSkillsStat(
        SkillsStatMetaGroup metaGroup,
        string id,
        string Object,
        SkillsStatTarget Target = SkillsStatTarget.NoTarget,
        string Range = "0",
        int KD = 0,
        int MP = 0,
        int Reserv = 0,
        int Duration = 0,
        int AOE_Lenght = 0,
        int AOE_Width = 0,
        bool is_movement = false,
        SkillsStatPattern Pattern = SkillsStatPattern.normal,
        SkillsStatClass Class = SkillsStatClass.skill,
        bool Bonus_Range = false,
        string Starcast = "",
        SkillsStatBranch Branch = SkillsStatBranch.none,
        bool is_knockback = false,
        bool Crime = false,
        SkillsStatMetacategory metacategory = SkillsStatMetacategory.none,
        int FMB = 0,
        string AP = "x",
        bool Attack = false,
        bool Stance = false,
        bool Charge = false,
        bool Maneuver = false,
        bool Spell = false)
    {
        // Load table if it exists
        List<string> table = Msl.ThrowIfNull(ModLoader.GetTable("gml_GlobalScript_table_skills_stat"));
        
        // Prepare line
        string newline = $"{id};{Object};{GetEnumMemberValue(Target)};{Range};{KD};{MP};{Reserv};{Duration};{AOE_Lenght};{AOE_Width};{is_movement};{Pattern};{Class};{Bonus_Range};{Starcast};{GetEnumMemberValue(Branch)};{is_knockback};{Crime};{GetEnumMemberValue(metacategory)};{FMB};{AP};{Attack};{Stance};{Charge};{Maneuver};{Spell}";
        
        // Find Meta Category in table
        string? foundLine = table.FirstOrDefault(line => line.Contains(GetEnumMemberValue(metaGroup)));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(table.IndexOf(foundLine) + 1, newline);
            ModLoader.SetTable(table, "gml_GlobalScript_table_skills_stat");
        }
        else
        {
            Log.Error($"Cannot find Meta Group {metaGroup} in Skills Stat table");
            throw new Exception("Meta Group not found in Skills Stat table");
        }
    }
}