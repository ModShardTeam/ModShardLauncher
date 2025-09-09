using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ModShardLauncher.Core.Models;

namespace ModShardLauncher.Controls
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
    }
}
