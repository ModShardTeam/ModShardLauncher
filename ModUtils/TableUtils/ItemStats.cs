using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]

public partial class Msl
{
    public enum ItemStatsTier
    {
        [EnumMember(Value = "")]
        none,
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
    
    public enum ItemStatsCategory
    {
        [EnumMember(Value = "")]
        none,
        medicine,
        beverage,
        junk,
        tool,
        drug,
        alcohol,
        ammo,
        valuable,
        ingredient,
        food,
        trophy,
        commodity,
        material,
        additive,
        resource,
        upgrade,
        flag,
        bag,
        quest,
        scroll,
        book,
        treasure,
        recipe,
        schematic
    }

    public enum ItemStatsSubcategory
    {
        [EnumMember(Value = "")]
        none,
        hide,
        ingredient,
        alchemy,
        gem,
        potion,
        meat,
        meat_large,
        fish,
        vegetable,
        fruit,
        berry,
        herb,
        mushroom,
        dairy,
        pastry,
        dish,
        quest,
        bird,
        treatise
    }
    
    public enum ItemStatsMaterial
    {
        cloth,
        glass,
        organic,
        metal,
        wood,
        leather,
        gold,
        pottery,
        gem,
        silver,
        stone,
        paper
    }
    
    public enum ItemStatsWeight
    {
        Light,
        Medium,
        [EnumMember(Value = "Very Light")]
        VeryLight,
        Heavy,
        Net
    }

    public enum ItemStatsTags
    {
        [EnumMember(Value = "")]
        // special
        none,
        special,
        WIP,
        
        // rarities
        common,
        uncommon,
        rare,
        unique,
        
        // elven
        elven,
        [EnumMember(Value = "elven common")]
        elvencommon,
        [EnumMember(Value = "elven uncommon")]
        elvenuncommon,
        [EnumMember(Value = "elven rare")]
        elvenrare,
        
        // animals
        [EnumMember(Value = "common animal")]
        commonanimal,
        [EnumMember(Value = "uncommon animal")]
        uncommonanimal,
        [EnumMember(Value = "rare animal")]
        rareanimal,
        
        // alchemy
        alchemy,
        [EnumMember(Value = "common alchemy")]
        commonalchemy,
        [EnumMember(Value = "uncommon alchemy")]
        uncommonalchemy,
        
        // food
        [EnumMember(Value = "common raw")]
        commonraw,
        [EnumMember(Value = "uncommon raw")]
        uncommonraw,
        [EnumMember(Value = "rare raw")]
        rareraw,
        [EnumMember(Value = "common cooked")]
        commoncooked,
        [EnumMember(Value = "uncommon cooked")]
        uncommoncooked,
        [EnumMember(Value = "rare cooked")]
        rarecooked,
        
        // dungeons
        crypt,
        catacombs,
        bastion
    }
    
    public static void InjectTableItemStats(
        string id,
        ItemStatsMaterial Material,
        ItemStatsWeight Weight,
        int? Price = null,
        int? EffPrice = null,
        ItemStatsTier tier = ItemStatsTier.none,
        ItemStatsCategory Cat = ItemStatsCategory.none,
        ItemStatsSubcategory Subcat = ItemStatsSubcategory.none,
        ushort? Fresh = null,
        ushort? Duration = null,
        ushort? Stacks = null,
        short? Hunger = null,
        float? Hunger_Change = null,
        short? Hunger_Resistance = null,
        short? Thirsty = null,
        float? Thirst_Change = null,
        short? Immunity = null,
        float? Immunity_Change = null,
        short? Intoxication = null,
        float? Toxicity_Change = null,
        short? Toxicity_Resistance = null, //Could be ushort ?
        short? Pain = null,
        float? Pain_Change = null,
        short? Pain_Resistance = null, // Could be ushort ?
        short? Pain_Limit = null,
        short? Morale = null,
        float? Morale_Change = null,
        short? Sanity = null,
        float? Sanity_Change = null,
        short? Condition = null,
        short? max_hp = null, // Could be ushort ?
        short? max_hp_res = null, // Could be ushort ?
        short? Health_Restoration = null, // Could be float ?
        short? Healing_Received = null,
        short? max_mp = null, // Could be ushort ?
        short? max_mp_res = null, // Could be ushort ?
        short? MP_Restoration = null,
        short? MP_turn = null, // Could be ushort ?
        short? Fatigue = null,
        float? Fatigue_Change = null,
        short? Fatigue_Gain = null, // Could be ushort ?
        short? Received_XP = null,
        short? Cooldown_Reduction = null,
        short? Weapon_Damage = null,
        short? Hit_Chance = null, // type unknown, assuming short
        short? FMB = null,
        short? CRTD = null, // Could be ushort ?
        short? Fortitude = null, // Could be ushort ?
        short? VSN = null, // type unknown, assuming short
        short? Bleeding_Resistance = null,
        short? Knockback_Resistance = null,
        short? Stun_Resistance = null,
        short? Physical_Resistance = null, // type unknown, assuming short
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
        short? Bleeding_Resistance_2 = null,
        short? Nausea_Chance = null, // Could be ushort ?
        short? Poisoning_Chance = null, // Could be ushort ?
        short? Poisoning_Duration = null,
        bool purse = false,
        bool bottle = false,
        string? upgrade = null, // Could be an enum, keeping as a string for custom upgrades I guess
        short? fodder = null, // Could be ushort ?
        short? stack = null, // Could be ushort ?
        bool fireproof = false,
        bool dropsOnce = false,
        ItemStatsTags tags = ItemStatsTags.none
        )
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_items_stats";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{id};;{Price};{EffPrice};{GetEnumMemberValue(tier)};{GetEnumMemberValue(Cat)};{GetEnumMemberValue(Subcat)};{Material};{GetEnumMemberValue(Weight)};;{Fresh};{Duration};{Stacks};;{Hunger};{Hunger_Change};{Hunger_Resistance};;{Thirsty};{Thirst_Change};;{Immunity};{Immunity_Change};;{Intoxication};{Toxicity_Change};{Toxicity_Resistance};;{Pain};{Pain_Change};{Pain_Resistance};{Pain_Limit};;{Morale};{Morale_Change};{Sanity};{Sanity_Change};;{Condition};{max_hp};{max_hp_res};{Health_Restoration};{Healing_Received};;{max_mp};{max_mp_res};{MP_Restoration};{MP_turn};;{Fatigue};{Fatigue_Change};{Fatigue_Gain};;{Received_XP};{Cooldown_Reduction};{Weapon_Damage};{Hit_Chance};{FMB};{CRTD};{Fortitude};{VSN};;{Bleeding_Resistance};{Knockback_Resistance};{Stun_Resistance};;{Physical_Resistance};{Nature_Resistance};{Magic_Resistance};{Slashing_Resistance};{Piercing_Resistance};{Blunt_Resistance};{Rending_Resistance};{Fire_Resistance};{Shock_Resistance};{Poison_Resistance};{Caustic_Resistance};{Frost_Resistance};{Arcane_Resistance};{Unholy_Resistance};{Sacred_Resistance};{Psionic_Resistance};{Bleeding_Resistance_2};;{Nausea_Chance};{Poisoning_Chance};{Poisoning_Duration};;{(purse ? "1" : "")};{(bottle ? "1" : "")};{upgrade};{fodder};{stack};{(fireproof ? "1" : "")};{(dropsOnce ? "1" : "")};{GetEnumMemberValue(tags)};";
        
        // Add line to table
        table.Add(newline);
        ModLoader.SetTable(table, tableName);
    }
}