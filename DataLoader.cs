using UndertaleModLib;
using UndertaleModLib.Models;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Windows;
using UndertaleModLib.Util;
using System.Linq;
using Microsoft.Win32;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog;
using UndertaleModLib.Decompiler;

namespace ModShardLauncher
{
    public class DataLoader
    {
        public static UndertaleData data = new();
        internal static string dataPath = "";
        public delegate void FileMessageEventHandler(string message);
        public static event FileMessageEventHandler FileMessageEvent;
        public static void ShowWarning(string warning, string title)
        {
            Console.WriteLine(title + ":" + warning);
        }
        public static void ShowError(string error, string title)
        {
            Console.WriteLine(title + ":" + error);
        }
        public static string GetVersion()
        {
            string version = data.Strings.First(t => t.Content.EndsWith(" Build date: ")).Content;
            // version is spelled Verison in the code base
            // so dont touch this part even if it feels like a mistake
            List<string> sp = version.Replace(" Build date: ", "").Replace("Verison: ", "").Split(".").ToList();
            List<string> sp2 = new();
            sp.ForEach(i =>
            {
                if (i.Length < 2) sp2.Add(0 + i);
                else sp2.Add(i);
            });
            version = "v" + string.Join(".", sp2);
            return version;
        }
        public static async Task<bool> DoOpenDialog()
        {
            // auto load if it can
            if(Main.Settings.LoadPos != "" && File.Exists(Main.Settings.LoadPos))
            {
                await LoadFile(Main.Settings.LoadPos);
                return true;
            }

            // else open a new dialog
            OpenFileDialog dlg = new()
            {
                DefaultExt = "win",
                Filter = "Game Maker Studio data files (.win, .unx, .ios, .droid, audiogroup*.dat)|*.win;*.unx;*.ios;*.droid;audiogroup*.dat|All files|*"
            };

            // load
            if (dlg.ShowDialog() == true)
            {
                await LoadFile(dlg.FileName);
                return true;
            }

            // nothing was load
            return false;
        }
        private static void ExportData()
        {
            File.WriteAllText("json_dump_code.json", JsonConvert.SerializeObject(data.Code.Select(t => t.Name.Content)));
            File.WriteAllText("json_dump_variables.json", JsonConvert.SerializeObject(data.Variables.Select(t => t.Name.Content)));
            File.WriteAllText("json_dump_rooms.json", JsonConvert.SerializeObject(data.Rooms.Select(t => t.Name.Content)));
            Msl.GenerateNRandomLinesFromCode(data.Code, new GlobalDecompileContext(data, false), 100, 1, 0);
        }
        private static void ExportPreset()
        {
            GlobalDecompileContext context = new(ModLoader.Data, false);
            File.WriteAllText("json_preset_bastion.json", Decompiler.Decompile(data.Code.First(t => t.Name.Content.Contains("scr_preset_bastion_1")), context));
            File.WriteAllText("json_preset_catacombs.json", Decompiler.Decompile(data.Code.First(t => t.Name.Content.Contains("scr_preset_catacombs")), context));
            File.WriteAllText("json_preset_crypt.json", Decompiler.Decompile(data.Code.First(t => t.Name.Content.Contains("scr_preset_crypt_1")), context));
        }
        private static bool LoadUmt(string filename)
        {
            bool hadWarnings = false;
            using (FileStream stream = new(filename, FileMode.Open, FileAccess.Read))
            {
                data = UndertaleIO.Read(
                    stream, warning =>
                    {
                        ShowWarning(warning, "Loading warning");

                        if (warning.Contains("unserializeCountError.txt")
                            || warning.Contains("object pool size"))
                            return;

                        hadWarnings = true;
                    }, 
                    delegate (string message)
                    {
                        FileMessageEvent?.Invoke(message);
                    }
                );
            }

            UndertaleEmbeddedTexture.TexData.ClearSharedStream();
            Log.Information(string.Format("Successfully load: {0}.", filename));

            return hadWarnings;
        }
        public static async Task LoadFile(string filename, bool re = false)
        {
            // save the filename for later
            dataPath = filename;
            // create a new dialog box
            LoadingDialog dialog = new()
            {
                Owner = Main.Instance
            };
            // task load a data.win with umt
            Task taskLoadDataWinWithUmt = Task.Run(() =>
            {
                bool hadWarnings = false;
                try
                {
                    hadWarnings = LoadUmt(filename);
                }
                catch (Exception ex)
                {   
                    Log.Error(ex, "Something went wrong");
                    throw;
                }
                Main.Instance.Dispatcher.Invoke(() =>
                {
                    dialog.Hide();
                });
            });
            // run
            dialog.ShowDialog();
            await taskLoadDataWinWithUmt;
            ModLoader.Initalize();
            // cleaning loot table
            LootUtils.LootTable.Clear();
            
            if(Main.Settings.LoadPos == "" && !re)
            {
                MessageBoxResult result = MessageBox.Show(
                    Application.Current.FindResource("LoadPath").ToString(),
                    Application.Current.FindResource("LoadPath").ToString(), 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    Main.Settings.LoadPos = filename;
                }
            }
        }
        public static async Task<bool> DoSaveDialog()
        {
            if (Main.Settings.SavePos != "")
            {
                await SaveFile(Main.Settings.SavePos);
                return true;
            }
            
            SaveFileDialog dlg = new()
            {
                DefaultExt = "win",
                Filter = "Game Maker Studio data files (.win, .unx, .ios, .droid, audiogroup*.dat)|*.win;*.unx;*.ios;*.droid;audiogroup*.dat|All files|*",
                FileName = "data.win"
            };

            if (dlg.ShowDialog() == true)
            {
                await SaveFile(dlg.FileName);
                return true;
            }

            return false;
        }
        private static void SaveTempWithUmt(string filename)
        {
            using (FileStream stream = new(filename + "temp", FileMode.Create, FileAccess.Write))
            {
                UndertaleIO.Write(stream, data, message =>
                {
                    FileMessageEvent?.Invoke(message);
                });
            }

            UndertaleEmbeddedTexture.TexData.ClearSharedStream();
            QoiConverter.ClearSharedBuffer();
        }
        private static void HandleFailedSave(Exception exception)
        {
            if (!UndertaleIO.IsDictionaryCleared)
            {
                try
                {
                    IEnumerable<IUndertaleListChunk> enumerableChunks = data.FORM.Chunks.Values.Where(x => x is not null).Select((UndertaleChunk x) => x as IUndertaleListChunk);
                    Parallel.ForEach(enumerableChunks, (chunk) =>
                    {
                        chunk.ClearIndexDict();
                    });

                    UndertaleIO.IsDictionaryCleared = true;
                }
                catch { }
            }

            Main.Instance.Dispatcher.Invoke(() =>
            {
                ShowError("An error occured while trying to save:\n" + exception.Message, "Save error");
            });
        }
        public static async Task SaveFile(string filename)
        {
            // create a new dialog
            LoadingDialog dialog = new()
            {
                Owner = Main.Instance
            };

            Task t = Task.Run(() =>
            {
                bool SaveSucceeded = true;
                // try temp save first
                try
                {
                    SaveTempWithUmt(filename);
                }
                catch (Exception e)
                {
                    HandleFailedSave(e);
                    SaveSucceeded = false;
                }

                // move save
                try
                {
                    if (SaveSucceeded)
                    {
                        if (File.Exists(filename)) File.Delete(filename);
                        File.Move(filename + "temp", filename);
                    }
                    else
                    {
                        if (File.Exists(filename + "temp")) File.Delete(filename + "temp");
                    }
                }
                catch (Exception exc)
                {
                    Main.Instance.Dispatcher.Invoke(() =>
                    {
                        ShowError("An error occured while trying to save:\n" + exc.Message, "Save error");
                    });

                    SaveSucceeded = false;
                }

                Main.Instance.Dispatcher.Invoke(() =>
                {
                    dialog.Hide();
                });
            });

            //run
            dialog.ShowDialog();
            await t;

            if (Main.Settings.SavePos == "")
            {
                MessageBoxResult result = MessageBox.Show(
                    Application.Current.FindResource("SavePath").ToString(),
                    Application.Current.FindResource("SavePath").ToString(), 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    Main.Settings.SavePos = filename;
                }
            }
        }
    }
}