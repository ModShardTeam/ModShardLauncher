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
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using UndertaleModLib.Models;
using ModShardLauncher.Extensions;
using ModShardLauncher.Controls;
using Serilog;
using Xunit.Sdk;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ModShardLauncher
{
    public static class ModLoader
    {
        internal static UndertaleData Data => DataLoader.data;
        public static string ModPath => Path.Join(Environment.CurrentDirectory, "Mods");
        public static string ModSourcesPath => Path.Join(Environment.CurrentDirectory, "ModSources");
        public static string? StoneshardInstallPath = null;
        public static Dictionary<string, ModFile> Mods = new();
        public static Dictionary<string, ModSource> ModSources = new();
        private static List<Assembly> Assemblies = new();
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
            WeaponDescriptions = Msl.ThrowIfNull(GetTable("gml_GlobalScript_table_weapons_text"));
        }
        public static void LocateStoneshardFolder()
        {
            HashSet<string?> locations = new();
            string? selectedPath = null;
            
            // Check Settings for existing path
            if (Main.Settings.StoneshardPath != null)
            {
                locations.Add(Main.Settings.StoneshardPath);
                if (VerifyInstallLocations(locations).Count >= 1)
                {
                    Log.Information("Stoneshard path loaded from settings.");
                    StoneshardInstallPath = Main.Settings.StoneshardPath;
                    return;
                }
                Log.Warning($"Stoneshard path loaded from settings is not a valid installation. ({Main.Settings.StoneshardPath})");
                Log.Warning("Saved path has been removed.");
                Main.Settings.StoneshardPath = null;
                locations.Clear();
            }
            
            Log.Information("No valid Stoneshard install found in Settings. Scanning...");
            
            // Check steam + gog default locations
            locations.UnionWith(GetDefaultInstallLocations());
            
            // Check registry for installed programs
            locations.UnionWith(GetRegistryInstallLocations());
            
            // Verify the folder contains `Stoneshard.exe` and `dialogs` folder
            locations = VerifyInstallLocations(locations); 
            
            // TODO REMOVE DEBUG: print all locations found
            Log.Information("Found valid install locations :");
            foreach (string location in locations.OfType<string>())
                Log.Information(location);
            
            switch (locations.Count)
            {
                case 1:
                {
                    selectedPath = locations.FirstOrDefault();
                    break;
                }
                // No path found: Ask user to locate the folder
                case < 1:
                {
                    Log.Warning("Couldn't locate Stoneshard folder, asking user.");
                    FolderBrowserDialog dialog = new();
                    HashSet<string?> toVerify = new();
                        
                    dialog.Description = "Select your Stoneshard installation folder";
                    dialog.ShowDialog();
                    
                    toVerify.Add(dialog.SelectedPath); if (VerifyInstallLocations(toVerify).Count == 1)
                        selectedPath = dialog.SelectedPath;

                    if (string.IsNullOrEmpty(selectedPath))
                    {
                        Log.Error("No Stoneshard install directory was selected.This could cause issues when patching mods.");
                        MessageBox.Show(
                            "No Stoneshard install directory was selected.\nThis could cause issues when patching mods.",
                            "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error
                            );
                        return;
                    }

                    break;
                }
                
                // More than 1 path found: Ask user to pick one
                case >= 2:
                {
                    Log.Information("Multiple Stoneshard installs found, asking user.");
                    // TODO: open list dialog
                    break;
                }
            }
            
            // Save to Settings
            if (!string.IsNullOrEmpty(selectedPath))
            {
                Log.Information($"Saving Stoneshard install path to Settings.");
                Main.Settings.StoneshardPath = selectedPath;
            }
            else
                Log.Error("Failed saving path to settings.");
            
            
            // Expose to modders
            StoneshardInstallPath = Main.Settings.StoneshardPath;
        }

        private static HashSet<string?> GetDefaultInstallLocations()
        {
            HashSet<string?> results = new();

            string[] defaultPaths =
            {
                @"C:\Program Files (x86)\Steam\steamapps\common\Stoneshard", // Steam
                @"$HOME\GOG Games", // GOG
                @"C:\GOG Games" // GOG Galaxy ?
            };

            foreach (string path in defaultPaths)
            {
                if (Directory.Exists(path))
                    results.Add(path);
            }
            
            return results;
        }
        private static HashSet<string?> GetRegistryInstallLocations()
        {
            string[] registryKeys = {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };
            RegistryHive[] hives = {
                RegistryHive.LocalMachine,
                RegistryHive.CurrentUser
            };
            RegistryView[] views = {
                RegistryView.Registry32,
                RegistryView.Registry64
            };
            HashSet<string?> results = new();

            foreach (RegistryHive hive in hives)
            {
                foreach (string registryKey in registryKeys)
                {
                    foreach (RegistryView view in views)
                    {
                        using RegistryKey? key = RegistryKey.OpenBaseKey(hive, view).OpenSubKey(registryKey);
                        if (key == null) continue;
                        foreach (string subkeyName in key.GetSubKeyNames())
                        {
                            using RegistryKey? subkey = key.OpenSubKey(subkeyName);
                            if (subkey?.GetValue("DisplayName")?.ToString()?.Equals("Stoneshard") == true)
                                results.Add(subkey.GetValue("InstallLocation")?.ToString());
                        }
                    }
                }
            }

            return results;
        }

        private static HashSet<string?> VerifyInstallLocations(HashSet<string?> locations)
        {
            HashSet<string?> results = new();
            
            foreach (string? location in locations)
            {
                if (File.Exists(location + @"\Stoneshard.exe") && Directory.Exists(location + @"\dialogs"))
                    results.Add(location);
            }
            
            return results;
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
            Credits = new();
            Disclaimers = new();
            List<ModFile> mods = ModInfos.Instance.Mods;
            Menus = new();
            foreach (ModFile mod in mods)
            {
                if (!mod.isEnabled) continue;
                if (!mod.isExisted)
                {
                    MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + mod.Name);
                    continue;
                }
                Main.Settings.EnableMods.Add(mod.Name);
                mod.PatchStatus = PatchStatus.Patching;

                // work around to find the FileVersion of ModShardLauncher.dll for single file publishing
                // see: https://github.com/dotnet/runtime/issues/13051
                ProcessModule mainProcess = Msl.ThrowIfNull(Process.GetCurrentProcess().MainModule);
                string mainProcessName = Msl.ThrowIfNull(mainProcess.FileName);
                string mod_version = "v" + FileVersionInfo.GetVersionInfo(mainProcessName).FileVersion;

                if (mod.Version != mod_version)
                {
                    MessageBoxResult result = MessageBox.Show(
                        Application.Current.FindResource("VersionDifferentWarning").ToString(),
                        Application.Current.FindResource("VersionDifferentWarningTitle").ToString() + " : " + mod.Name, 
                        MessageBoxButton.OK
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
                mod.PatchStatus = PatchStatus.Success;
            }
            Msl.AddDisclaimerRoom(Credits.Select(x => x.Item1).ToArray(), Credits.SelectMany(x => x.Item2).Distinct().ToArray());
            Msl.ChainDisclaimerRooms(Disclaimers);
            Msl.CreateMenu(Menus);
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
            // add the new loot related functions if there is any
            LootUtils.InjectLootScripts();
        }
        internal static void PatchInnerFile()
        {
            if (Data.Code.All(x => x.Name.Content != "print"))
                Msl.AddInnerFunction("print");
            if (Data.Code.All(x => x.Name.Content != "give"))
                Msl.AddInnerFunction("give");
            if (Data.Code.All(x => x.Name.Content != "SendMsg"))
                Msl.AddInnerFunction("SendMsg");
            if (Data.Code.All(x => x.Name.Content != "createHookObj"))
                Msl.AddInnerFunction("createHookObj");
            
            if (Data.Extensions.All(x => x.Name.Content != "display_mouse_lock"))
                throw new InvalidOperationException("The display_mouse_lock extension is not found.");
            if (Data.Extensions.First(x => x.Name.Content == "display_mouse_lock")
                .Files.All(x => x.Filename.Content != "ModShard.dll"))
                AddExtension(new ModShard());
            
            if (Data.GameObjects.All(x => x.Name.Content != "o_ScriptEngine"))
            {
                UndertaleGameObject engine = Msl.AddObject("o_ScriptEngine");
                engine.Persistent = true;
                UndertaleGameObject.Event ev = new()
                {
                    EventSubtypeOther = EventSubtypeOther.AsyncNetworking
                };
                ev.Actions.Add(new UndertaleGameObject.EventAction()
                {
                    CodeId = Msl.AddInnerCode("ScriptEngine_server")
                });
                engine.Events[7].Add(ev);
                UndertaleGameObject.Event create = new();
                create.Actions.Add(new UndertaleGameObject.EventAction()
                {
                    CodeId = Msl.AddInnerCode("ScriptEngine_create")
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
            else
                // Should probably be replaced with a dialog box as it's not very visible as it is 
                Log.Warning("You are patching a non-vanilla .win file. This may cause some issues and is not recommended.");
        }
        public static void AddExtension(UndertaleExtensionFile file)
        {
            UndertaleExtension ext = Data.Extensions.First(t => t.Name.Content == "display_mouse_lock");
            ext.Files.Add(file);
        }
    }
}
