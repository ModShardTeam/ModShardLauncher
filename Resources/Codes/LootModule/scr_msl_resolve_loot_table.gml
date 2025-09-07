function scr_msl_resolve_loot_table(argument0, argument1)
{
    var objectName = object_get_name(argument0.object_index);
    scr_msl_log("instance: " + string(argument0.id) + " of " + objectName);

    var referenceLootTable = scr_msl_resolve_refence_table(argument0);
    if (referenceLootTable == noone)
    {
        scr_msl_log("Reference Table resolution failed");
        return 0;
    }

    var file = file_text_open_read("loot_table.json"); 
    var json = file_text_read_string(file);
    var data = json_parse(json);

    if (!variable_struct_exists(data, referenceLootTable))
    {
        scr_msl_log("cant find ref " + referenceLootTable);
        file_text_close(file);
        return 0;
    }
    var lootStruct = variable_struct_get(data, referenceLootTable);

    if (!variable_struct_exists(lootStruct, "GuaranteedItems"))
    {
        scr_msl_log("no guaranteedItems");
        file_text_close("loot_table.json");
        return 0;
    }
    var guaranteedItems = variable_struct_get(lootStruct, "GuaranteedItems");

    if (!scr_msl_resolve_guaranteed_items(guaranteedItems, argument1, argument0))
    {
        scr_msl_log("Guaranteed Items resolution failed");
        file_text_close("loot_table.json");
        return 0;
    }

    if (!variable_struct_exists(lootStruct, "RandomLootMin") || !variable_struct_exists(lootStruct, "RandomLootMax") || !variable_struct_exists(lootStruct, "EmptyWeight"))
    {
        scr_msl_log("no int");
        file_text_close("loot_table.json");
        return 0;
    }

    var randomLootMin = variable_struct_get(lootStruct, "RandomLootMin");
    var randomLootMax = variable_struct_get(lootStruct, "RandomLootMax");
    var emptyWeight = variable_struct_get(lootStruct, "EmptyWeight");

    var iteration = randomLootMin + irandom(randomLootMax - randomLootMin);
    scr_msl_log("iteration " + string(iteration));

    if (!variable_struct_exists(lootStruct, "RandomItemsTable"))
    {
        scr_msl_log("no RandomItemsTable");
        file_text_close("loot_table.json");
        return 0;
    }

    var randomItemsTable = variable_struct_get(lootStruct, "RandomItemsTable");

    scr_msl_resolve_random_items(randomItemsTable, argument1, argument0, iteration, emptyWeight);

    file_text_close(file);

    return 1;
}