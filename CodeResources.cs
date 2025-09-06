using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;

namespace ModShardLauncher
{
    internal static class CodeResources
    {
        private static readonly Lazy<Dictionary<string, string>> _scripts = new(LoadAllScripts);
        public static string GetGML(string name)
        {
            if (_scripts.Value.TryGetValue(name, out var script)) return script;

            throw new ArgumentException($"GML script '{name}' not found");
        }
        private static Dictionary<string, string> LoadAllScripts()
        {
            Dictionary<string, string> scripts = new();
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            // Load all .gml resources
            IEnumerable<string> resourceNames = assembly.GetManifestResourceNames()
                .Where(name => name.EndsWith(".gml"));
            foreach (string resourceName in resourceNames)
            {
                string? scriptName = Path.GetFileNameWithoutExtension(resourceName);

                using Stream? stream =
                    assembly.GetManifestResourceStream(resourceName) ??
                    throw new FileNotFoundException($"GML script '{scriptName}' not found");
                using StreamReader reader = new(stream);
                scripts[scriptName.Split('.').Last()] = reader.ReadToEnd();
            }
            
            return scripts;
        }
    }
}
