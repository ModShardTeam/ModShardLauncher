using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using UndertaleModLib.Models;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Win32;

namespace ModShardLauncher
{
    public class RandomLootTable
    {
        public int[] TableLootWeight { get; }
        public string[] TableLootItems { get; }
        public int[] TableLootRarity { get; }
        public int[] TableLootDurability { get; }

        public RandomLootTable(int[] tableLootWeight, string[] tableLootItems, int[] tableLootRarity, int[] tableLootDurability)
        {
            TableLootWeight = tableLootWeight;
            TableLootItems = tableLootItems;
            TableLootRarity = tableLootRarity;
            TableLootDurability = tableLootDurability;
        }
    }
    public class LootTable
    {
        public string[] GuaranteedItems { get; }
        public int RandomLootMin { get; }
        public int RandomLootMax { get; }
        public int EmptyWeight { get; }
        public RandomLootTable RandomLootTable { get; }
        public LootTable(string[] guaranteedItems, int randomLootMin, int randomLootMax, int emptyWeight, RandomLootTable randomLootTable)
        {
            GuaranteedItems = guaranteedItems;
            RandomLootMin = randomLootMin;
            RandomLootMax = randomLootMax;
            EmptyWeight = emptyWeight;
            RandomLootTable = randomLootTable;
        }
    }
    public static class LootUtils
    {
        public static Dictionary<string, LootTable> LootTables = new();
        public static void ResetLootTables()
        {
            LootTables.Clear();
        }
        public static void SaveLootTables(string DirPath)
        {
            if (LootTables.Count == 0)  return;
          
            File.WriteAllText(Path.Combine(DirPath, "loot_table.json"), JsonConvert.SerializeObject(LootTables));
            Log.Information("Successfully saving the loot table json.");
        }
        public static void InjectLootScripts()
        {
            if (LootTables.Count == 0)  return;

            string lootFunction = @"function scr_resolve_loot_table(argument0)
            {
                file = file_text_open_read(""loot_table.json""); 
                json = file_text_read_string(file);
                var data = json_parse(json);

                if (!variable_struct_exists(data, argument0))
                {
                    scr_actionsLogUpdate(""cant find: "" + string(argument0));
                    return 0;
                }
                var lootStruct = variable_struct_get(data, argument0);

                var objectName = """";
                var obj = -1;

                if (!variable_struct_exists(lootStruct, ""GuaranteedItems""))
                {
                    scr_actionsLogUpdate(""no guaranteedItems"");
                    return 0;
                }
                var guaranteedItems = variable_struct_get(lootStruct, ""GuaranteedItems"");
                var sizeGuaranteedItems = array_length(guaranteedItems);

                for (var i = 0; i < sizeGuaranteedItems; i += 1)
                {
                    objectName = guaranteedItems[i];
                    obj = asset_get_index(objectName)
                    if (obj > -1)
                    {
                        scr_inventory_add_item(obj);
                    }
                    else
                    {
                        scr_actionsLogUpdate(""invalid object "" + string(objectName));
                    }
                }
    
                if (!variable_struct_exists(lootStruct, ""RandomLootMin"") || !variable_struct_exists(lootStruct, ""RandomLootMax"") || !variable_struct_exists(lootStruct, ""EmptyWeight""))
                {
                    scr_actionsLogUpdate(""no int"");
                    return 0;
                }

                var randomLootMin = variable_struct_get(lootStruct, ""RandomLootMin"");
                var randomLootMax = variable_struct_get(lootStruct, ""RandomLootMax"");
                var emptyWeight = variable_struct_get(lootStruct, ""EmptyWeight"");

                var iteration = randomLootMin + irandom(randomLootMax - randomLootMin);
                scr_actionsLogUpdate(""iteration "" + string(iteration));

                if (!variable_struct_exists(lootStruct, ""RandomLootTable""))
                {
                    scr_actionsLogUpdate(""no RandomLootTable"");
                    return 0;
                }

                var randomLootTable = variable_struct_get(lootStruct, ""RandomLootTable"");
                
                if (!variable_struct_exists(randomLootTable, ""TableLootWeight"") 
                    || !variable_struct_exists(randomLootTable, ""TableLootItems"")
                    || !variable_struct_exists(randomLootTable, ""TableLootRarity"")
                    || !variable_struct_exists(randomLootTable, ""TableLootDurability""))
                {
                    scr_actionsLogUpdate(""no tableLoot data"");
                    return 0;
                }

                var tableLootWeight = variable_struct_get(randomLootTable, ""TableLootWeight"");
                var tableLootItems = variable_struct_get(randomLootTable, ""TableLootItems"");
                var tableLootRarity = variable_struct_get(randomLootTable, ""TableLootRarity"");
                var tableLootDurability = variable_struct_get(randomLootTable, ""TableLootDurability"");

                var sizeTableLoot = array_length(tableLootWeight);
                var tableLootSpecialLootAlready = array_create(sizeTableLoot, 0);

                for (var j = 0; j < iteration; j += 1)
                {
                    var totalWeight = emptyWeight;
                    for (var i = 0; i < sizeTableLoot; i += 1)
                    {
                        if (ds_list_find_index(scr_atr(""specialItemsPool""), tableLootItems[i]) != -1)
                        {
                            tableLootSpecialLootAlready[i] = 1;
                        }
                        else
                        {
                            totalWeight += tableLootWeight[i];
                        }
                    }

                    scr_actionsLogUpdate(""totalWeight "" + string(totalWeight));

                    var randomWeight = irandom(totalWeight - 1);
                    scr_actionsLogUpdate(""randomWeight "" + string(randomWeight));
                    var cumulativeWeight = 0;
                    var index = -1;

                    for (var i = 0; i < sizeTableLoot; i += 1)
                    {
                        if (tableLootSpecialLootAlready[i] == 1)
                        {
                            continue;
                        }
                        cumulativeWeight += tableLootWeight[i]
                        if (randomWeight < cumulativeWeight) 
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        scr_actionsLogUpdate(""found "" + string(index));

                        if (tableLootRarity[index] == -1)
                        {
                            objectName = tableLootItems[index];
                            obj = asset_get_index(objectName)
                            if (obj > -1)
                            {
                                scr_inventory_add_item(obj);
                            }
                            else
                            {
                                scr_actionsLogUpdate(""invalid object "" + string(objectName));
                            }
                        }
                        else
                        {
                            with (scr_inventory_add_weapon(tableLootItems[index], (tableLootRarity[index] << 0)))
                            {
                                scr_inv_atr_set(""Duration"", tableLootDurability[index]);
                            }
                        }
                        
                    }
                    else 
                    {
                        scr_actionsLogUpdate(""found empty"");
                    }
                }
            }";

            Msl.AddFunction(lootFunction, "scr_resolve_loot_table");

            Msl.LoadGML("gml_Object_o_chest_p_Alarm_1")
                .MatchFrom("script_execute")
                .InsertBelow("scr_resolve_loot_table(object_get_namZe(other.object_index))")
                .Save();
                
            Msl.LoadGML("gml_Object_c_container_Other_10")
                .MatchFrom("script_execute")
                .InsertBelow("scr_resolve_loot_table(object_get_name(other.object_index))")
                .Save();
        }
    }
    public static partial class Msl
    {
        public static void AddLootTable(string lootTableID, string[] guaranteedItems, int randomLootMin, int randomLootMax, int emptyWeight, RandomLootTable randomLootTable)
        {
            LootTable lootTable = new(guaranteedItems, randomLootMin, randomLootMax, emptyWeight, randomLootTable);
            LootUtils.LootTables.Add(lootTableID, lootTable);
        }
    }
}