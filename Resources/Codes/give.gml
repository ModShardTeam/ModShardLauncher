function give(argument0, argument1)
{
    switch argument1
    {
        case "weapon":
            with (o_inventory)
            {
                with (scr_inventory_add_weapon(argument0, (1 << 0)))
                    scr_inv_atr_set("Duration", 100)
            }
            break
    }
}