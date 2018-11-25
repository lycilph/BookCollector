using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Panda.Converters
{
    public class BoolToFontStyleConverter : IValueConverter
    {
        public FontStyle TrueStyle { get; set; } = FontStyles.Italic;
        public FontStyle FalseStyle { get; set; } = FontStyles.Normal;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return TrueStyle;
            else
                return FalseStyle;
                
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
