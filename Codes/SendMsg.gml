function SendMsg(argument0, argument1, argument2, argument3)
{
    var buffer = buffer_create(4, buffer_grow, 1)
    switch (argument0)
    {
        case "MSG":
            buffer_write(buffer, buffer_u32, -1)
        break
        case "CLB":
            buffer_write(buffer, buffer_u32, argument2)
        break
        case "HOK":
            buffer_write(buffer, buffer_u32, -1)
        break
    }

    buffer_write(buffer, buffer_string, (("[" + argument0) + "]"))
    buffer_write(buffer, buffer_string, argument1)
    if(argument != undefined && argument0 == "HOK")
    {
        buffer_write(buffer, buffer_string, "<EXTRAMSG>")
        var extra = ""
        for (var i = 0; i < array_length(argument3); i++)
            extra += string(argument3[i]) + "|"
        buffer_write(buffer, buffer_string, extra)
    }
    network_send_raw(global.Server, buffer, buffer_get_size(buffer))
}