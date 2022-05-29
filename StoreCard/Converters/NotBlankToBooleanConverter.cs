#region

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using StoreCard.Utils;

#endregion

namespace StoreCard.Converters;

[ValueConversion(typeof(string), typeof(bool))]
public sealed class NotBlankToBooleanConverter : IValueConverter
{
    #region IValueConverter Members

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str.Trim() != string.Empty;
        }

        Logger.Log("Tried to convert non-string value as a string.");
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion
}
