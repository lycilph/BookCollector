using System;
using System.Globalization;
using System.Windows.Data;

namespace Panda.Converters
{
    public class StringEmptyToEnabledConverter : IValueConverter
    {
        public bool IsEmptyValue { get; set; } = false;
        public bool IsNotEmptyValue { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((value ?? "").ToString()) ? IsEmptyValue : IsNotEmptyValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
