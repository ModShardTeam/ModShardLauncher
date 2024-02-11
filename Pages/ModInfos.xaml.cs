using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace ModShardLauncher.Pages
{
    /// <summary>
    /// ModInfos.xaml 的交互逻辑
    /// </summary>
    public partial class ModInfos : UserControl
    {
        public static ModInfos Instance;
        public List<ModFile> Mods { get; set; } = new();
        public ModInfos()
        {
            InitializeComponent();
            Instance = this;
        }
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
                ModLoader.PatchFile();
                Log.Information("Successfully patch vanilla");
                await DataLoader.DoSaveDialog();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
                Log.Information("Failed patching vanilla");
                MessageBox.Show(Application.Current.FindResource("SaveDataWarning").ToString());
            }
            
            Main.Instance.Refresh();
        }
    }
}
