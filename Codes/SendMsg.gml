function SendMsg(argument0, argument1, argument2)
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
    network_send_raw(global.Server, buffer, buffer_get_size(buffer))
}