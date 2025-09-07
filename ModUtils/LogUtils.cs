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
        UndertaleGameObject timer = Msl.AddObject("o_msl_timer", isPersistent: true);
        UndertaleGameObject log = Msl.AddObject("o_msl_log", isPersistent: true);

        Msl.LoadGML(Msl.EventName("o_gameLoader", EventType.Create, 0))
            .MatchAll()
            .InsertBelow("global._msl_log = instance_create_depth(0, 0, -100, o_msl_log);")
            .Save();

        Msl.AddNewEvent(timer, Msl.GetCodeRes("o_msl_timer_Create_0"), EventType.Create, 0);
        Msl.AddNewEvent(timer, Msl.GetCodeRes("o_msl_timer_Step_0"), EventType.Step, 0);
        Msl.AddNewEvent(log, Msl.GetCodeRes("o_msl_log_Create_0"), EventType.Create, 0);

        Msl.AddInnerFunction("scr_msl_log_save");
        Msl.AddInnerFunction("scr_msl_log");
    }
}