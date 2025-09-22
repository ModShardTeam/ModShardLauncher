using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ModShardLauncher.Core.Errors;
using ModShardLauncher.Core.UI;
using ModShardLauncher.Core.Models;
using Serilog;

namespace ModShardLauncher.Controls
{
    /// <summary>
    /// ModInfos.xaml 的交互逻辑
    /// </summary>
    public partial class ModInfos : UserControl, INotifyPropertyChanged
    {
        public static ModInfos Instance;
        public List<ModFile> Mods { get; set; } = new();
        public event PropertyChangedEventHandler? PropertyChanged;
        private bool _selectAll;
        public bool SelectAll
        {
            get { return _selectAll; }
            set
            {
                if (_selectAll != value)
                {
                    _selectAll = value;
                    OnPropertyRaised(nameof(SelectAll));
                    SelectAllMods();
                }
            }
        }
        public ModInfos()
        {
            InitializeComponent();
            this.DataContext = this;
            Instance = this;
        }
        private async void Open_Click(object sender, EventArgs e)
        {
            await DataLoader.DoOpenDialog();
            Main.Instance.Refresh();
        }
        private async void Save_Click(object sender, EventArgs e)
        {
            if (DataLoader.data.FORM == null)
            {
                MessageBox.Show(Application.Current.FindResource("LoadDataWarning").ToString());
                return;
            }

            MSLDiagnostic? diag = ModLoader.PatchFile();

            if (diag is null)
            {
                Main.Instance.LogModListStatus();
                Log.Information("Successfully patch vanilla");

                Task<bool> save = DataLoader.DoSaveDialog();
                await save;
                if (save.Result)
                {
                    LootUtils.SaveLootTables(Msl.ThrowIfNull(Path.GetDirectoryName(DataLoader.savedDataPath)));
                }
                else
                {
                    Log.Information("Saved cancelled.");
                }
            }
            else
            {
                Main.Instance.LogModListStatus();
                Log.Information("Failed patching vanilla");
                ErrorMessageDialog.Show(diag.Title(), diag.MessageDialog(), Main.Instance.logPath);
            }

            // reload the data
            await DataLoader.LoadFile(DataLoader.dataPath);
            Main.Instance.Refresh();
        }
        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
        private void SelectAllMods()
        {
            foreach (ModFile mod in Mods)
            {
                mod.Enabled = true;
            }
        }
    }
}
