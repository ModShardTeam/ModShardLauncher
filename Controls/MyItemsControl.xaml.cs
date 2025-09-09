using ModShardLauncher.Core.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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
