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
    public enum ConsumParamToolMetaCategory
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
    public enum ConsumParamWeight
    {
        Light,
        Medium,
        Heavy,
        Net,
        VeryLight
    }
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
    private static string? GetEnumMemberValue<T>(this T value)
        where T : Enum
    {
        return typeof(T)
            .GetTypeInfo()
            .DeclaredMembers
            .SingleOrDefault(x => x.Name == value.ToString())?
            .GetCustomAttribute<EnumMemberAttribute>(false)?
            .Value;
    }
    
    public static void InjectTableConsumableParameters(
        ConsumParamToolMetaCategory metaCategory, 
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
        // Load table if it exists
        List<string> table = Msl.ThrowIfNull(ModLoader.GetTable("gml_GlobalScript_table_Consumable_Parameters"));
        
        // Prepare line
        string newline = $"{id};{Cat};{GetEnumMemberValue(Subcat)};{Price};{EffPrice};{Material};{Weight};{Fresh};{Duration};{Hunger};{Hunger_Change};{Hunger_Resistance};{Thirsty};{Thirst_Change};{Intoxication};{Toxicity_Change};{Toxicity_Resistance};{Pain};{Pain_Resistance};{Pain_Change};{Pain_Limit};{Sanity};{Sanity_Change};{Morale};{Morale_Change};{max_mp_res};{MP_Restoration};{max_hp_res};{Health_Restoration};{Healing_Received};{Condition};{Immunity};{Fatigue};{Fatigue_Change};{Fatigue_Gain};{Physical_Resistance};{Nature_Resistance};{Magic_Resistance};{Slashing_Resistance};{Piercing_Resistance};{Blunt_Resistance};{Rending_Resistance};{Fire_Resistance};{Shock_Resistance};{Poison_Resistance};{Caustic_Resistance};{Frost_Resistance};{Arcane_Resistance};{Unholy_Resistance};{Sacred_Resistance};{Psionic_Resistance};{Bleeding_Resistance};{(purse ? "1" : "")};{(bottle ? "1" : "")};{GetEnumMemberValue(tags)}";
        
        // Find Meta Category in table
        string? foundLine = table.FirstOrDefault(line => line.Contains(GetEnumMemberValue(metaCategory)));
        
        // Add line to table
        if (foundLine != null)
        {
            table.Insert(table.IndexOf(foundLine) + 1, newline);
            ModLoader.SetTable(table, "gml_GlobalScript_table_Consumable_Parameters");
        }
        else
        {
            Log.Error($"Cannot find Meta Category {metaCategory} in Consumable Parameters table");
            throw new Exception("MetaCategory not found in Consumable Parameters table");
        }
    }
}