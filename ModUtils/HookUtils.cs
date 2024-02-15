using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModShardLauncher
{
    public static partial class Msl
    {
        /// <summary>
        /// Add a hook with the name <paramref name="hookName"/> to <paramref name="functionName"/>.
        /// Havent done yet.
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="position"></param>
        /// <param name="paramNames">All the things you want to get.</param>
        public static void HookFunction(string functionName, string hookName, params string[] paramNames)
        {
            try
            {
                Log.Information(string.Format("Trying add hook in: {0}", functionName));

                List<string>? originalCode = GetStringGMLFromFile(functionName).Split("\n").ToList();
                originalCode.Append($"var {hookName} = createHookObj({paramNames.Length}, {string.Join(", ", paramNames)})");
                originalCode.Append($"SendMsg(\"HOK\", \"{hookName}<EXTRAMSG>\" + {hookName}, false)");
                SetStringGMLInFile(string.Join("\n", originalCode), functionName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
    }
}
