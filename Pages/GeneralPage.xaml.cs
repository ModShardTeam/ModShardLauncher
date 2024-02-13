using ModShardLauncher.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using UndertaleModTool;

namespace ModShardLauncher.Pages
{
    /// <summary>
    /// GeneralPage.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralPage : UserControl
    {
        public GeneralPage()
        {
            InitializeComponent();

            // add Languages
            Languages.Add("中文");
            Languages.Add("English");
            //Languages.Add("Русский");

            switch (Main.Settings.Language)
            {
                case "Chinese":
                    LangSelector.SelectedIndex = 0;
                    UserSettings.ChangeLanguage(0);
                    break;
                case "English":
                    LangSelector.SelectedIndex = 1;
                    UserSettings.ChangeLanguage(1);
                    break;
                case "Russian":
                    LangSelector.SelectedIndex = 2;
                    UserSettings.ChangeLanguage(2);
                    break;
            }
        }
        public int selectIndex { get; set; } = 1;
        public List<string> Languages { get; set; } = new List<string>();
        public int selection = -1;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selection == -1)
            {
                selection = LangSelector.SelectedIndex;
                return;
            }
            ComboBox combo = Msl.ThrowIfNull(sender as ComboBox);
            UserSettings.ChangeLanguage(combo.SelectedIndex);
            selection = combo.SelectedIndex;
            LangSelector.SelectedIndex = selection;
        }

        private void Logger_Checked(object sender, RoutedEventArgs e)
        {
            Main.Settings.EnableLogger = Msl.ThrowIfNull(Logger.IsChecked);
            UserSettings.CheckLog(Main.Settings.EnableLogger);
            Main.Settings.SaveSettings();
        }
    }
}
