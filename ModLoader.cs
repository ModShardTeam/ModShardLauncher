using System.Collections.Generic;
using ModShardLauncher.Mods;
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
            try
            {
                string ret = JsonConvert.SerializeObject(table).Replace("\n", "");
                UndertaleCode target = Data.Code.First(t => t.Name.Content == name);
                GlobalDecompileContext context = new(Data, false);
                string text = Decompiler.Decompile(target, context);
                text = Regex.Replace(text, "\\[.*\\]", ret);
                target.ReplaceGML(text, Data);

                Log.Information(string.Format("Successfully set table: {0}", name.ToString()));
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static void LoadFiles()
        {
            List<ModFile> mods = Main.Instance.ModPage.Mods;
            List<ModSource> modSources = Main.Instance.ModSourcePage.ModSources;
            foreach(ModFile i in mods)
                i.Stream?.Close();
            
            List<ModFile> modCaches = new();
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

            foreach(string source in sources)
            {
                ModSource info = new()
                {
                    Name = source.Split("\\")[^1],
                    Path = source
                };
                modSources.Add(info);
            }

            string[] files = Directory.GetFiles(ModPath, "*.sml");
            foreach (string file in files)
            {
                ModFile? f = null;
                try
                {
                    f = FileReader.Read(file);
                }
                catch(Exception ex)
                {
                    Log.Information(ex, string.Format("Cannot read the mod {0}", file));
                }
                if (f == null) continue;
                try
                {
                    Assembly assembly = f.Assembly;
                    // for array or list, use the available search method instead of Linq one
                    // use the Linq ones for IEnumerable
                    Type? modType = Array.Find(assembly.GetTypes(), t => t.IsSubclassOf(typeof(Mod)));

                    if (modType == null)
                    {
                        MessageBox.Show("加载错误: " + assembly.GetName().Name + " 此Mod需要一个Mod类");
                        continue;
                    }
                    else
                    {
                        if (Activator.CreateInstance(modType) is not Mod mod) continue;
                        mod.LoadAssembly();
                        mod.ModFiles = f;
                        f.instance = mod;

                        ModFile? old = mods.Find(t => t.Name == f.Name);
                        if (old != null) f.Enabled = old.Enabled;

                        modCaches.Add(f);
                    }
                }
                catch
                {
                    throw;
                }
            }
            mods.Clear();
            modCaches.ForEach(i => {
                mods.Add(i);
            });
        }
        public static void PatchMods()
        {
            Credits = new();
            Disclaimers = new();
            List<ModFile> mods = ModInfos.Instance.Mods;
            Menus = new();

            Stopwatch watch = Stopwatch.StartNew();
            foreach (ModFile mod in mods)
            {
                if (!mod.Enabled) continue;
                if (!mod.Existed)
                {
                    MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + mod.Name);
                    continue;
                }

                Main.Settings.EnabledMods.Add(mod.Name);
                mod.PatchStatus = PatchStatus.Patching;

                if (mod.Version != Main.Instance.mslVersion)
                {
                    Log.Warning("Mod {{{0}}} was built with msl {{{1}}} which is different from the current msl {{{2}}}", mod.Name, mod.Version, Main.Instance.mslVersion);
                }
                TextureLoader.LoadTextures(mod);
                mod.instance.PatchMod();
                mod.PatchStatus = PatchStatus.Success;
            }
            Msl.AddDisclaimerRoom(Credits.Select(x => x.Item1).ToArray(), Credits.SelectMany(x => x.Item2).Distinct().ToArray());
            Msl.ChainDisclaimerRooms(Disclaimers);
            Msl.CreateMenu(Menus);

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Log.Information("Patching lasts {{{0}}} ms", elapsedMs);
        }
        public static void PatchFile()
        {
            // add new msl log function
            LogUtils.InjectLog();
            PatchMods();
            // add the new loot related functions if there is any
            LootUtils.InjectLootScripts();
        }
    }
}
