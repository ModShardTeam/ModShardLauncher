function getWeaponDataById(argument0)
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
    var data = json_encode(variable_instance_get(weaIndex, "data"))
    var dataCache = ""
    for(var i = 1; i <= string_length(data); i++)
    {
        if(string_ord_at(data, i) > 256 && string_char_at(data, i) != "\n")
        {
            dataCache += string(string_ord_at(data, i)) + "|"
            if(string_ord_at(data, i + 1) <= 256)
                dataCache = string_delete(dataCache, string_length(dataCache), 1)
        }
        else dataCache += string_char_at(data, i)
    }
    return dataCache
}