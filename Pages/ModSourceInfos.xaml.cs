using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace ModShardLauncher.Pages
{
    /// <summary>
    /// ModSourceInfos.xaml 的交互逻辑
    /// </summary>
    public partial class ModSourceInfos : UserControl
    {
        public static ModSourceInfos Instance;
        public List<ModSource> ModSources {  get; set; } = new();
        public ModSourceInfos()
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

            try
            {
                ModLoader.LoadFiles();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }

            Main.Instance.Refresh();
        }
    }
}
