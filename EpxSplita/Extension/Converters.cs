using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace EpxSplita.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class Bool2NotVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool sourceValue = System.Convert.ToBoolean(value);
            if (sourceValue) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility sourceValue = (Visibility)value;
            if (sourceValue == Visibility.Visible) return false;
            else return true;
        }
    }



    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class Bool2Visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool sourceValue = System.Convert.ToBoolean(value);
            if (sourceValue) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility sourceValue = (Visibility)value;
            if (sourceValue == Visibility.Visible) return true;
            else return false;
        }
    }

    [ValueConversion(typeof(String), typeof(String))]
    public class ListBoxGroupConverter : IValueConverter
    {
        public String SplitGroupName { get; set; }
        public String NormalGroupName { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().Contains(SplitGroupName)) return SplitGroupName;
            else return NormalGroupName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class MultiToArrayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.ToArray();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }
    }
}
