using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using UndertaleModLib.Models;
using Newtonsoft.Json;
using System.IO;

namespace ModShardLauncher
{
    public class LootElement
    {
        public string[] GuaranteedItems { get; }
        public int RandomLootMin { get; }
        public int RandomLootMax { get; }
        public int EmptyWeight { get; }
        public int[] TableLootWeight { get; }
        public string[] TableLootItems { get; }
        public LootElement(string[] guaranteedItems, int randomLootMin, int randomLootMax, int emptyWeight, string[] tableLoot, int[] weightLoot)
        {
            GuaranteedItems = guaranteedItems;
            RandomLootMin = randomLootMin;
            RandomLootMax = randomLootMax;
            EmptyWeight = emptyWeight;
            TableLootWeight = weightLoot;
            TableLootItems = tableLoot;
        }
    }
    public static class LootUtils
    {
        public static Dictionary<string, LootElement> LootTable = new();
    }
    public static partial class Msl
    {
        public static void AddLoot(string nameObject, string[] guaranteedItems, int randomLootMin, int randomLootMax, int emptyWeight, string[] tableLoot, int[] weightLoot) 
        {
            LootElement lootElement = new(guaranteedItems, randomLootMin, randomLootMax, emptyWeight, tableLoot, weightLoot);
            LootUtils.LootTable.Add(nameObject, lootElement);
        }

        public static void InjectLoot()
        {
            File.WriteAllText("loot_table.json", JsonConvert.SerializeObject(LootUtils.LootTable));
            string lootFunction = @"function scr_resolve_loot_table(argument0)
            {
                file = file_text_open_read(""loot_table.json""); 
                json = file_text_read_string(file);
                var data = json_parse(json);
                var lootStruct = ds_map_find_value(data, argument0);

                if (__is_undefined(lootStruct))
                {
                    scr_actionsLogUpdate(""nothing"");
                    return 0;
                }

                var guaranteedItems = ds_map_find_value(lootStruct, ""GuaranteedItems"");

                if (__is_undefined(guaranteedItems))
                {
                    scr_actionsLogUpdate(""no guaranteedItems"");
                    return 0;
                }

                var sizeGuaranteedItems = ds_list_size(guaranteedItems);
                for (var i = 0; i < sizeGuaranteedItems; i += 1)
                {
                    scr_inventory_add_item(ds_list_find_value(guaranteedItems, i));
                }

                var randomLootMin = ds_map_find_value(lootStruct, ""RandomLootMin"");
                var randomLootMax = ds_map_find_value(lootStruct, ""RandomLootMax"");
                var emptyWeight = ds_map_find_value(lootStruct, ""EmptyWeight"");

                if (__is_undefined(randomLootMin) || __is_undefined(randomLootMax) || __is_undefined(emptyWeight))
                {
                    scr_actionsLogUpdate(""no int"");
                    return 0;
                }
                
                var iteration = randomLootMin + irandom(randomLootMax - randomLootMin);
                var tableLootWeight = ds_map_find_value(lootStruct, ""TableLootWeight"");
                var tableLootItems = ds_map_find_value(lootStruct, ""TableLootItems"");

                if (__is_undefined(tableLootWeight) || __is_undefined(tableLootItems))
                {
                    scr_actionsLogUpdate(""no tableLoot"");
                    return 0;
                }

                var sizeTableLoot = ds_map_size(tableLootWeight);
                var totalWeight = emptyWeight;
                for (var i = 0; i < sizeTableLoot; i += 1)
                {
                    totalWeight += ds_list_find_value(tableLootWeight, i);
                }

                scr_actionsLogUpdate(""totalWeight "" + string(totalWeight));

                for (var j = 0; j < iteration; j += 1)
                {
                    var randomWeight = irandom(totalWeight - 1);
                    scr_actionsLogUpdate(""randomWeight "" + string(randomWeight));
                    var cumulativeWeight = 0;
                    var index = -1;

                    for (var i = 0; i < sizeTableLoot; i += 1)
                    {
                        cumulativeWeight += ds_list_find_value(tableLootWeight, i)
                        if (randomWeight < cumulativeWeight) 
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        scr_actionsLogUpdate(""found "" + string(index));
                        scr_inventory_add_item(ds_list_find_value(tableLootItems, index));
                    }
                    else 
                    {
                        scr_actionsLogUpdate(""found empty"");
                    }
                }
                
            }";

            AddFunction(lootFunction, "scr_resolve_loot_table");

            LoadGML("gml_Object_o_chest_p_Alarm_1")
                .MatchFrom("script_execute")
                .InsertBelow("scr_resolve_loot_table(other.name)")
                .Save();
                
            LoadGML("gml_Object_c_container_Other_10")
                .MatchFrom("script_execute")
                .InsertBelow("scr_resolve_loot_table(other.name)")
                .Save();
        }
    }
}