using UndertaleModLib;
using UndertaleModLib.Models;
using System.Threading.Tasks;
using System.IO;
using System;
using UndertaleModTool;
using System.Windows;
using System.Diagnostics;
using UndertaleModLib.Decompiler;
using ModShardLauncher.Mods;
using System.Text;
using System.Windows.Threading;
using UndertaleModLib.ModelsDebug;
using UndertaleModLib.Util;
using System.Linq;
using Microsoft.Win32;
using System.Windows.Documents;
using System.Collections.Generic;

namespace ModShardLauncher
{
    public class DataLoader
    {
        public static UndertaleData data = null;
        internal static string DataPath = "";
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
            var version = data.Strings.First(t => t.Content.EndsWith(" Build date: ")).Content;
            var sp = version.Replace(" Build date: ", "").Replace("Verison: ", "").Split(".").ToList();
            var sp2 = new List<string>();
            sp.ForEach(i =>
            {
                if (i.Length < 2) sp2.Add(0 + i);
            });
            version = "v" + string.Join(".", sp2);
            return version;
        }
        public static async Task<bool> DoOpenDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.DefaultExt = "win";
            dlg.Filter = "Game Maker Studio data files (.win, .unx, .ios, .droid, audiogroup*.dat)|*.win;*.unx;*.ios;*.droid;audiogroup*.dat|All files|*";

            if (dlg.ShowDialog() == true)
            {
                await LoadFile(dlg.FileName);
                return true;
            }
            return false;
        }
        public static async Task LoadFile(string filename, bool re = false)
        {
            LoadingDialog loader = new LoadingDialog(re ? "重新加载中" :"加载中", "加载中 请稍候...");
            loader.PreventClose = true;
            DataPath = filename;
            Task t = Task.Run(() =>
            {
                bool hadWarnings = false;
                try
                {
                    using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        data = UndertaleIO.Read(stream, warning =>
                        {
                            ShowWarning(warning, "Loading warning");

                            if (warning.Contains("unserializeCountError.txt")
                                || warning.Contains("object pool size"))
                                return;

                            hadWarnings = true;
                        },delegate (string message)
                        {
                            FileMessageEvent?.Invoke(message);
                        });
                    }

                    UndertaleEmbeddedTexture.TexData.ClearSharedStream();
                }
                catch (Exception e)
                {
                    throw e;
                    //this.ShowError("An error occured while trying to load:\n" + e.Message, "Load error");
                }
                MainWindow.Instance.Dispatcher.Invoke(async () =>
                {
                    loader.Hide();
                });
            });

            loader.ShowDialog();
            await t;
            ModLoader.Initalize();
        }
        public static async Task<bool> DoSaveDialog()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = "win";
            dlg.Filter = "Game Maker Studio data files (.win, .unx, .ios, .droid, audiogroup*.dat)|*.win;*.unx;*.ios;*.droid;audiogroup*.dat|All files|*";
            dlg.FileName = "data.win";

            if(dlg.ShowDialog() == true)
            {
                await SaveFile(dlg.FileName);
                return true;
            }
            return false;
        }
        public static async Task SaveFile(string filename)
        {
            LoadingDialog dialog = new LoadingDialog("保存中", "保存中 请稍候...");
            dialog.PreventClose = true;
            IProgress<Tuple<int, string>> progress = new Progress<Tuple<int, string>>(i => { dialog.ReportProgress(i.Item2, i.Item1); });
            IProgress<double?> setMax = new Progress<double?>(i => { dialog.Maximum = i; });
            Task t = Task.Run(async () =>
            {
                bool SaveSucceeded = true;
                try
                {
                    using (var stream = new FileStream(filename + "temp", FileMode.Create, FileAccess.Write))
                    {
                        UndertaleIO.Write(stream, data, message =>
                        {
                            FileMessageEvent?.Invoke(message);
                        });
                    }

                    UndertaleEmbeddedTexture.TexData.ClearSharedStream();
                    QoiConverter.ClearSharedBuffer();
                }
                catch (Exception e)
                {
                    if (!UndertaleIO.IsDictionaryCleared)
                    {
                        try
                        {
                            var listChunks = data.FORM.Chunks.Values.Select(x => x as IUndertaleListChunk);
                            Parallel.ForEach(listChunks.Where(x => x is not null), (chunk) =>
                            {
                                chunk.ClearIndexDict();
                            });

                            UndertaleIO.IsDictionaryCleared = true;
                        }
                        catch { }
                    }
                    MainWindow.Instance.Dispatcher.Invoke(() =>
                    {
                        ShowError("An error occured while trying to save:\n" + e.Message, "Save error");
                    });
                    SaveSucceeded = false;
                }
                try
                {
                    if (SaveSucceeded)
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                        File.Move(filename + "temp", filename);
                    }
                    else
                    {
                        if (File.Exists(filename + "temp"))
                            File.Delete(filename + "temp");
                    }
                }
                catch (Exception exc)
                {
                    MainWindow.Instance.Dispatcher.Invoke(() =>
                    {
                        ShowError("An error occured while trying to save:\n" + exc.Message, "Save error");
                    });

                    SaveSucceeded = false;
                }
                MainWindow.Instance.Dispatcher.Invoke(() =>
                {
                    dialog.Hide();
                });
            });
            dialog.ShowDialog();
            await t;
            ModLoader.LoadFiles();
        }
    }
}