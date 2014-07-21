using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using BDatum.SDK;
using BDatum.SDK.REST;
using System.Configuration;

namespace BdatumRestore.ViewModel
{
    #region HeaderToImageConverter
    [ValueConversion(typeof(string), typeof(bool))]
    public class ImageConverter : IValueConverter
    {
        private static IConfiguration _configuration { get; set; }

        public static ImageConverter Instance =
                new ImageConverter();

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                    value = "{DisconnectedItem}";
                if (value.ToString() != "{DisconnectedItem}")
                {
                    if ((value.ToString()).Contains(@"\"))
                    {
                        Uri uri = new Uri
                        ("pack://application:,,,/Resources/folder.png");
                        BitmapImage source = new BitmapImage(uri);
                        return source;
                    }
                    else
                    {
                        Uri uri = new Uri("pack://application:,,,/Resources/File.png");
                        BitmapImage source = new BitmapImage(uri);
                        return source;
                    }
                }
                return null;
              
            }

            public object ConvertBack(object value, Type targetType,
                object parameter, CultureInfo culture)
            {
                throw new NotSupportedException("Cannot convert back");
            }
       
    }
    #endregion // HeaderToImageConverter
}
