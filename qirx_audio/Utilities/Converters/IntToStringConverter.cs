using System;
using System.Globalization;

namespace softsyst.qirx.Converters
{
    public class IntToStringConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
               return  ((int)value).ToString();
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string s = (string)value;
                int num;
                if (int.TryParse(s, out num))
                    return num;
            }
            return 0;
        }
    }
}
