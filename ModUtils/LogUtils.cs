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
        buffer_write(global._msl_log.buf, buffer_text, ""["" + time + ""]: "" + argument0 + ""\n"");
    }
    else
    {
        scr_actionsLogUpdate(""msl log buff does not exist. Please report that bug with the MSL devs."");
    }
}
else
{
    scr_actionsLogUpdate(""msl log does not exist. Please report that bug with the MSL devs."");
}

if (global._msl_log.timer == noone || !instance_exists(global._msl_log.timer))
{
    var t = instance_create(0, 0, o_msl_timer);
    t.end_time = 10;
    t.func = gml_Script_scr_msl_log_save;

    global._msl_log.timer = t.id;
}

}";

        string mslLogSave = @"function scr_msl_log_save()
{
    buffer_save_async(global._msl_log.buf, global._msl_log.name, 0, 16384);
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
buf = buffer_create(16384, buffer_wrap, 1);

var curr_time = date_current_datetime();
var format_time = string(date_get_year(curr_time)) + string(date_get_month(curr_time)) + string(date_get_day(curr_time)) + ""_"" + string(date_get_hour(curr_time)) + string(date_get_minute(curr_time));
name = ""Logs/msl_log_"" + format_time + "".txt"";

timer = -4
", 
            EventType.Create, 0);

        Msl.AddFunction(mslLogSave, "scr_msl_log_save");
        Msl.AddFunction(mslLog, "scr_msl_log");
        Msl.LoadGML(Msl.EventName("o_gameLoader", EventType.Create, 0))
            .MatchAll()
            .InsertBelow(@"
if (!directory_exists(""Logs""))
{
    directory_create(""Logs"");
}
global._msl_log = instance_create(0, 0, o_msl_log);
")
            .Save();       
    }
}