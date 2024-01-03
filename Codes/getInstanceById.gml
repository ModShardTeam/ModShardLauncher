function getInstanceById(argument0)
{
    var objIndex
    for (var i = 0; i < instance_count; i++)
    {
        var insId = instance_find(all, i)
        if(string_ends_with(string(insId), argument0)) 
        {
            objIndex = insId
            break
        }
    }
    var count = variable_instance_names_count(objIndex)
    var names = variable_instance_get_names(objIndex)
    var res = array_create(count)
    for(var i = 0; i < count; i++)
    {
        var value = string(variable_instance_get(objIndex, names[i]))
        if(string_ord_at(value, 1) > 256)
        {
            value = ""
            var vc = string(variable_instance_get(objIndex, names[i]))
            for (var j = 1; j <= string_length(vc); j++)
                value += (string(string_ord_at(vc, j)) + "|")
            value = string_delete(value, string_length(value), 1)
        }
        res[i] = names[i] + " : " + value
    }
    return json_stringify(res, 1)
}