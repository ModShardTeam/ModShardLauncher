function scr_msl_resolve_items(argument0, argument1, argument2, argument3, argument4)
{
    if (argument1 == -1)
    {
        var objectName = argument0;
        var obj = "";
        if (argument3 == 0)
        {
            obj = asset_get_index("o_inv_" + objectName)
            if (obj > -1)
            {
                scr_inventory_add_item(obj);
            }
            else
            {
                scr_msl_log("invalid object " + string(objectName));
            }
        }
        else
        {
            obj = asset_get_index("o_loot_" + objectName)
            if (obj > -1)
            {
                scr_loot_drop(argument4.x, argument4.y, obj)
            }
            else
            {
                scr_msl_log("invalid object " + string(objectName));
            }
        }
    }
    else
    {
        if (argument3 == 0)
        {
            with (scr_inventory_add_weapon(argument0, argument1))
            {
                scr_inv_atr_set("Duration", argument2);
            }
        }
        else
        {
            with (scr_weapon_loot(argument0, argument4.x, argument4.y, 100, argument1))
            {
                scr_inv_atr_set("Duration", argument2)
            }
        }
    }
}