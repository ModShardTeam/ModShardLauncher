using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModShardLauncher.Mods
{
    /// <summary>
    /// SourceBar.xaml 的交互逻辑
    /// </summary>
    public partial class SourceBar : UserControl
    {
        public SourceBar()
        {
            InitializeComponent();
        }

        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FilePacker.Pack((DataContext as ModSource).Path);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }
            
            (Main.Instance.Viewer.Content as UserControl).UpdateLayout();
            Main.Instance.Refresh();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", (DataContext as ModSource).Path);
            
        }
    }
}
