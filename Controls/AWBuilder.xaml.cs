using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ModShardLauncher.Controls
{
    /// <summary>
    /// AWBuilder.xaml
    /// </summary>
    public partial class AWBuilder : UserControl
    {
        public AWBuilder()
        {
            InitializeComponent();
        }
        public List<object> List { get; set; } = new List<object>();
        public GeneralPage GeneralPage = new();
    }
}
