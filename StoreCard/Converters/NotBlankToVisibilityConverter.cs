using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StoreCard.Converters;

[ValueConversion(typeof(string), typeof(Visibility))]

internal sealed class NotBlankToVisibilityConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str.Trim() == string.Empty ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion
}
