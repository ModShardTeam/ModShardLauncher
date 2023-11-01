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

namespace ModShardLauncher.Pages
{
    /// <summary>
    /// ModSourceInfos.xaml 的交互逻辑
    /// </summary>
    public partial class ModSourceInfos : UserControl
    {
        public ModSourceInfos()
        {
            InitializeComponent();
            Instance = this;
        }
        public static ModSourceInfos Instance;
        public List<ModSource> ModSources {  get; set; } = new List<ModSource>();
        private async void Open_Click(object sender, EventArgs e)
        {
            await DataLoader.DoOpenDialog();
            Main.Instance.Refresh();
        }
        private async void Save_Click(object sender, EventArgs e)
        {
            if (DataLoader.data == null)
            {
                MessageBox.Show(Application.Current.FindResource("LoadDataWarning").ToString());
                return;
            }
            ModLoader.PatchFile();
            DataLoader.DoSaveDialog();
            Main.Instance.Refresh();
        }
    }
}
