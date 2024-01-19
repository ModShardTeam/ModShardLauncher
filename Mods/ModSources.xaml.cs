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
using UndertaleModLib;
using Serilog;

namespace ModShardLauncher.Mods
{
    /// <summary>
    /// ModSources.xaml 的交互逻辑
    /// </summary>
    public partial class ModSources : UserControl
    {
        public ModSources()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(DataLoader.data == null)
            {
                MessageBox.Show(Application.Current.FindResource("LoadDataWarning").ToString());
                return;
            }
            try
            {
                FilePacker.Pack((DataContext as ModSource).Path);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }

            ModLoader.LoadFiles();
        }
    }
}
