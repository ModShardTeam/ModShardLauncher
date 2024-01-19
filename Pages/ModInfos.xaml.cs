using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
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
using Serilog;

namespace ModShardLauncher.Pages
{
    /// <summary>
    /// ModInfos.xaml 的交互逻辑
    /// </summary>
    public partial class ModInfos : UserControl
    {
        public ModInfos()
        {
            InitializeComponent();
            Instance = this;
        }
        public static ModInfos Instance;
        public List<ModFile> Mods { get; set; } = new List<ModFile>();

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

            try 
            {
                Task taskPatch = Task.Run(() => ModLoader.PatchFile());
                Task<bool> taskSave = DataLoader.DoSaveDialog();
                await taskPatch;
                await taskSave;

                Log.Information("Successfully patch vanilla");
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
                Log.Information("Failed patching vanilla");
            }

            Main.Instance.Refresh();
        }
    }
}
