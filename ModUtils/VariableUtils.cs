using System;
using System.Linq;
using Serilog;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static partial class Msl
    {
        public static UndertaleVariable GetVariable(string name)
        {
            UndertaleVariable variable = ModLoader.Data.Variables.First(t => t.Name?.Content == name);
            Log.Information("Found variable: {0}", variable);

            return variable;
        }
        public static UndertaleString GetString(string name)
        {
            UndertaleString variable = ModLoader.Data.Strings.First(t => t.Content == name);
            Log.Information("Found string: {0}", variable);

            return variable;
        }
    }
}