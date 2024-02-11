using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

namespace ModShardLauncher.Mods
{
    /// <summary>
    /// ModBar.xaml 的交互逻辑
    /// </summary>
    public partial class ModBar : UserControl
    {
        public ImageSource? Icon = null;
        public ModBar()
        {
            InitializeComponent();
        }
    }
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] imageData = (byte[])value;
            if (imageData.Length == 0) return new BitmapImage(new Uri("/Resources/icon_default.png", UriKind.Relative));

            BitmapImage biImg = new();
            MemoryStream ms = new(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();
            biImg.Freeze();

            return biImg;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
