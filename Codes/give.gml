function give(argument0)
{
    with (o_inventory)
    {
        with (scr_inventory_add_weapon(argument0, (1 << 0)))
            scr_inv_atr_set("Duration", 100)
    }
}