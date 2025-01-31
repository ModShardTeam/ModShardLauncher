﻿using UndertaleModLib;
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
using System.Security.Cryptography;

namespace ModShardLauncher
{
    public class DataLoader
    {
        public static UndertaleData data = new();
        internal static string dataPath = "";
        internal static string savedDataPath = "";
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
        public static async Task<bool> DoOpenDialog()
        {
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
        /// <summary>
        /// Export all items, weapons and armors in csv files.
        /// </summary>
        private static void ExportItems()
        {
            try
            {
                DirectoryInfo dir = new("export");
                if (dir.Exists) dir.Delete(true);
                dir.Create();

                List<string>? weapons = ModLoader.GetTable("gml_GlobalScript_table_weapons");
                List<string>? armor = ModLoader.GetTable("gml_GlobalScript_table_armor");

                File.WriteAllLines(
                    Path.Join(dir.FullName, Path.DirectorySeparatorChar.ToString(), "_all_items.csv"), 
                    data.GameObjects.Select(t => t.Name.Content).Where(x => x.Contains("o_inv_")).Select(x => x.Replace("o_inv_", ""))
                );

                if (weapons != null)
                {
                    File.WriteAllLines(
                        Path.Join(dir.FullName, Path.DirectorySeparatorChar.ToString(), "_all_weapons.csv"), 
                        weapons.Select(x => string.Join(';', x.Split(';').Take(4)))
                    );
                }
                
                if (armor != null)
                {
                    File.WriteAllLines(
                        Path.Join(dir.FullName, Path.DirectorySeparatorChar.ToString(), "_all_armors.csv"), 
                        armor.Select(x => string.Join(';', x.Split(';').Take(6)))
                    );
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        private static void ExportPreset()
        {
            GlobalDecompileContext context = new(ModLoader.Data, false);
            File.WriteAllText("json_preset_bastion.json", Decompiler.Decompile(data.Code.First(t => t.Name.Content.Contains("scr_preset_bastion_1")), context));
            File.WriteAllText("json_preset_catacombs.json", Decompiler.Decompile(data.Code.First(t => t.Name.Content.Contains("scr_preset_catacombs")), context));
            File.WriteAllText("json_preset_crypt.json", Decompiler.Decompile(data.Code.First(t => t.Name.Content.Contains("scr_preset_crypt_1")), context));
        }
        /// <summary>
        /// Compute the MD5 checksum of a file located in a FileStream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static string ComputeChecksum(FileStream stream)
        {
            using var md5 = MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(stream));
        }
        /// <summary>
        /// Return True if the MD5 checksum of a file is equal either to the MD5 checksum of the GOG data.win of Stoneshard or to the MD5 checksum of the STEAM data.win of Stoneshard.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static bool CompareChecksum(FileStream stream)
        {
            string hash = ComputeChecksum(stream);
            // Log.Information(hash); // uncomment to log the checksum of new versions and add to the array
            string[] checksums =
            {
                "6E37E076EDFDC25468195EC1FFA937A5", // GOG 0.8.2.10
                "392EE0E8C6A09A16DED58C5737ECF1B5", // Steam 0.8.2.10
                "5F91989CA7E2A2B1234B2CD2A6AF9821", // Steam 0.9.1.16-vm
                "2BD331F728428746FA337D6C7B67040A", // Steam 0.9.1.17-vm
                "6F9F1E29275EEF60E3A725ECA1033DF8" // Steam 0.9.1.18-vm
            };
            return checksums.Contains(hash);
        }
        private static bool LoadUmt(string filename)
        {
            bool hadWarnings = false;
            using (FileStream stream = new(filename, FileMode.Open, FileAccess.Read))
            {
                if(!CompareChecksum(stream))
                {
                    Log.Warning("Checksum inconsistency, {{{0}}} may not be vanilla or is a new version.", filename);
                }
                data = UndertaleIO.Read(
                    stream, warning =>
                    {
                        ShowWarning(warning, "Loading warning");

                        if (warning.Contains("unserializeCountError.txt") || warning.Contains("object pool size"))
                            return;

                        hadWarnings = true;
                    }, 
                    delegate (string message)
                    {
                        FileMessageEvent?.Invoke(message);
                    }
                );
            }

            //UndertaleEmbeddedTexture.TexData.ClearSharedStream();
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
            LootUtils.ResetLootTables();
            ExportItems();
        }
        public static async Task<bool> DoSaveDialog()
        {   
            SaveFileDialog dlg = new()
            {
                DefaultExt = "win",
                Filter = "Game Maker Studio data files (.win, .unx, .ios, .droid, audiogroup*.dat)|*.win;*.unx;*.ios;*.droid;audiogroup*.dat|All files|*",
                FileName = "data.win"
            };

            if (dlg.ShowDialog() == true)
            {
                savedDataPath = dlg.FileName;
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

            //UndertaleEmbeddedTexture.TexData.ClearSharedStream();
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
        }
    }
}