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
    public enum ArmorMetaGroup
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
        W,
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
        Unique,
        Legendary
    }
    
    public enum ArmorMaterial
    {
        wood,
        leather,
        metal,
        cloth
    }
    
    public enum ArmorTags
    {
        [EnumMember(Value = "aldor common")]
        aldorcommon,
        [EnumMember(Value = "aldor uncommon")]
        aldoruncommon,
        [EnumMember(Value = "aldor rare")]
        aldorrare,
        [EnumMember(Value = "aldor magic")]
        aldormagic,
        [EnumMember(Value = "fjall common")]
        fjallcommon,
        [EnumMember(Value = "fjall uncommon")]
        fjaluncommon,
        [EnumMember(Value = "elven common")]
        elvencommon,
        [EnumMember(Value = "elven uncommon")]
        elvenuncommon,
        [EnumMember(Value = "elven rare")]
        elvenrare,
        [EnumMember(Value = "skadia common")]
        skadiacommon,
        [EnumMember(Value = "skadia uncommon")]
        skadiauncommon,
        [EnumMember(Value = "nistra common")]
        nistracommon,
        [EnumMember(Value = "foreign common")]
        foreigncommon,
        [EnumMember(Value = "brynn common")]
        bryncommon,
        [EnumMember(Value = "norse common")]
        norsecommon,
        [EnumMember(Value = "aldwynn uncommon")]
        aldwynnuncommon,
        [EnumMember(Value = "maen uncommon")]
        maenuncommon,
        special,
        [EnumMember(Value = "special exc")]
        specialexc
    }
    public static void InjectTableArmor(
        ArmorMetaGroup metaGroup,
        string name,
        int LVL,
        string id,
        ArmorSlot Slot,
        ArmorClass Class,
        ArmorRarity rarity,
        ArmorMaterial Mat,
        ArmorTags tags,
        bool IsOpen = false,
        bool NoDrop = false,
        int? Price = null,
        int? MaxDuration = null,
        int? DEF = null,
        int? PRR = null,
        int? Block_Power = null,
        int? Block_Recovery = null,
        int? EVS = null,
        int? VSN = null,
        int? FMB = null,
        int? MP = null,
        int? MP_Restoration = null,
        int? Skills_Energy_Cost = null,
        int? Spells_Energy_Cost = null,
        int? Magic_Power = null,
        int? Backfire_Damage = null,
        int? Miscast_Chance = null,
        int? Miracle_Chance = null,
        int? Miracle_Power = null,
        int? Damage_Received = null,
        int? Cooldown_Reduction = null,
        int? Hit_Chance = null,
        int? CRT = null,
        int? CRTD = null,
        int? CTA = null,
        int? max_hp = null,
        int? Health_Restoration = null,
        int? Healing_Received = null,
        int? Lifesteal = null,
        int? Manasteal = null,
        int? STL = null,
        int? Noise_Produced = null,
        int? Fortitude = null,
        int? Savvy = null,
        int? Damage_Returned = null,
        int? Received_XP = null,
        int? Knockback_Resistance = null,
        int? Bleeding_Resistance = null,
        int? Stun_Resistance = null,
        int? Pain_Resistance = null,
        int? Fatigue_Gain = null,
        int? Pyromantic_Power = null,
        int? Geomantic_Power = null,
        int? Venomantic_Power = null,
        int? Electromantic_Power = null,
        int? Cryomantic_Power = null,
        int? Arcanistic_Power = null,
        int? Astromantic_Power = null,
        int? Psimantic_Power = null,
        int? Chronomantic_Power = null,
        int? Physical_Resistance = null,
        int? Nature_Resistance = null,
        int? Magic_Resistance = null,
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
        int? Received_Experience = null,
        int? fragment_cloth01 = null,
        int? fragment_cloth02 = null,
        int? fragment_cloth03 = null,
        int? fragment_cloth04 = null,
        int? fragment_leather01 = null,
        int? fragment_leather02 = null,
        int? fragment_leather03 = null,
        int? fragment_leather04 = null,
        int? fragment_metal01 = null,
        int? fragment_metal02 = null,
        int? fragment_metal03 = null,
        int? fragment_metal04 = null,
        int? fragment_gold = null,
        int? dur_per_frag = null
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_armor";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{name};{LVL};{id};{GetEnumMemberValue(Slot)};{GetEnumMemberValue(Class)};{GetEnumMemberValue(rarity)};{GetEnumMemberValue(Mat)};{Price};{MaxDuration};{DEF};;{PRR};{Block_Power};{Block_Recovery};{EVS};{VSN};{FMB};{MP};{MP_Restoration};{Skills_Energy_Cost};{Spells_Energy_Cost};{Magic_Power};{Backfire_Damage};{Miscast_Chance};{Miracle_Chance};{Miracle_Power};{Damage_Received};{Cooldown_Reduction};{Hit_Chance};{CRT};{CRTD};{CTA};{max_hp};{Health_Restoration};{Healing_Received};{Lifesteal};{Manasteal};{STL};{Noise_Produced};{Fortitude};{Savvy};{Damage_Returned};{Received_XP};;{Knockback_Resistance};{Bleeding_Resistance};{Stun_Resistance};{Pain_Resistance};{Fatigue_Gain};;{Pyromantic_Power};{Geomantic_Power};{Venomantic_Power};{Electromantic_Power};{Cryomantic_Power};{Arcanistic_Power};{Astromantic_Power};{Psimantic_Power};{Chronomantic_Power};;{Physical_Resistance};{Nature_Resistance};{Magic_Resistance};{Slashing_Resistance};{Piercing_Resistance};{Blunt_Resistance};{Rending_Resistance};{Fire_Resistance};{Shock_Resistance};{Poison_Resistance};{Caustic_Resistance};{Frost_Resistance};{Arcane_Resistance};{Unholy_Resistance};{Sacred_Resistance};{Psionic_Resistance};{Received_Experience};{GetEnumMemberValue(tags)};{(IsOpen ? "1" : "")};{(NoDrop ? "1" : "")};{fragment_cloth01};{fragment_cloth02};{fragment_cloth03};{fragment_cloth04};{fragment_leather01};{fragment_leather02};{fragment_leather03};{fragment_leather04};{fragment_metal01};{fragment_metal02};{fragment_metal03};{fragment_metal04};{fragment_gold};{dur_per_frag};";
        
        // Find Meta Category in table
        string metaGroupStr = "// " + GetEnumMemberValue(metaGroup);
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(metaGroupStr));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
            Log.Information($"Injected Armor {id} into Meta Group {metaGroup}");
        }
        else
        {
            Log.Error($"Cannot find Meta Group {metaGroup} in {tableName} table");
            throw new Exception($"Cannot find Meta Group {metaGroup} in {tableName} table");
        }
        
    }
}