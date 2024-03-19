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

                if (!variable_struct_exists(lootStruct, ""TableLootWeight"") || !variable_struct_exists(lootStruct, ""TableLootItems""))
                {
                    scr_actionsLogUpdate(""no tableLoot"");
                    return 0;
                }

                var tableLootWeight = variable_struct_get(lootStruct, ""TableLootWeight"");
                var tableLootItems = variable_struct_get(lootStruct, ""TableLootItems"");

                var sizeTableLoot = array_length(tableLootWeight);
                var totalWeight = emptyWeight;
                for (var i = 0; i < sizeTableLoot; i += 1)
                {
                    totalWeight += tableLootWeight[i];
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
                        scr_actionsLogUpdate(""found empty"");
                    }
                }
            }";

            AddFunction(lootFunction, "scr_resolve_loot_table");

            LoadGML("gml_Object_o_chest_p_Alarm_1")
                .MatchFrom("script_execute")
                .InsertBelow("scr_resolve_loot_table(object_get_name(other.object_index))")
                .Save();
                
            LoadGML("gml_Object_c_container_Other_10")
                .MatchFrom("script_execute")
                .InsertBelow("scr_resolve_loot_table(object_get_name(other.object_index))")
                .Save();
        }
    }
}