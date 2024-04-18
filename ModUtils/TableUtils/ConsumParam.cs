using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Serilog;
using Serilog.Core;

namespace ModShardLauncher;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public partial class Msl
{
    /// <summary>
    /// <para>Enum used in the Consumable Parameters table.</para>
    /// Defines the group to insert the consumable in.
    /// </summary>
    public enum ConsumParamMetaGroup
    {
        TOOLS,
        MEDICINE,
        DRINKS,
        MEAT,
        SEAFOOD,
        FOOD,
        DISHES,
        FRUITS,
        VEGETABLES,
        BERRIES,
        MUSHROOMS,
        HERBS,
        GEMS,
        VALUABLES,
        COMMODITIES,
        [EnumMember(Value = "QUEST ITEMS")]
        QUESTITEMS,
        [EnumMember(Value = "LEGENDARY TREASURES")]
        LEGENDARYTREASURES,
        [EnumMember(Value = "CARAVAN UPGRADES")]
        CARAVANUPGRADES,
        [EnumMember(Value = "JUNK & MATERIALS")]
        JUNKMATERIALS,
        [EnumMember(Value = "ANIMAL LOOT")]
        ANIMALLOOT,
        [EnumMember(Value = "MAPS, SCROLLS & BOOKS")]
        MAPSSCROLLSBOOKS
    }
    /// <summary>
    /// <para>Enum used in Consumable Parameters table. </para>
    /// Defines the category of the consumable.
    /// </summary>
    public enum ConsumParamCategory
    {
        trap,
        tool,
        valuable,
        material,
        other,
        bag,
        medicine,
        junk,
        beverage,
        food,
        ingredient,
        commodity,
        book,
        quest,
        scroll,
        trasure,
        upgrade,
        treatise
    }
    /// <summary>
    /// <para>Enum used in Consumable Parameters table. </para>
    /// Defines the subcategory of the consumable.
    /// </summary>
    public enum ConsumParamSubCategory
    {
        [EnumMember(Value = "")]
        none,
        drug,
        alco,
        alcohol,
        meat,
        Fish,
        fish,
        dairy,
        pastry,
        dish,
        r,
        fruit,
        herb,
        vegetable,
        berry,
        mushroom,
        gem,
        quest
    }
    /// <summary>
    /// <para>Enum used in Consumable Parameters table. </para>
    /// Defines the material the consumable is made of.
    /// </summary>
    public enum ConsumParamMaterial
    {
        metal,
        glass,
        cloth,
        leather,
        wood,
        leather2,
        organic,
        pottery,
        gem,
        stone,
        paper,
    }
    /// <summary>
    /// <para>Enum used in Consumable Parameters table. </para>
    /// Defines the weight class of the consumable.
    /// </summary>
    public enum ConsumParamWeight
    {
        Light,
        Medium,
        Heavy,
        Net,
        VeryLight
    }
    /// <summary>
    /// <para>Enum used in Consumable Parameters table. </para>
    /// Defines the rarity of the consumable.
    /// </summary>
    public enum ConsumParamTags
    {
        [EnumMember(Value = "")]
        none,
        common,
        uncommon,
        special,
        rare,
        [EnumMember(Value = "elven rare")]
        elvenrare,
        [EnumMember(Value = "elven uncommon")]
        elvenuncommon,
        [EnumMember(Value = "elven common")]
        elvencommon
    }
    
    // https://stackoverflow.com/questions/27372816/how-to-read-the-value-for-an-enummember-attribute
    /// <summary>
    /// Adds a line to the Consumable Parameters table with the given parameters.
    /// </summary>
    /// <param name="metaGroup">The group in which to inject the consumable.</param>
    /// <param name="id">The unique id of the consumable.</param>
    /// <param name="Cat">The category the consumable belongs to.</param>
    /// <param name="Material">The material the consumable is made out of.</param>
    /// <param name="Weight">The weight class of the consumable.</param>
    /// <param name="Subcat">The subcategory the consumable belongs to.</param>
    /// <param name="tags">The rarity of the consumable.</param>
    /// <param name="Price">The price of the consumable.</param>
    /// <param name="EffPrice"></param>
    /// <param name="Fresh"></param>
    /// <param name="Duration"></param>
    /// <param name="Hunger"></param>
    /// <param name="Hunger_Change"></param>
    /// <param name="Hunger_Resistance"></param>
    /// <param name="Thirsty"></param>
    /// <param name="Thirst_Change"></param>
    /// <param name="Intoxication"></param>
    /// <param name="Toxicity_Change"></param>
    /// <param name="Toxicity_Resistance"></param>
    /// <param name="Pain"></param>
    /// <param name="Pain_Resistance"></param>
    /// <param name="Pain_Change"></param>
    /// <param name="Pain_Limit"></param>
    /// <param name="Sanity"></param>
    /// <param name="Sanity_Change"></param>
    /// <param name="Morale"></param>
    /// <param name="Morale_Change"></param>
    /// <param name="max_mp_res"></param>
    /// <param name="MP_Restoration"></param>
    /// <param name="max_hp_res"></param>
    /// <param name="Health_Restoration"></param>
    /// <param name="Healing_Received"></param>
    /// <param name="Condition"></param>
    /// <param name="Immunity"></param>
    /// <param name="Fatigue"></param>
    /// <param name="Fatigue_Change"></param>
    /// <param name="Fatigue_Gain"></param>
    /// <param name="Physical_Resistance"></param>
    /// <param name="Nature_Resistance"></param>
    /// <param name="Magic_Resistance"></param>
    /// <param name="Slashing_Resistance"></param>
    /// <param name="Piercing_Resistance"></param>
    /// <param name="Blunt_Resistance"></param>
    /// <param name="Rending_Resistance"></param>
    /// <param name="Fire_Resistance"></param>
    /// <param name="Shock_Resistance"></param>
    /// <param name="Poison_Resistance"></param>
    /// <param name="Caustic_Resistance"></param>
    /// <param name="Frost_Resistance"></param>
    /// <param name="Arcane_Resistance"></param>
    /// <param name="Unholy_Resistance"></param>
    /// <param name="Sacred_Resistance"></param>
    /// <param name="Psionic_Resistance"></param>
    /// <param name="Bleeding_Resistance"></param>
    /// <param name="purse"></param>
    /// <param name="bottle"></param>
    public static void InjectTableConsumableParameters(
        ConsumParamMetaGroup metaGroup,
        string id,
        ConsumParamCategory Cat,
        ConsumParamMaterial Material,
        ConsumParamWeight Weight,
        ConsumParamSubCategory Subcat = ConsumParamSubCategory.none,
        ConsumParamTags tags = ConsumParamTags.none,
        int? Price = null,
        float? EffPrice = null,
        int? Fresh = null,
        int? Duration = null,
        int? Hunger = null,
        float? Hunger_Change = null,
        int? Hunger_Resistance = null,
        int? Thirsty = null,
        float? Thirst_Change = null,
        int? Intoxication = null,
        float? Toxicity_Change = null,
        int? Toxicity_Resistance = null,
        int? Pain = null,
        int? Pain_Resistance = null,
        float? Pain_Change = null,
        string? Pain_Limit = null,
        int? Sanity = null,
        float? Sanity_Change = null,
        int? Morale = null,
        float? Morale_Change = null,
        int? max_mp_res = null,
        int? MP_Restoration = null,
        int? max_hp_res = null,
        int? Health_Restoration = null,
        int? Healing_Received = null,
        int? Condition = null,
        int? Immunity = null,
        int? Fatigue = null,
        float? Fatigue_Change = null,
        int? Fatigue_Gain = null,
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
        int? Bleeding_Resistance = null,
        bool purse = false,
        bool bottle = false)
    {
        // Table filename
        const string tableName = "gml_GlobalScript_table_Consumable_Parameters";
        
        // Load table if it exists
        List<string> table = ThrowIfNull(ModLoader.GetTable(tableName));
        
        // Prepare line
        string newline = $"{id};{Cat};{GetEnumMemberValue(Subcat)};{Price};{EffPrice};{Material};{Weight};{Fresh};{Duration};{Hunger};{Hunger_Change};{Hunger_Resistance};{Thirsty};{Thirst_Change};{Intoxication};{Toxicity_Change};{Toxicity_Resistance};{Pain};{Pain_Resistance};{Pain_Change};{Pain_Limit};{Sanity};{Sanity_Change};{Morale};{Morale_Change};{max_mp_res};{MP_Restoration};{max_hp_res};{Health_Restoration};{Healing_Received};{Condition};{Immunity};{Fatigue};{Fatigue_Change};{Fatigue_Gain};{Physical_Resistance};{Nature_Resistance};{Magic_Resistance};{Slashing_Resistance};{Piercing_Resistance};{Blunt_Resistance};{Rending_Resistance};{Fire_Resistance};{Shock_Resistance};{Poison_Resistance};{Caustic_Resistance};{Frost_Resistance};{Arcane_Resistance};{Unholy_Resistance};{Sacred_Resistance};{Psionic_Resistance};{Bleeding_Resistance};{(purse ? "1" : "")};{(bottle ? "1" : "")};{GetEnumMemberValue(tags)};";
        
        // Find Meta Category in table
        string metaGroupStr = ThrowIfNull(GetEnumMemberValue(metaGroup));
        (int ind, string? foundLine) = table.Enumerate().FirstOrDefault(x => x.Item2.Contains(metaGroupStr));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(ind + 1, newline);
            ModLoader.SetTable(table, tableName);
        }
        else
        {
            Log.Error($"Cannot find Meta Group {metaGroup} in Consumable Parameters table");
            throw new Exception("Meta Group not found in Consumable Parameters table");
        }
    }
}