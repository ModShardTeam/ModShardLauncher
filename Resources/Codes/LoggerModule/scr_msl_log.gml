function scr_msl_log(argument0)
{
    var time = date_datetime_string(date_current_datetime());
    if (global._msl_log != noone)
    {
        if (global._msl_log.buf != noone)
        {
            var string_log = "[" + time + "]: " + argument0 + "\n";
            var len_log = string_byte_length(string_log);

            if (len_log > global._msl_log.size - 1)
            {
                var msg_space = global._msl_log.size - string_byte_length("[" + time + "]: \n") - 1;
                argument0 = string_copy(argument0, 1, msg_space);
                string_log = "[" + time + "]: " + argument0 + "\n";
                len_log = string_byte_length(string_log);
            }

            if (len_log + global._msl_log.cur_size > global._msl_log.size)
            {
                scr_msl_log_save();
                global._msl_log.nfile += 1;
                global._msl_log.cur_size = 0;
            }

            buffer_write(global._msl_log.buf, buffer_text, string_log);
            global._msl_log.cur_size += len_log;
            
            if (global._msl_log.timer == noone || !instance_exists(global._msl_log.timer))
            {
                var t = instance_create_depth(0, 0, -100, o_msl_timer);
                t.end_time = 5;
                t.func = gml_Script_scr_msl_log_save;

                global._msl_log.timer = t.id;
            }
        }
        else
        {
            scr_actionsLogUpdate("msl log buff does not exist. Please report that bug to the MSL devs.");
        }
    }
    else
    {
        scr_actionsLogUpdate("msl log does not exist. Please report that bug to the MSL devs.");
    }
}