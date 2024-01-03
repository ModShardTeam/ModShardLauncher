using ModShardLauncher.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ModShardLauncher.Pages
{
    /// <summary>
    /// ScriptEnginePage.xaml 的交互逻辑
    /// </summary>
    public partial class ScriptEnginePage : Window
    {
        public static ScriptEnginePage Instance;

        public DispatcherTimer Timer;

        public ScriptEnginePage()
        {
            InitializeComponent();
            Instance = this;
            Timer = new DispatcherTimer();
        }

        private void ScriptBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (ScriptBox.Text.Length == 0) return;
                ModInterfaceServer.SendScript(ScriptBox.Text);
                ScriptBox.Text = "";
            }
        }
    }
}
