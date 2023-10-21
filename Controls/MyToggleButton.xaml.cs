using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ModShardLauncher.Controls
{
    /// <summary>
    /// MyToggleButton.xaml 的交互逻辑
    /// </summary>
    public partial class MyToggleButton : UserControl
    {
        public MyToggleButton()
        {
            InitializeComponent();
        }
        public ImageSource ImageSource { get; set; }
        public string Text { get; set; }
        [Category("Behavior")]
        public event EventHandler Checked;
        private void MyButton_Checked(object sender, RoutedEventArgs e)
        {
            if(Checked != null) Checked.Invoke(this, e);
        }
    }
}
