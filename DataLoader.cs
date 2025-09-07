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
                    // save the filename for later
                    dataPath = dlg.FileName;
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

            Log.Information("Successfully load: {0}.", filename);

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
                try
                {
                    await SaveFile(dlg.FileName);
                    return true;
                }
                catch (AggregateException exs)
                {
                    Log.Error("Multiple exceptions occurred during save operation for {{{0}}:", dlg.FileName);
                    foreach (Exception ex in exs.InnerExceptions)
                    {
                        Log.Error(ex, "Exception: {Message}", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An exception occurred during save operation for {{{0}}:", dlg.FileName);
                }
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
        private static void HandleFailedSave()
        {
            if (!UndertaleIO.IsDictionaryCleared)
            {
                IEnumerable<IUndertaleListChunk> enumerableChunks = data.FORM.Chunks.Values.Where(x => x is not null).Select(x => (IUndertaleListChunk)x);
                Parallel.ForEach(enumerableChunks, (chunk) => chunk.ClearIndexDict());
                UndertaleIO.IsDictionaryCleared = true;
            }
        }
        public static async Task SaveFile(string filename)
        {
            // create a new dialog
            LoadingDialog dialog = new()
            {
                Owner = Main.Instance
            };

            Task taskSaveDataWinWithUmt = Task.Run(() =>
            {
                List<Exception> exceptions = new();
                bool SaveSucceeded = true;
                // try temp save first
                try
                {
                    SaveTempWithUmt(filename);
                }
                catch (Exception savedException)
                {
                    exceptions.Add(savedException);
                    try
                    {
                        HandleFailedSave();
                    }
                    catch (Exception handledException)
                    {
                        exceptions.Add(handledException);
                    }
                    SaveSucceeded = false;
                }

                // clean after saving
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
                catch (Exception cleanUpException)
                {
                    exceptions.Add(cleanUpException);
                    SaveSucceeded = false;
                }

                Main.Instance.Dispatcher.Invoke(() =>
                {
                    dialog.Hide();
                });

                if (!SaveSucceeded) throw new AggregateException(exceptions);
            });

            //run
            dialog.ShowDialog();
            await taskSaveDataWinWithUmt;
        }
    }
}