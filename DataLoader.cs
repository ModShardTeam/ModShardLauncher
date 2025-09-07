using UndertaleModLib;
using System.Threading.Tasks;
using System.IO;
using System;
using UndertaleModLib.Util;
using System.Linq;
using Microsoft.Win32;
using System.Collections.Generic;
using Serilog;
using ModShardLauncher.Loader;

namespace ModShardLauncher
{
    public class DataLoader
    {
        public static UndertaleData data = new();
        internal static string dataPath = "";
        internal static string savedDataPath = "";
        public static void LogUTMTWarnings(string warning)
        {
            Log.Warning("[UTMT WARNING]: {0}", warning);
        }
        public static void LogUTMTMessages(string message)
        {
            return;  //Log.Information("[UTMT MESSAGE]: {0}", message);
        }
        public static void Reset()
        {
            data = new();
            dataPath = "";
            savedDataPath = "";
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
                try
                {
                    await LoadFile(dlg.FileName);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Something went wrong while loading {0}.", dlg.FileName);
                    Reset();
                }
            }

            // nothing was load
            return false;
        }
        private static void LoadUmt(string filename)
        {
            using FileStream stream = new(filename, FileMode.Open, FileAccess.Read);
            if (!ChecksumChecker.CompareChecksum(stream))
            {
                Log.Warning("Checksum inconsistency, {{{0}}} may not be steam vanilla from the modbranch beta branch or is a new version.", filename);
            }
            data = UndertaleIO.Read(stream, LogUTMTWarnings, LogUTMTMessages);
        }
        public static async Task LoadFile(string filename)
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
                LoadUmt(filename);
                Main.Instance.Dispatcher.Invoke(() =>
                {
                    dialog.Hide();
                });
            });
            // run
            dialog.ShowDialog();
            await taskLoadDataWinWithUmt;
            
            if (data.IsYYC())
            {
                throw new InvalidDataException(string.Format(
                    "{{{0}}} was made with YYC (YoYo Compiler) which is unmodable currently. You should move to the beta branch called modbranch instead.",
                    filename
                ));
            }

            Log.Information(string.Format("Successfully load: {0}.", filename));

            ModLoader.Initalize();
            // cleaning loot table
            LootUtils.ResetLootTables();
            data.Export(DataType.Items);
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
                UndertaleIO.Write(stream, data, LogUTMTMessages);
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
                    IEnumerable<IUndertaleListChunk> enumerableChunks = data.FORM.Chunks.Values.Where(x => x is not null).Select(x => (IUndertaleListChunk)x);
                    Parallel.ForEach(enumerableChunks, (chunk) => chunk.ClearIndexDict());
                    UndertaleIO.IsDictionaryCleared = true;
                }
                catch { }
            }

            Main.Instance.Dispatcher.Invoke(() =>
            {
                Log.Error("An error occured while trying to save:\n" + exception.Message, "Save error");
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
                        Log.Error("An error occured while trying to save:\n" + exc.Message, "Save error");
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