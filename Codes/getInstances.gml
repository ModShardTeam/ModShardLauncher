function getInstances(argument0)
{
    var objIndex = asset_get_index(argument0)
    var res = array_create((instance_number(objIndex) - 1))
    for (var i = 0; i < instance_number(objIndex); i++)
    {
        var insId = instance_find(objIndex, i)
        var name = object_get_name(insId.object_index)
        if (name == "o_inv_slot")
            name = ds_map_find_value(insId.data, "Name")
        res[i] = ""
        for (var j = 1; j <= string_length(name); j++)
            res[i] += (string(string_ord_at(name, j)) + "|")
        res[i] = string_delete(res[i], string_length(res[i]), 1)
        res[i] += (" " + string(insId))
    }
    return json_stringify(res, 1)
}

