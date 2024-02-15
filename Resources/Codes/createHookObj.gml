function createHookObj()
{
    var res = array_create(argument[0])
    for(var i = 1; i <= argument[0]; i++)
        res[i - 1] = variable_instance_get(id, argument[i])
    return json_stringify(res, 1)
}