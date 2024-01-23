using System;
using System.Linq;
using Serilog;
using UndertaleModLib.Models;

namespace ModShardLauncher
{
    public static class VariableUtils
    {
        public static UndertaleVariable GetVariable(string name)
        {
            try 
            {
                UndertaleVariable variable = ModLoader.Data.Variables.First(t => t.Name?.Content == name);
                Log.Information(string.Format("Found variable: {0}", variable.ToString()));

                return variable;
            }
            catch(Exception ex) {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static UndertaleString GetString(string name)
        {
            try 
            {
                UndertaleString variable = ModLoader.Data.Strings.First(t => t.Content == name);
                Log.Information(string.Format("Found string: {0}", variable.ToString()));

                return variable;
            }
            catch(Exception ex) {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
    }
}