function scr_msl_resolve_guaranteed_items(argument0, argument1, argument2)
{
    if (!variable_struct_exists(argument0, "ListItems") 
        || !variable_struct_exists(argument0, "ListRarity")
        || !variable_struct_exists(argument0, "ListDurability"))
    {
        scr_msl_log("no ItemsTable data");
        return 0;
    }

    var items = variable_struct_get(argument0, "ListItems");
    var rarity = variable_struct_get(argument0, "ListRarity");
    var durability = variable_struct_get(argument0, "ListDurability");

    var size_array = array_length(items);

    if (size_array != array_length(rarity) ||
        size_array != array_length(durability))
    {
        scr_msl_log("List with incorrect size");
        return 0;
    }

    for(var _i = 0; _i < size_array; _i++)
    {
        scr_msl_resolve_items(items[_i], rarity[_i], durability[_i], argument1, argument2);
    }

    return 1;
}