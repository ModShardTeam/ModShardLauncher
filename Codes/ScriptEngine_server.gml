if (server >= 0 && ds_exists(async_load, 1))
{
    var cid = ds_map_find_value(async_load, "id")
    var t = ds_map_find_value(async_load, "type")
    if (t == 3)
    {
        var buffer = ds_map_find_value(async_load, "buffer")
        var length = buffer_read(buffer, buffer_u32)
        var ID = buffer_read(buffer, buffer_u32)
        var message = ""
        for (var i = 0; i < length; i++)
            message += buffer_read(buffer, buffer_string)
        var scr = string_split(message, " ")
        var scriptID = asset_get_index(scr[0])
        array_delete(scr, 0, 1)
        for (i = 0; i < array_length(scr); i++)
        {
            if (string_starts_with(scr[i], "\"") && string_ends_with(scr[i], "\""))
            {
                scr[i] = string_replace_all(scr[i], "_", " ")
                scr[i] = string_delete(scr[i], 1, 1)
                scr[i] = string_delete(scr[i], string_length(scr[i]), 1)
            }
        }
        var ret = ""
        if (scriptID == -1)
            ret = ("script is wrong: " + message)
        else if (array_length(scr) > 0)
            ret = script_execute_ext(scriptID, scr)
        else
            ret = script_execute(scriptID)
        buffer = buffer_create(4, buffer_grow, 1)
        buffer_write(buffer, buffer_u32, ID)
        buffer_write(buffer, buffer_string, "[CLB]")
        buffer_write(buffer, buffer_string, ret)
        network_send_raw(server, buffer, buffer_get_size(buffer))
    }
}
