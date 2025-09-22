using ModShardLauncher.Core.Models;
using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ModShardLauncher.Controls
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
                UtilsPacker.Pack(Msl.ThrowIfNull(DataContext as ModSource).Path);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }
            
            Msl.ThrowIfNull((UserControl)Main.Instance.Viewer.Content).UpdateLayout();
            Main.Instance.Refresh();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Msl.ThrowIfNull(DataContext as ModSource).Path);
            
        }
    }
}
