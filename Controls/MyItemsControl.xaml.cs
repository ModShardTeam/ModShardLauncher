using ModShardLauncher.Mods;
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

namespace ModShardLauncher.Controls
{
    /// <summary>
    /// MyScrollViewer.xaml 的交互逻辑
    /// </summary>
    public partial class MyItemsControl : UserControl
    {
        public MyItemsControl()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(object),
            typeof(MyItemsControl),
            new PropertyMetadata(default(object), OnItemsPropertyChanged));
        private static void OnItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        public List<object> ItemsSource { get; set; }
    }
    public class TempSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ModFile)
                return Application.Current.FindResource("mod") as DataTemplate;
            else return Application.Current.FindResource("source") as DataTemplate;
        }
    }
}
