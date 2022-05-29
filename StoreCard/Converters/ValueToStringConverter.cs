#region

using System;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace StoreCard.Converters;

[ValueConversion(typeof(object), typeof(string))]
public sealed class ValueToStringConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value?.ToString() ?? string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion
}
