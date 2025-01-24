using System;
using System.Collections.Generic;
using Serilog;
using Newtonsoft.Json;
using System.IO;
using UndertaleModLib.Models;

namespace ModShardLauncher;

public static class LogUtils
{
    public static void InjectLog()
    {
        string mslLog = @"function scr_msl_log(argument0)
{
var time = date_datetime_string(date_current_datetime());
if (global._msl_log != noone)
{
    if (global._msl_log.buf != noone)
    {
        var string_log = ""["" + time + ""]: "" + argument0 + ""\n"";
        var len_log = string_byte_length(string_log);

        if (len_log > global._msl_log.size)
        {
            string_log = string_copy(string_log, 1, global._msl_log.size - 1) + ""\n"";
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
        scr_actionsLogUpdate(""msl log buff does not exist. Please report that bug to the MSL devs."");
    }
}
else
{
    scr_actionsLogUpdate(""msl log does not exist. Please report that bug to the MSL devs."");
}
}";

        string mslLogSave = @"function scr_msl_log_save()
{
    var nfile_name = global._msl_log.name + ""_"" + string(global._msl_log.nfile) + "".txt"";
    buffer_save_async(global._msl_log.buf, nfile_name, 0, global._msl_log.cur_size);
    instance_destroy(global._msl_log.timer);
}";

        UndertaleGameObject timer = Msl.AddObject("o_msl_timer", isPersistent: true);
        Msl.AddNewEvent(timer, @"
func = -4;
end_time = 0;
cumulative_time = 0;", 
            EventType.Create, 0);

        Msl.AddNewEvent(timer, @"
cumulative_time += delta_time / 1000000;
if (cumulative_time > end_time)
{
    if (func != noone)
    {
        script_execute(func)
    }
    instance_destroy();
}", 
            EventType.Step, 0);

        UndertaleGameObject log = Msl.AddObject("o_msl_log", isPersistent: true);
        Msl.AddNewEvent(log, @"
size = 1000000
buf = buffer_create(size, buffer_wrap, 1);
cur_size = 0
nfile = 0

var curr_time = date_current_datetime();
var format_time = string_format(date_get_year(curr_time), 2, 0) + string_format(date_get_month(curr_time), 2, 0) + string_format(date_get_day(curr_time), 2, 0) + ""_"" + string_format(date_get_hour(curr_time), 2, 0) + string_format(date_get_minute(curr_time), 2, 0);
name = ""Logs/msl_log_"" + string_replace_all(format_time, "" "", ""0"");

timer = -4
", 
            EventType.Create, 0);

        Msl.AddFunction(mslLogSave, "scr_msl_log_save");
        Msl.AddFunction(mslLog, "scr_msl_log");
        Msl.LoadGML(Msl.EventName("o_gameLoader", EventType.Create, 0))
            .MatchAll()
            .InsertBelow(@"global._msl_log = instance_create_depth(0, 0, -100, o_msl_log);")
            .Save();       
    }
}