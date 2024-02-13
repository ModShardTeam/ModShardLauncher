using ModShardLauncher.Controls;
using ModShardLauncher.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using UndertaleModLib;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Runtime.InteropServices;

namespace ModShardLauncher
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        public static Main Instance;
        public MainPage MainPage;
        public ModInfos ModPage;
        public ModSourceInfos ModSourcePage;
        public Settings SettingsPage;
        public static UserSettings Settings = new();
        public static LoggingLevelSwitch lls = new();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public static IntPtr handle;

        public Main()
        {
            handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            Instance = this;
            MainPage = new MainPage();
            ModPage = new ModInfos();
            UserSettings.LoadSettings();
            ModSourcePage = new ModSourceInfos();
            if (!Directory.Exists(ModLoader.ModPath))
                Directory.CreateDirectory(ModLoader.ModPath);
            if (!Directory.Exists(ModLoader.ModSourcesPath))
                Directory.CreateDirectory(ModLoader.ModSourcesPath);

            // create File and Console (controlledby a switch) sinks
            LoggerConfiguration logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(string.Format("logs/log_{0}.txt", DateTime.Now.ToString("yyyyMMdd_HHmm")))
                .WriteTo.Logger(log => log
                    .MinimumLevel.ControlledBy(lls)
                    .WriteTo.Console()
                );

            Log.Logger = logger.CreateLogger();

            try
            {
                ModLoader.LoadFiles();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }
            
            SettingsPage = new Settings();
            InitializeComponent();

            Viewer.Content = MainPage;
        }

        private void MyToggleButton_Checked(object sender, EventArgs e)
        {
            foreach(var i in stackPanel.Children)
            {
                if(i != sender && i is MyToggleButton button)
                {
                    button.MyButton.IsChecked = false;
                }
            }
        }
        public void Refresh()
        {
            ModPage = new ModInfos();
            ModSourcePage = new ModSourceInfos();
            Settings settingCache = new();
            settingCache.Viewer.Content = SettingsPage.Viewer.Content;
            SettingsPage = settingCache;
            try
            {
                ModLoader.LoadFiles();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }
            if (Viewer.Content is ModInfos) Viewer.Content = ModPage;
            else if (Viewer.Content is ModSourceInfos) Viewer.Content = ModSourcePage;
            else if (Viewer.Content is Settings) Viewer.Content = SettingsPage;
            else Viewer.Content = MainPage;
        }

        private void MyToggleButton_Click(object sender, EventArgs e)
        {
            Log.CloseAndFlushAsync();
            Close();
        }

        private void MyToggleButton_Click_1(object sender, EventArgs e)
        {
            if (sender is MyToggleButton button && Msl.ThrowIfNull(button.MyButton.IsChecked)) Viewer.Content = ModPage;
            else Viewer.Content = MainPage;
        }

        private void MyToggleButton_Click_2(object sender, EventArgs e)
        {
            if (sender is MyToggleButton button && Msl.ThrowIfNull(button.MyButton.IsChecked)) Viewer.Content = ModSourcePage;
            else Viewer.Content = MainPage;
        }
        private void MyToggleButton_Click_4(object sender, EventArgs e)
        {
            if (sender is MyToggleButton button && Msl.ThrowIfNull(button.MyButton.IsChecked)) Viewer.Content = SettingsPage;
            else Viewer.Content = MainPage;
        }
        private void MyToggleButton_Click_3(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
            if (sender is MyToggleButton button) button.MyButton.IsChecked = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.SaveSettings();
        }
    }
    public class UserSettings
    {
        public string Language = "English";
        public bool EnableLogger = true;
        public string SavePos = "";
        public string LoadPos = "";
        public List<string> EnableMods = new();
        public static void LoadSettings()
        {
            // if no settings file, early stop
            if (!File.Exists("Settings.json")) return;

            // read file
            string settings = File.ReadAllText("Settings.json");
            // convert if as UserSettings
            Main.Settings = Msl.ThrowIfNull(JsonConvert.DeserializeObject<UserSettings>(settings));

            CheckLog(Main.Settings.EnableLogger);

            // auto load if loadpos not empty
            if (Main.Settings.LoadPos != "")
                Main.Instance.Dispatcher.Invoke(async () => await DataLoader.LoadFile(Main.Settings.LoadPos));

            // auto check active mods
            if (Main.Settings.EnableMods.Count > 0)
            {
                List<ModFile> listModFile = ModInfos.Instance.Mods;
                foreach (string i in Main.Settings.EnableMods)
                {
                    (int indexMod, ModFile? modFile) = listModFile.Enumerate().FirstOrDefault(t => t.Item2.Name == i);
                    if (modFile != null)
                        listModFile[indexMod].isEnabled = true;
                    else
                        Log.Warning($"Mod {i} not found");
                }
            }
        }
        public static void CheckLog(bool isLogConsole)
        {
            if (isLogConsole)
            {
                Main.ShowWindow(Main.handle, Main.SW_SHOW);
                Main.lls.MinimumLevel = LogEventLevel.Information;
            }
            else
            {
                Main.ShowWindow(Main.handle, Main.SW_HIDE);
                Main.lls.MinimumLevel = (LogEventLevel) 1 + (int) LogEventLevel.Fatal;
            }
        }
        public static void ChangeLanguage(int index)
        {
            ResourceDictionary resDict;
            switch (index)
            {
                case 0:
                    resDict = Application.Current.Resources.MergedDictionaries.First(t => t.Source.OriginalString == @"Language/zh-cn.xaml");
                    Application.Current.Resources.MergedDictionaries.Remove(resDict);
                    Application.Current.Resources.MergedDictionaries.Add(resDict);
                    Main.Settings.Language = "Chinese";
                    break;
                case 1:
                    resDict = Application.Current.Resources.MergedDictionaries.First(t => t.Source.OriginalString == @"Language/en-us.xaml");
                    Application.Current.Resources.MergedDictionaries.Remove(resDict);
                    Application.Current.Resources.MergedDictionaries.Add(resDict);
                    Main.Settings.Language = "English";
                    break;
                case 2:
                    resDict = Application.Current.Resources.MergedDictionaries.First(t => t.Source.OriginalString == @"Language/ru-ru.xaml");
                    Application.Current.Resources.MergedDictionaries.Remove(resDict);
                    Application.Current.Resources.MergedDictionaries.Add(resDict);
                    Main.Settings.Language = "Русский";
                    break;
            }
        }
        public void SaveSettings()
        {
            File.WriteAllText("Settings.json", JsonConvert.SerializeObject(this));
        }
    }
}
