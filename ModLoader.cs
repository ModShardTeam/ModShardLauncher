using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UndertaleModLib;
using System.Diagnostics;
using UndertaleModLib.Decompiler;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using UndertaleModLib.Models;
using ModShardLauncher.Controls;
using Serilog;
using ModShardLauncher.Core.Errors;
using ModShardLauncher.Core.Models;
using ModShardLauncher.Mods;

namespace ModShardLauncher
{
    public static class ModLoader
    {
        internal static UndertaleData Data => DataLoader.data;
        public static string ModPath => Path.Join(Environment.CurrentDirectory, "Mods");
        public static string ModSourcesPath => Path.Join(Environment.CurrentDirectory, "ModSources");
        private static List<Menu> Menus = new();
        public static List<string> Weapons = new();
        public static List<string> WeaponDescriptions = new();
        private static List<(string, string[])> Credits = new();
        private static List<(string, UndertaleRoom.GameObject)> Disclaimers = new();
        public static Dictionary<string, Action<string>> ScriptCallbacks = new Dictionary<string, Action<string>>();
        public static void ShowMessage(string msg)
        {
            Trace.Write(msg);
        }
        public static void Initalize()
        {
            Weapons = Msl.ThrowIfNull(GetTable("gml_GlobalScript_table_weapons"));
            WeaponDescriptions = Msl.ThrowIfNull(GetTable("gml_GlobalScript_table_equipment"));
        }
        internal static void AddCredit(string modNameShort, string[] authors)
        {
            Credits.Add((modNameShort, authors));
        }
        internal static void AddDisclaimer(string modNameShort, UndertaleRoom.GameObject overlay)
        {
            Disclaimers.Add((modNameShort, overlay));
        }
        public static void AddMenu(string name, params UIComponent[] components)
        {
            Menus.Add(new Menu(name, components));
        }
        public static List<string>? GetTable(string name)
        {
            try
            {
                UndertaleCode table = Data.Code.First(t => t.Name.Content == name);
                return table.Instructions
                    .Where(i => AssemblyWrapper.IsPushString(i))
                    .Select(i => (i.Value as UndertaleResourceById<UndertaleString, UndertaleChunkSTRG>)!.Resource.Content)
                    .Reverse()
                    .ToList();
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static void SetTable(List<string> table, string name)
        {
            string ret = JsonConvert.SerializeObject(table).Replace("\n", "");
            UndertaleCode target = Data.Code.First(t => t.Name.Content == name);
            GlobalDecompileContext context = new(Data, false);
            string text = Decompiler.Decompile(target, context);
            text = Regex.Replace(text, "\\[.*\\]", ret);
            target.ReplaceGML(text, Data);

            Log.Information("Successfully set table: {0}", name);
        }
        private static void LoadSourceFiles()
        {
            List<ModSource> modSources = Main.Instance.ModSourcePage.ModSources;
            modSources.Clear();

            // List all folders being a C# project
            // Currently only test the existence of a .csproj file
            // TODO: test framework
            // TODO: test inclusion of ModShardLauncher as a reference
            IEnumerable<string> sources = Directory
                .GetDirectories(ModSourcesPath)
                .Where(
                    x => Directory
                        .EnumerateFiles(x, "*.csproj", SearchOption.TopDirectoryOnly)
                        .FirstOrDefault()
                        != null
            );

            foreach (string source in sources)
            {
                ModSource info = new()
                {
                    Name = source.Split("\\")[^1],
                    Path = source
                };
                modSources.Add(info);
            }
        }
        private static bool GetConcreteType(Type typeToGet, Type t)
        {
            // expect a non interface, non abstract typeToGet and with a constructor without any parameter
            return typeToGet.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null;
        }
        private static ModFile? LoadModFile(string file)
        {
            ModFile? f = null;

            // read the file
            try
            {
                f = FileReader.Read(file);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "Cannot read the mod {0}", file);
            }
            if (f == null) return null;

            Assembly assembly = f.Assembly;
            Type[] types;

            // load all types in the assembly
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Log.Error("Failed to load types from assembly {0}: {1}", assembly.GetName().Name, ex.Message);
                foreach (Exception? loaderEx in ex.LoaderExceptions.Where(e => e != null))
                {
                    Log.Warning("Loader exception: {LoaderError}", loaderEx!.Message);
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error loading types from {0}: {1}", assembly.GetName().Name, ex.Message);
                return null;
            }

            // capture the Mod type if it exists
            Type? modType = Array.Find(types, t => GetConcreteType(typeof(Mod), t));

            // check if Mod was correctly found
            if (modType == null)
            {
                Log.Warning(
                    "No valid Mod class found in assembly {0}. Expected a non-abstract class inheriting from Mod with parameterless constructor.",
                    assembly.GetName().Name
                );
                return null;
            }

            try
            {
                if (Activator.CreateInstance(modType) is not Mod mod)
                {
                    Log.Error("Created instance of {0} is not assignable to Mod (this should not happen)", modType.Name);
                    return null;
                }
                mod.ModFiles = f;
                f.Instance = mod;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to create instance of mod type {0}: {1}", modType.Name, ex.Message);
                return null;
            }

            return f;
        }
        private static void LoadModFiles()
        {
            List<ModFile> mods = Main.Instance.ModPage.Mods;
            foreach (ModFile mod in mods) mod.Stream?.Close();
            List<ModFile> modCaches = new();

            string[] files = Directory.GetFiles(ModPath, "*.sml");

            foreach (string file in files)
            {
                ModFile? f = LoadModFile(file);
                if (f == null) continue;
                ModFile? old = mods.Find(t => t.Name == f.Name);
                if (old != null) f.Enabled = old.Enabled;

                modCaches.Add(f);
            }

            mods.Clear();
            mods.AddRange(modCaches);
        }
        public static void LoadFiles()
        {
            LoadSourceFiles();
            LoadModFiles();
        }
        public static void PatchMods()
        {
            Credits = new();
            Disclaimers = new();
            List<ModFile> mods = ModInfos.Instance.Mods;
            Menus = new();

            foreach (ModFile mod in mods)
            {
                if (!mod.Enabled) continue;
                if (!mod.Existed)
                {
                    Log.Warning("The mod {0} which was located at {1} does not exist anymore.", mod.Name, mod.Path);
                    continue;
                }

                Main.Settings.EnabledMods.Add(mod.Name);
                mod.PatchStatus = PatchStatus.Patching;

                if (mod.Version != Main.Instance.mslVersion)
                {
                    Log.Warning("Mod {{{0}}} was built with msl {{{1}}} which is different from the current msl {{{2}}}", mod.Name, mod.Version, Main.Instance.mslVersion);
                }
                TextureLoader.LoadTextures(mod);
                mod.Instance.PatchMod();
                mod.PatchStatus = PatchStatus.Success;
                Main.LogModStatus(mod);
            }
            Msl.AddDisclaimerRoom(Credits.Select(x => x.Item1).ToArray(), Credits.SelectMany(x => x.Item2).Distinct().ToArray());
            Msl.ChainDisclaimerRooms(Disclaimers);
            Msl.CreateMenu(Menus);
        }
        public static MSLDiagnostic? PatchFile()
        {
            try
            {
                // add new msl log function
                LogUtils.InjectLog();
                PatchMods();
                // add the new loot related functions if there is any
                LootUtils.InjectLootScripts();
                return null;
            }
            catch (Exception ex)
            {
                MSLDiagnostic diag = new(ex, Main.Instance.GetFailingMod());
                diag.ToLog();
                return diag;
            }
        }
    }
}
