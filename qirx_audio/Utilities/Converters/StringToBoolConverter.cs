using System;
using System.Globalization;

namespace softsyst.qirx.Converters
{
    public class StringToBoolConverter 
    {
        public object Convert(object value, Type targetType,
                            object parameter, CultureInfo culture)
        {
            if (!(value is string))
                throw new Exception("StringToBoolConverter: Wrong datatype");

            string sval = (string)value;
            sval = sval.ToLower();

            if (sval == "yes" || sval =="true" || sval == "on")
                return (bool)true;
            return (bool)false;
        }

        public object ConvertBack(object value, Type targetType,
                                object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new Exception("StringToBoolConverter: Wrong datatype");

            if ((bool)value)
                return "yes";
            return "no";
        }
    }
}
