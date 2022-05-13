using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace StoreCard.Converters;

internal class NotBlankToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str.Trim() != string.Empty;
        }

        Debug.WriteLine("Tried to convert non-string value as a string.");
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}