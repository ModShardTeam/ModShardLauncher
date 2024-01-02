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

namespace ModShardLauncher
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        public static Main Instance;
        public UndertaleData Data => DataLoader.data;
        public MainPage MainPage;
        public ModInfos ModPage;
        public ModSourceInfos ModSourcePage;
        public Settings SettingsPage;
        public static UserSettings Settings = new UserSettings();
        public Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/msl_log.txt")
                .CreateLogger();

            Instance = this;
            MainPage = new MainPage();
            ModPage = new ModInfos();
            ModSourcePage = new ModSourceInfos();
            if (!Directory.Exists(ModLoader.ModPath))
                Directory.CreateDirectory(ModLoader.ModPath);
            if (!Directory.Exists(ModLoader.ModSourcesPath))
                Directory.CreateDirectory(ModLoader.ModSourcesPath);
            ModLoader.LoadFiles();
            Settings.LoadSettings();
            SettingsPage = new Settings();
            InitializeComponent();

            Viewer.Content = MainPage;

            
            //Viewer.Content = new DescriptionView(Application.Current.FindResource("Welcome_Use").ToString(),
            //Application.Current.FindResource("Welcome_Desc").ToString());
        }

        private void MyToggleButton_Checked(object sender, EventArgs e)
        {
            foreach(var i in stackPanel.Children)
            {
                if(i != sender && i is MyToggleButton)
                {
                    (i as MyToggleButton).MyButton.IsChecked = false;
                }
            }
        }
        public void Refresh()
        {
            ModPage = new ModInfos();
            ModSourcePage = new ModSourceInfos();
            var settingCache = new Settings();
            settingCache.Viewer.Content = SettingsPage.Viewer.Content;
            SettingsPage = settingCache;
            ModLoader.LoadFiles();
            if (Viewer.Content is ModInfos) Viewer.Content = ModPage;
            else if (Viewer.Content is ModSourceInfos) Viewer.Content = ModSourcePage;
            else if (Viewer.Content is Settings) Viewer.Content = SettingsPage;
            else Viewer.Content = MainPage;
        }

        private void MyToggleButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MyToggleButton_Click_1(object sender, EventArgs e)
        {
            if ((bool)(sender as MyToggleButton).MyButton.IsChecked) Viewer.Content = ModPage;
            else Viewer.Content = MainPage;
        }

        private void MyToggleButton_Click_2(object sender, EventArgs e)
        {
            if ((bool)(sender as MyToggleButton).MyButton.IsChecked) Viewer.Content = ModSourcePage;
            else Viewer.Content = MainPage;
        }
        private void MyToggleButton_Click_4(object sender, EventArgs e)
        {
            if ((bool)(sender as MyToggleButton).MyButton.IsChecked) Viewer.Content = SettingsPage;
            else Viewer.Content = MainPage;
        }
        private void MyToggleButton_Click_3(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
            (sender as MyToggleButton).MyButton.IsChecked = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.SaveSettings();
        }
    }
    public class UserSettings
    {
        public string Language = "English";
        public string SavePos = "";
        public string LoadPos = "";
        public List<string> EnableMods = new List<string>();
        public void LoadSettings()
        {
            if (!File.Exists("Settings.json")) return;
            var settings = File.ReadAllText("Settings.json");
            Main.Settings = JsonConvert.DeserializeObject<UserSettings>(settings);
            if (Main.Settings.LoadPos != "")
                Main.Instance.Dispatcher.Invoke(async () => await DataLoader.LoadFile(Main.Settings.LoadPos));
            if (Main.Settings.EnableMods.Count > 0)
            {
                var list = ModInfos.Instance.Mods;
                foreach (var i in Main.Settings.EnableMods)
                {
                    var tar = list.First(t => t.Name == i);
                    list[list.IndexOf(tar)].isEnabled = true;
                }
            }
        }
        public void ChangeLanguage(int index)
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
