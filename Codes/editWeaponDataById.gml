function editWeaponDataById(argument0, argument1, argument2, argument3)
{
    var weaIndex
    for(var i = 0; i < instance_number(o_inv_slot); i++)
    {
        var insId = instance_find(o_inv_slot, i)
        if(string_ends_with(string(insId), argument0))
        {
            weaIndex = insId
            break
        }
    }
    var data = variable_instance_get(weaIndex, "data")
    if(argument3 == undefined) argument3 = "string"
    switch argument3
    {
        case "string":
            ds_map_set(data, argument1, string(argument2))
            break
        case "number":
            ds_map_set(data, argument1, real(argument2))
            break
    }
    variable_instance_set(weaIndex, "data", data)
}