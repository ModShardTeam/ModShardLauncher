using ModShardLauncher.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ModShardLauncher.Pages
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }
        public List<object> List { get; set; } = new List<object>();
        public GeneralPage GeneralPage = new GeneralPage();

        private void GeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = Msl.ThrowIfNull(((ToggleButton)sender).IsChecked);
            if (isChecked) Viewer.Content = GeneralPage;
            else Viewer.Content = null;
        }
    }
}
