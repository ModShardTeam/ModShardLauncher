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
using ModShardLauncher.Extensions;
using ModShardLauncher.Pages;
using Serilog;

namespace ModShardLauncher
{
    public static class ModLoader
    {
        internal static UndertaleData Data => DataLoader.data;
        public static string ModPath => Path.Join(Environment.CurrentDirectory, "Mods");
        public static string ModSourcesPath => Path.Join(Environment.CurrentDirectory, "ModSources");
        public static Dictionary<string, ModFile> Mods = new();
        public static Dictionary<string, ModSource> ModSources = new();
        private static List<Assembly> Assemblies = new();
        public static List<string> Weapons = new();
        public static List<string> WeaponDescriptions = new();
        public static void ShowMessage(string msg)
        {
            Trace.Write(msg);
        }
        public static void Initalize()
        {
            Weapons = Msl.ThrowIfNull(GetTable("gml_GlobalScript_table_weapons"));
            WeaponDescriptions = Msl.ThrowIfNull(GetTable("gml_GlobalScript_table_weapons_text"));
        }
        public static List<string>? GetTable(string name)
        {
            try
            {
                UndertaleCode table = Data.Code.First(t => t.Name.Content == name);
                GlobalDecompileContext context = new(Data, false);
                string text = Decompiler.Decompile(table, context);
                string matchedText = Regex.Match(text, "return (\\[.*\\])").Groups[1].Value;
                List<string>? tableAsList = JsonConvert.DeserializeObject<List<string>>(matchedText);

                Log.Information(string.Format("Get table: {0}", name.ToString()));
                return tableAsList;
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
            Mods.Clear();
            modSources.Clear();
            ModSources.Clear();

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
                ModSources.Add(info.Name, info);
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
                        if (old != null) f.isEnabled = old.isEnabled;

                        modCaches.Add(f);
                    }
                    Assemblies.Add(assembly);
                }
                catch
                {
                    throw;
                }
            }
            mods.Clear();
            modCaches.ForEach(i => {
                mods.Add(i);
                Mods.Add(i.Name, i);
            });
        }
        public static void PatchMods()
        {
            List<ModFile> mods = ModInfos.Instance.Mods;
            foreach (ModFile mod in mods)
            {
                if (!mod.isEnabled) continue;
                if (!mod.isExisted)
                {
                    MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + mod.Name);
                    continue;
                }
                Main.Settings.EnableMods.Add(mod.Name);
                string version = DataLoader.GetVersion();
                Regex reg = new("0([0-9])");
                version = reg.Replace(version, "$1");
                if (mod.Version != version)
                {
                    MessageBoxResult result = MessageBox.Show(
                        Application.Current.FindResource("VersionDifferentWarning").ToString(),
                        Application.Current.FindResource("VersionDifferentWarningTitle").ToString() + " : " + mod.Name, 
                        MessageBoxButton.YesNo, 
                        MessageBoxImage.Question
                    );
                    if (result == MessageBoxResult.No) continue;
                }
                TextureLoader.LoadTextures(mod);
                mod.instance.PatchMod();
                foreach (Type type in Array.FindAll(mod.Assembly.GetTypes(), t => !t.IsAbstract))
                {
                    if (type.IsSubclassOf(typeof(Weapon))) 
                        LoadWeapon(type);
                }
            }
        }
        public static void LoadWeapon(Type type)
        {
            if (Activator.CreateInstance(type) is not Weapon weapon) return;
            weapon.SetDefaults();
            (string, string, string) strs = weapon.AsString();
            Weapons.Insert(Weapons.IndexOf("SWORDS - BLADES;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;") + 1, strs.Item1);
            WeaponDescriptions.Insert(WeaponDescriptions.IndexOf(";;SWORDS;;;;;;SWORDS;SWORDS;;;;") + 1, weapon.Name + ";" + string.Join(";", weapon.NameList.Values));
            WeaponDescriptions.Insert(WeaponDescriptions.IndexOf(";weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;") + 1,
                weapon.Name + ";" + string.Join(";", weapon.WeaponDescriptions.Values));
            WeaponDescriptions.Insert(WeaponDescriptions.IndexOf(";weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;") + 1,
                weapon.Name + ";He;;;It;She;She;She;She;He;;;;");
        }
        public static void PatchFile()
        {
            PatchInnerFile();
            PatchMods();
            SetTable(Weapons, "gml_GlobalScript_table_weapons");
            SetTable(WeaponDescriptions, "gml_GlobalScript_table_weapons_text");
            try
            {
                LoadFiles();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }
        }
        internal static void PatchInnerFile()
        {
            AddExtension(new ModShard());
            UndertaleGameObject engine = Msl.AddObject("o_ScriptEngine");
            engine.Persistent = true;
            UndertaleGameObject.Event ev = new()
            {
                EventSubtypeStep = EventSubtypeStep.Step
            };
            ev.Actions.Add(new UndertaleGameObject.EventAction()
            {
                CodeId = Msl.AddCode(@"if(GetScript() != ""NoScript"")
{
    var scr = string_split(GetScript(),"" "")
    var scriptID = asset_get_index(scr[0])
    array_delete(scr, 0, 1)
    for(i = 0;i<array_length(scr);i++)
        scr[i]=string_replace(scr[i],""_"","" "")
    var ret = """"
    if(scriptID == -1)
        ret = (""script is wrong: "" + GetScript())
    else
    {
        if(array_length(scr) > 0)
            ret = script_execute_ext(scriptID, scr)
        else ret = script_execute(scriptID)
    }
    RunCallBack(string(ret))
    PopScript()
}", "ScriptEngine_step")
            });
            engine.Events[3].Add(ev);
            Msl.AddFunction(@"function print(argument0)
{
    show_message(argument0)
}","print");
            Msl.AddFunction(@"function give(argument0)
{
    with (o_inventory)
    {
        with (scr_inventory_add_weapon(argument0, (1 << 0)))
            scr_inv_atr_set(""Duration"", 100)
    }
}", "give");
            UndertaleGameObject.Event create = new();
            create.Actions.Add(new UndertaleGameObject.EventAction()
            {
                CodeId = Msl.AddCode(@"ScriptThread()", "ScriptEngine_create")
            });
            engine.Events[0].Add(create);
            UndertaleRoom start = Data.Rooms.First(t => t.Name.Content == "START");
            UndertaleRoom.GameObject newObj = new()
            {
                ObjectDefinition = engine,
                InstanceID = Data.GeneralInfo.LastObj++
            };

            start.GameObjects.Add(newObj);
        }
        public static void AddExtension(UndertaleExtensionFile file)
        {
            UndertaleExtension ext = Data.Extensions.First(t => t.Name.Content == "display_mouse_lock");
            ext.Files.Add(file);
        }
    }
}
