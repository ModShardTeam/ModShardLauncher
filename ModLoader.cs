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
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Controls;
using UndertaleModLib.Models;
using ModShardLauncher.Extensions;
using UndertaleModLib.Compiler;
using System.Text;
using System.Xml.Linq;
using ModShardLauncher.Pages;
using Serilog;

namespace ModShardLauncher
{
    public class ModLoader
    {
        internal static UndertaleData Data => DataLoader.data;
        public static string ModPath => Path.Join(Environment.CurrentDirectory, "Mods");
        public static string ModSourcesPath => Path.Join(Environment.CurrentDirectory, "ModSources");
        public static Dictionary<string, ModFile> Mods = new Dictionary<string, ModFile>();
        public static Dictionary<string, ModSource> ModSources = new Dictionary<string, ModSource>();
        private static List<Assembly> Assemblies = new List<Assembly>();
        private static bool patched = false;
        public static List<string> Weapons;
        public static List<string> WeaponDescriptions;
        public static void ShowMessage(string msg)
        {
            Trace.Write(msg);
        }
        public static void Initalize()
        {
            Weapons = GetTable("gml_GlobalScript_table_weapons");
            WeaponDescriptions = GetTable("gml_GlobalScript_table_weapons_text");
        }
        public static UndertaleGameObject AddObject(string name)
        {
            var obj = new UndertaleGameObject()
            {
                Name = Data.Strings.MakeString(name)
            };
            if(Data.GameObjects.FirstOrDefault(t => t.Name.Content == name) == default)
                Data.GameObjects.Add(obj);
            return obj;
        }
        public static UndertaleGameObject GetObject(string name)
        {
            return Data.GameObjects.FirstOrDefault(t => t.Name.Content == name);
        }
        public static UndertaleSprite GetSprite(string name)
        {
            return Data.Sprites.FirstOrDefault(t => t.Name.Content == name);
        }
        public static void SetObject(string name, UndertaleGameObject o)
        {
            var obj = Data.GameObjects.First(t => t.Name.Content.IndexOf(name) != -1);
            Data.GameObjects[Data.GameObjects.IndexOf(obj)] = o;
        }
        public static UndertaleCode AddCode(string Code, string name)
        {
            var code = new UndertaleCode();
            var locals = new UndertaleCodeLocals();
            code.Name = Data.Strings.MakeString(name);
            locals.Name = code.Name;
            UndertaleCodeLocals.LocalVar argsLocal = new UndertaleCodeLocals.LocalVar();
            argsLocal.Name = Data.Strings.MakeString("arguments");
            argsLocal.Index = 0;
            locals.Locals.Add(argsLocal);
            code.LocalsCount = 1;
            Data.CodeLocals.Add(locals);
            code.ReplaceGML(Code, Data);
            Data.Code.Add(code);
            return code;
        }
        public static UndertaleCode AddFunction(string Code, string name)
        {
            var scriptCode = AddCode(Code, name);
            Data.Code.Add(Data.Code[0]);
            Data.Code.RemoveAt(0);
            return scriptCode;
        }
        public static List<string> GetTable(string name)
        {
            var table = Data.Code.First(t => t.Name.Content.IndexOf(name) != -1);
            GlobalDecompileContext context = new GlobalDecompileContext(Data, false);
            var text = Decompiler.Decompile(table, context);
            var ret = Regex.Match(text, "return (\\[.*\\])").Groups[1].Value;
            return JsonConvert.DeserializeObject<List<string>>(ret);
        }
        public static UndertaleCode GetCode(string name)
        {
            try {
                var code = Data.Code.First(t => t.Name.Content == name);
                Log.Information(string.Format("Find function: {0}", code.ToString()));

                return code;
            }
            catch(Exception ex) {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static string GetDecompiledCode(string name)
        {
            try {
                var func = GetCode(name);
                GlobalDecompileContext context = new GlobalDecompileContext(Data, false);
                var text = Decompiler.Decompile(func, context);
                
                return text;
            }
            catch(Exception ex) {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static string GetDisassemblyCode(string name)
        {
            var func = Data.Code.First(t => t.Name.Content.IndexOf(name) != -1);
            var text = func.Disassemble(Data.Variables, Data.CodeLocals.For(func));
            
            return text;
        }
        public static void SetDecompiledCode(string Code, string name)
        {
            var code = GetCode(name);
            code.ReplaceGML(Code, Data);
        }
        public static void InsertDecompiledCode(string Code, string name, int pos)
        {
            try {
                Log.Information(string.Format("Trying insert code in: {0}", name.ToString()));
                var code = GetDecompiledCode(name).Split("\n").ToList();
                code.Insert(pos, Code);
                SetDecompiledCode(string.Join("\n", code), name);
                Log.Information(string.Format("Patched function with InsertDecompiledCode: {0}", name.ToString()));
            }
            catch(Exception ex) {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static void ReplaceDecompiledCode(string Code, string name, int pos)
        {
            var code = GetDecompiledCode(name).Split("\n").ToList();
            code[pos] = Code;
            SetDecompiledCode(string.Join("\n", code), name);
        }
        public static void SetDisassemblyCode(string Code, string name)
        {
            var code = Data.Code.First(t => t.Name.Content.IndexOf(name) != -1);
            code.ReplaceGML(Code, Data);
        }
        public static void SetTable(List<string> table, string name)
        {
            var ret = JsonConvert.SerializeObject(table).Replace("\n", "");
            var target = Data.Code.First(t => t.Name.Content.IndexOf(name) != -1);
            GlobalDecompileContext context = new GlobalDecompileContext(Data, false);
            var text = Decompiler.Decompile(target, context);
            text = Regex.Replace(text, "\\[.*\\]", ret);
            target.ReplaceGML(text, Data);
        }
        public static Weapon GetWeapon(string ID)
        {
            var str = Weapons.First(t => t.StartsWith(ID));
            var descs = WeaponDescriptions.FindAll(t => t.StartsWith(ID))[1].Split(";").ToList();
            descs.Remove("");
            descs.RemoveAt(0);
            var names = WeaponDescriptions.First(t => t.StartsWith(ID)).Split(";").ToList();
            names.Remove("");
            names.RemoveAt(0);
            var weapon = new Weapon(str, descs, names);
            return weapon;
        }
        public static void SetWeapon(string ID, Weapon weapon)
        {
            var target = Weapons.First(t => t.StartsWith(ID));
            var name = WeaponDescriptions.First(t => t.StartsWith(ID));
            var desc = WeaponDescriptions.FindAll(t => t.StartsWith(ID))[1];
            var index = Weapons.IndexOf(target);
            var index2 = WeaponDescriptions.IndexOf(desc);
            var index3 = WeaponDescriptions.IndexOf(name);
            Weapons[index] = Weapon.Weapon2String(weapon).Item1;
            WeaponDescriptions[index2] = Weapon.Weapon2String(weapon).Item2;
            WeaponDescriptions[index3] = Weapon.Weapon2String(weapon).Item3;
        }
        public static void LoadFiles()
        {
            var mods = Main.Instance.ModPage.Mods;
            var modSources = Main.Instance.ModSourcePage.ModSources;
            foreach(ModFile i in mods)
                if(i.Stream != null) i.Stream.Close();
            var modCaches = new List<ModFile>();
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

            foreach(var i in sources)
            {
                var info = new ModSource()
                {
                    Name = i.Split("\\").Last(),
                    Path = i
                };
                modSources.Add(info);
                ModSources.Add(info.Name, info);
            }
            var files = Directory.GetFiles(ModPath, "*.sml");
            foreach (var file in files)
            {
                var f = FileReader.Read(file);
                if (f == null) continue;
                Assembly assembly = f.Assembly;
                if (assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Mod))).Count() == 0)
                {
                    MessageBox.Show("加载错误: " + assembly.GetName().Name + " 此Mod需要一个Mod类");
                    continue;
                }
                else
                {
                    var type = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Mod))).ToList()[0];
                    var mod = Activator.CreateInstance(type) as Mod;
                    mod.LoadAssembly();
                    mod.ModFiles = f;
                    f.instance = mod;
                    var old = mods.FirstOrDefault(t => t.Name == f.Name);
                    if (old != null) f.isEnabled = old.isEnabled;
                    modCaches.Add(f);
                }
                Assemblies.Add(assembly);
            }
            mods.Clear();
            modCaches.ForEach(i => {
                mods.Add(i);
                Mods.Add(i.Name, i);
            });
        }
        public static void PatchMods()
        {
            Assembly ass = Assembly.GetEntryAssembly();
            var mods = ModInfos.Instance.Mods;
            foreach (ModFile mod in mods)
            {
                if (!mod.isEnabled) continue;
                if (!mod.isExisted)
                {
                    MessageBox.Show(Application.Current.FindResource("ModLostWarning").ToString() + " : " + mod.Name);
                    continue;
                }
                Main.Settings.EnableMods.Add(mod.Name);
                var version = DataLoader.GetVersion();
                var reg = new Regex("0([0-9])");
                version = reg.Replace(version, "$1");
                if (mod.Version != version)
                {
                    var result = MessageBox.Show(Application.Current.FindResource("VersionDifferentWarning").ToString(),
                        Application.Current.FindResource("VersionDifferentWarningTitle").ToString() + " : " + mod.Name, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No) continue;
                }
                TextureLoader.LoadTextures(mod);
                mod.instance.PatchMod();
                var modAss = mod.Assembly;
                Type[] types = modAss.GetTypes().Where(t => !t.IsAbstract).ToArray();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(Weapon))) 
                        LoadWeapon(type);
                }
            }
        }
        public static void LoadWeapon(Type type)
        {
            var weapon = Activator.CreateInstance(type) as Weapon;
            weapon.SetDefaults();
            var strs = weapon.AsString();
            Weapons.Insert(Weapons.IndexOf("SWORDS - BLADES;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;") + 1, strs.Item1);
            WeaponDescriptions.Insert(WeaponDescriptions.IndexOf(";;SWORDS;;;;;;SWORDS;SWORDS;;;;") + 1, weapon.Name + ";" + string.Join(";", weapon.NameList.Values));
            WeaponDescriptions.Insert(WeaponDescriptions.IndexOf(";weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;weapon_desc;") + 1,
                weapon.Name + ";" + string.Join(";", weapon.WeaponDescriptions.Values));
            WeaponDescriptions.Insert(WeaponDescriptions.IndexOf(";weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;weapon_pronoun;") + 1,
                weapon.Name + ";He;;;It;She;She;She;She;He;;;;");
        }
        public static async void PatchFile()
        {
            PatchInnerFile();
            PatchMods();
            SetTable(Weapons, "gml_GlobalScript_table_weapons");
            SetTable(WeaponDescriptions, "gml_GlobalScript_table_weapons_text");
            LoadFiles();
        }
        internal static void PatchInnerFile()
        {
            AddExtension(new ModShard());
            var engine = AddObject("o_ScriptEngine");
            engine.Persistent = true;
            var ev = new UndertaleGameObject.Event();
            ev.EventSubtypeStep = EventSubtypeStep.Step;
            ev.Actions.Add(new UndertaleGameObject.EventAction()
            {
                CodeId = AddCode(@"if(GetScript() != ""NoScript"")
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
            AddFunction(@"function print(argument0)
{
    show_message(argument0)
}","print");
            AddFunction(@"function give(argument0)
{
    with (o_inventory)
    {
        with (scr_inventory_add_weapon(argument0, (1 << 0)))
            scr_inv_atr_set(""Duration"", 100)
    }
}", "give");
            var create = new UndertaleGameObject.Event();
            create.Actions.Add(new UndertaleGameObject.EventAction()
            {
                CodeId = AddCode(@"ScriptThread()", "ScriptEngine_create")
            });
            engine.Events[0].Add(create);
            var start = Data.Rooms.First(t => t.Name.Content == "START");
            var newObj = new UndertaleRoom.GameObject()
            {
                ObjectDefinition = engine,
                InstanceID = Data.GeneralInfo.LastObj++
            };

            start.GameObjects.Add(newObj);
        }
        public static void AddExtension(UndertaleExtensionFile file)
        {
            var ext = Data.Extensions.First(t => t.Name.Content == "display_mouse_lock");
            ext.Files.Add(file);
        }
    }
}
