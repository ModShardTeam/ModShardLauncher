using System;
using System.Collections.Generic;
using Serilog;
using Newtonsoft.Json;
using System.IO;

namespace ModShardLauncher
{
    public class ItemsTable
    {
        public string[] ListItems { get; }
        public int[] ListRarity { get; }
        public int[] ListDurability { get; }
        public ItemsTable()
        {
            ListItems = Array.Empty<string>();
            ListRarity = Array.Empty<int>();
            ListDurability = Array.Empty<int>();
        }
        public ItemsTable(string[] listItems, int[] listRarity, int[] listDurability)
        {
            if (listItems.Length != listRarity.Length || listItems.Length != listDurability.Length)
            {
                throw new ArgumentException($"Error in ItemsTable constructor, {listItems}, {listRarity} and {listDurability} must have the same length.");
            } 
            ListItems = listItems;
            ListRarity = listRarity;
            ListDurability = listDurability;
        }
    }
    public class RandomItemsTable
    {
        public int[] ListWeight { get; }
        public ItemsTable ItemsTable { get; }
        public RandomItemsTable(int[] listWeight, ItemsTable itemsTable)
        {
            if (listWeight.Length != itemsTable.ListItems.Length)
            {
                throw new ArgumentException($"Error in RandomItemsTable constructor, {listWeight}, and {itemsTable} elements must have the same length.");
            } 
            ListWeight = listWeight;
            ItemsTable = itemsTable;
        }
        public RandomItemsTable(int[] listWeight, string[] listItems, int[] listRarity, int[] listDurability): this(listWeight, new ItemsTable(listItems, listRarity, listDurability)) { }
    }
    public class ReferenceTable
    {
        public string DefaultTable { get; }
        public Dictionary<int, string> Ids { get; }
        public Dictionary<int, string> Tiers { get; }
        public ReferenceTable(string defaultTable, Dictionary<int, string> ids, Dictionary<int, string> tiers)
        {
            DefaultTable = defaultTable;
            Ids = ids;
            Tiers = tiers;
        }
    }
    public class LootTable
    {
        public ItemsTable GuaranteedItems { get; }
        public int RandomLootMin { get; }
        public int RandomLootMax { get; }
        public int EmptyWeight { get; }
        public RandomItemsTable RandomItemsTable { get; }
        public LootTable(ItemsTable guaranteedItems, int randomLootMin, int randomLootMax, int emptyWeight, RandomItemsTable randomItemsTable)
        {
            GuaranteedItems = guaranteedItems;
            RandomLootMin = randomLootMin;
            RandomLootMax = randomLootMax;
            EmptyWeight = emptyWeight;
            RandomItemsTable = randomItemsTable;
        }
    }
    public static class LootUtils
    {
        internal static Dictionary<string, ReferenceTable> ReferenceTables = new();
        internal static Dictionary<string, LootTable> LootTables = new();
        public static void ResetLootTables()
        {
            ReferenceTables.Clear();
            LootTables.Clear();
        }
        public static void SaveLootTables(string DirPath)
        {
            if (LootTables.Count > 0)
            {
                File.WriteAllText(Path.Combine(DirPath, "loot_table.json"), JsonConvert.SerializeObject(LootTables));
                Log.Information("Successfully saving the loot table json.");
            }
            if (ReferenceTables.Count > 0)
            {
                File.WriteAllText(Path.Combine(DirPath, "reference_table.json"), JsonConvert.SerializeObject(ReferenceTables));
                Log.Information("Successfully saving the reference table json.");
            }
        }
        public static void InjectLootScripts()
        {
            if (LootTables.Count == 0 && ReferenceTables.Count == 0)  return;

            string mslItemsFunction = @"function scr_msl_resolve_items(argument0, argument1, argument2, argument3, argument4)
{
    if (argument1 == -1)
    {
        var objectName = argument0;
        var obj = """";
        if (argument3 == 0)
        {
            obj = asset_get_index(""o_inv_"" + objectName)
            if (obj > -1)
            {
                scr_inventory_add_item(obj);
            }
            else
            {
                scr_msl_log(""invalid object "" + string(objectName));
            }
        }
        else
        {
            obj = asset_get_index(""o_loot_"" + objectName)
            if (obj > -1)
            {
                scr_loot_drop(argument4.x, argument4.y, obj)
            }
            else
            {
                scr_msl_log(""invalid object "" + string(objectName));
            }
        }
    }
    else
    {
        if (argument3 == 0)
        {
            with (scr_inventory_add_weapon(argument0, argument1))
            {
                scr_inv_atr_set(""Duration"", argument2);
            }
        }
        else
        {
            with (scr_weapon_loot(argument0, argument4.x, argument4.y, 100, argument1))
            {
                scr_inv_atr_set(""Duration"", argument2)
            }
        }
    }
}";

            string mslRefFunction = @"function scr_msl_resolve_refence_table(argument0)
{
    var objectName = object_get_name(argument0.object_index);
    var refFile = file_text_open_read(""reference_table.json""); 
    var refJson = file_text_read_string(refFile);
    var refData = json_parse(refJson);

    var min_lvl = scr_globaltile_dungeon_get(""mob_lvl_min"");
    var max_lvl = scr_globaltile_dungeon_get(""mob_lvl_max"");
    var tier = floor(((max_lvl + min_lvl) / 2));
    scr_msl_log(""current tier: "" + string(tier));

    if (!variable_struct_exists(refData, objectName))
    {
        scr_msl_log(""cant find object "" + objectName);
        file_text_close(refFile);
        return -4;
    }
    var refStruct = variable_struct_get(refData, objectName);
    var referenceLootTableIndex = -1;

    if (!variable_struct_exists(refStruct, ""DefaultTable""))
    {
        scr_msl_log(""cant find DefaultTable"");
        file_text_close(refFile);
        return -4;
    }
    var defaultTable = variable_struct_get(refStruct, ""DefaultTable"");
    
    if (!variable_struct_exists(refStruct, ""Ids""))
    {
        scr_msl_log(""cant find Ids"");
        file_text_close(refFile);
        return -4;
    }
    var idsStruct = variable_struct_get(refStruct, ""Ids"");

    var _ids = variable_struct_get_names(idsStruct);
    for (var i = 0; i < array_length(_ids); i++;)
    {
        if (real(_ids[i]) == argument0.id)
        {
            var referenceLootTable = variable_struct_get(idsStruct, _ids[i]);
            scr_msl_log(""ref with id: "" + referenceLootTable);
            file_text_close(refFile);
            return referenceLootTable;
        }
    }

    if (!variable_struct_exists(refStruct, ""Tiers""))
    {
        scr_msl_log(""cant find Tiers"");
        file_text_close(refFile);
        return -4;
    }
    var tiersStruct = variable_struct_get(refStruct, ""Tiers"");
    var tiers = variable_struct_get_names(tiersStruct);
    var indexTier = -1;

    for (var i = 0; i < array_length(tiers); i++;)
    {
        if (tier < real(tiers[i]))
        {
            indexTier = i - 1;
            break;
        }
        else
        {
            indexTier = i;
        }
    }

    if (indexTier == -1)
    {
        var referenceLootTable = defaultTable;
        scr_msl_log(""ref with default: "" + referenceLootTable);
    }
    else
    {
        var referenceLootTable = variable_struct_get(tiersStruct, tiers[indexTier]);
        scr_msl_log(""ref with tier: "" + referenceLootTable);
    }
    
    file_text_close(refFile);
    return referenceLootTable;
}";

            string mslLootGuaranteedItemsFunction = @"function scr_msl_resolve_guaranteed_items(argument0, argument1, argument2)
{
    if (!variable_struct_exists(argument0, ""ListItems"") 
        || !variable_struct_exists(argument0, ""ListRarity"")
        || !variable_struct_exists(argument0, ""ListDurability""))
    {
        scr_msl_log(""no ItemsTable data"");
        return 0;
    }

    var items = variable_struct_get(argument0, ""ListItems"");
    var rarity = variable_struct_get(argument0, ""ListRarity"");
    var durability = variable_struct_get(argument0, ""ListDurability"");

    var size_array = array_length(items);

    if (size_array != array_length(rarity) ||
        size_array != array_length(durability))
    {
        scr_msl_log(""List with incorrect size"");
        return 0;
    }

    for(var _i = 0; _i < size_array; _i++)
    {
        scr_msl_resolve_items(items[_i], rarity[_i], durability[_i], argument1, argument2);
    }

    return 1;
}
";
            string mslLootRandomItemsFunction = @"function scr_msl_resolve_random_items(argument0, argument1, argument2, argument3, argument4)
{
    if (!variable_struct_exists(argument0, ""ItemsTable"") 
        || !variable_struct_exists(argument0, ""ListWeight""))
    {
        scr_msl_log(""no randomLoot data"");
        return 0;
    }

    var itemsTable = variable_struct_get(argument0, ""ItemsTable"");
    var weight = variable_struct_get(argument0, ""ListWeight"");

    if (!variable_struct_exists(itemsTable, ""ListItems"")
        || !variable_struct_exists(itemsTable, ""ListRarity"")
        || !variable_struct_exists(itemsTable, ""ListDurability""))
    {
        scr_msl_log(""no randomLoot data"");
        return 0;
    }

    var items = variable_struct_get(itemsTable, ""ListItems"");
    var rarity = variable_struct_get(itemsTable, ""ListRarity"");
    var durability = variable_struct_get(itemsTable, ""ListDurability"");

    var sizeItems = array_length(items);
    var tableItemsSpecialLootAlready = array_create(sizeItems, 0);

    for (var _j = 0; _j < argument3; _j++)
    {
        var totalWeight = argument4;
        for (var _i = 0; _i < sizeItems; _i++)
        {
            if (ds_list_find_index(scr_atr(""specialItemsPool""), items[_i]) != -1)
            {
                tableItemsSpecialLootAlready[_i] = 1;
            }
            else
            {
                totalWeight += weight[_i];
            }
        }
        scr_msl_log(""totalWeight "" + string(totalWeight));

        var randomWeight = irandom(totalWeight - 1);
        scr_msl_log(""randomWeight "" + string(randomWeight));
        var cumulativeWeight = 0;
        var index = -1;

        for (var _i = 0; _i < sizeItems; _i++)
        {
            if (tableItemsSpecialLootAlready[_i] == 1)
            {
                continue;
            }
            cumulativeWeight += weight[_i]
            if (randomWeight < cumulativeWeight) 
            {
                index = _i;
                break;
            }
        }

        if (index != -1)
        {
            scr_msl_log(""found "" + string(index));
            scr_msl_resolve_items(items[index], rarity[index], durability[index], argument1, argument2);
        }
        else 
        {
            scr_msl_log(""found empty"");
        }
    }

    return 1;
}";

            string mslLootFunction = @"function scr_msl_resolve_loot_table(argument0, argument1)
{
    var objectName = object_get_name(argument0.object_index);
    scr_msl_log(""instance: "" + string(argument0.id) + "" of "" + objectName);

    var referenceLootTable = scr_msl_resolve_refence_table(argument0);
    if (referenceLootTable == noone)
    {
        scr_msl_log(""Reference Table resolution failed"");
        return 0;
    }

    var file = file_text_open_read(""loot_table.json""); 
    var json = file_text_read_string(file);
    var data = json_parse(json);

    if (!variable_struct_exists(data, referenceLootTable))
    {
        scr_msl_log(""cant find ref "" + referenceLootTable);
        file_text_close(file);
        return 0;
    }
    var lootStruct = variable_struct_get(data, referenceLootTable);

    if (!variable_struct_exists(lootStruct, ""GuaranteedItems""))
    {
        scr_msl_log(""no guaranteedItems"");
        file_text_close(""loot_table.json"");
        return 0;
    }
    var guaranteedItems = variable_struct_get(lootStruct, ""GuaranteedItems"");

    if (!scr_msl_resolve_guaranteed_items(guaranteedItems, argument1, argument0))
    {
        scr_msl_log(""Guaranteed Items resolution failed"");
        file_text_close(""loot_table.json"");
        return 0;
    }

    if (!variable_struct_exists(lootStruct, ""RandomLootMin"") || !variable_struct_exists(lootStruct, ""RandomLootMax"") || !variable_struct_exists(lootStruct, ""EmptyWeight""))
    {
        scr_msl_log(""no int"");
        file_text_close(""loot_table.json"");
        return 0;
    }

    var randomLootMin = variable_struct_get(lootStruct, ""RandomLootMin"");
    var randomLootMax = variable_struct_get(lootStruct, ""RandomLootMax"");
    var emptyWeight = variable_struct_get(lootStruct, ""EmptyWeight"");

    var iteration = randomLootMin + irandom(randomLootMax - randomLootMin);
    scr_msl_log(""iteration "" + string(iteration));

    if (!variable_struct_exists(lootStruct, ""RandomItemsTable""))
    {
        scr_msl_log(""no RandomItemsTable"");
        file_text_close(""loot_table.json"");
        return 0;
    }

    var randomItemsTable = variable_struct_get(lootStruct, ""RandomItemsTable"");

    scr_msl_resolve_random_items(randomItemsTable, argument1, argument0, iteration, emptyWeight);

    file_text_close(file);

    return 1;
}";

            Msl.AddFunction(mslItemsFunction, "scr_msl_resolve_items");
            Msl.AddFunction(mslRefFunction, "scr_msl_resolve_refence_table");
            Msl.AddFunction(mslLootGuaranteedItemsFunction, "scr_msl_resolve_guaranteed_items");
            Msl.AddFunction(mslLootRandomItemsFunction, "scr_msl_resolve_random_items");
            Msl.AddFunction(mslLootFunction, "scr_msl_resolve_loot_table");

            Msl.LoadGML("gml_Object_o_chest_p_Alarm_1")
                .MatchFrom("script_execute")
                .InsertBelow("scr_msl_resolve_loot_table(other, 0)")
                .Save();
                
            Msl.LoadGML("gml_Object_c_container_Other_10")
                .MatchFrom("script_execute")
                .InsertBelow("scr_msl_resolve_loot_table(other, 0)")
                .Save();
                
            Msl.LoadGML("gml_Object_o_unit_Destroy_0")
                .MatchAll()
                .InsertBelow("scr_msl_resolve_loot_table(self, 1)")
                .Save();
        }
    }
    public static partial class Msl
    {
        public static void AddLootTable(string lootTableID, ItemsTable guaranteedItems, int randomLootMin, int randomLootMax, int emptyWeight, RandomItemsTable randomItemsTable)
        {
            LootTable lootTable = new(guaranteedItems, randomLootMin, randomLootMax, emptyWeight, randomItemsTable);
            LootUtils.LootTables.Add(lootTableID, lootTable);
        }
        public static void AddReferenceTable(string nameObject, string table)
        {
            LootUtils.ReferenceTables.Add(nameObject, new ReferenceTable(table, new Dictionary<int, string>(), new Dictionary<int, string>()));
        }
        public static void AddReferenceTable(string nameObject, string table, Dictionary<int, string>? ids, Dictionary<int, string>? tiers)
        {
            LootUtils.ReferenceTables.Add(nameObject, new ReferenceTable(table, ids ?? new Dictionary<int, string>(), tiers ?? new Dictionary<int, string>()));
        }
        public static void AddReferenceTableForMultipleObjects(string table, params string[] nameObjects)
        {
            foreach(string nameObject in nameObjects)
            {
                LootUtils.ReferenceTables.Add(nameObject, new ReferenceTable(table, new Dictionary<int, string>(), new Dictionary<int, string>()));
            }
        }
    }
}