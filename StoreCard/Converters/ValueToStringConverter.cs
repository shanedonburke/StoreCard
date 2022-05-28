using System;
using System.Globalization;
using System.Windows.Data;

namespace StoreCard.Converters;

[ValueConversion(typeof(object), typeof(string))]
public class ValueToStringConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion
}
