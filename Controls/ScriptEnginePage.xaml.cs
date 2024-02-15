using ModShardLauncher.Mods;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModShardLauncher.Controls
{
    /// <summary>
    /// ScriptEnginePage.xaml 的交互逻辑
    /// </summary>
    public partial class ScriptEnginePage : Window
    {
        public static ScriptEnginePage? Instance;

        public DispatcherTimer Timer;

        public ScriptEnginePage()
        {
            InitializeComponent();
            Instance = this;
            Timer = new DispatcherTimer();
        }

        private void ScriptBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ScriptBox.Text.Length == 0) return;
                ModInterfaceServer.SendScript(ScriptBox.Text);
                ScriptBox.Text = "";
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ModInterfaceServer.Server.Close();
        }
    }
}
