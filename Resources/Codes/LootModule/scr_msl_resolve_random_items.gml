function scr_msl_resolve_random_items(argument0, argument1, argument2, argument3, argument4)
{
    if (!variable_struct_exists(argument0, "ItemsTable") 
        || !variable_struct_exists(argument0, "ListWeight"))
    {
        scr_msl_log("no randomLoot data");
        return 0;
    }

    var itemsTable = variable_struct_get(argument0, "ItemsTable");
    var weight = variable_struct_get(argument0, "ListWeight");

    if (!variable_struct_exists(itemsTable, "ListItems")
        || !variable_struct_exists(itemsTable, "ListRarity")
        || !variable_struct_exists(itemsTable, "ListDurability"))
    {
        scr_msl_log("no randomLoot data");
        return 0;
    }

    var items = variable_struct_get(itemsTable, "ListItems");
    var rarity = variable_struct_get(itemsTable, "ListRarity");
    var durability = variable_struct_get(itemsTable, "ListDurability");

    var sizeItems = array_length(items);
    var tableItemsSpecialLootAlready = array_create(sizeItems, 0);

    for (var _j = 0; _j < argument3; _j++)
    {
        var totalWeight = argument4;
        for (var _i = 0; _i < sizeItems; _i++)
        {
            if (ds_list_find_index(scr_atr("specialItemsPool"), items[_i]) != -1)
            {
                tableItemsSpecialLootAlready[_i] = 1;
            }
            else
            {
                totalWeight += weight[_i];
            }
        }
        scr_msl_log("totalWeight " + string(totalWeight));

        var randomWeight = irandom(totalWeight - 1);
        scr_msl_log("randomWeight " + string(randomWeight));
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
            scr_msl_log("found " + string(index));
            scr_msl_resolve_items(items[index], rarity[index], durability[index], argument1, argument2);
        }
        else 
        {
            scr_msl_log("found empty");
        }
    }

    return 1;
}