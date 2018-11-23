using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Panda.Converters
{
    public class StringJoinConverter : IValueConverter
    {
        public string Separator { get; set; } = ",";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IEnumerable<string> strs))
                throw new ArgumentException("Value must be of type IEnumerable<string>");

            return string.Join(Separator, strs);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
