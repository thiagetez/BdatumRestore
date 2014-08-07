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

        /// <summary>
        /// Coloca a imagem nos folders
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                    value = "{DisconnectedItem}";
                IFolder folder = value as IFolder;
                if (folder != null)
                {
                    if (folder.isFolder)
                    {
                        Uri uri = new Uri
                        ("pack://application:,,,/Resources/folder.png");
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
