﻿#region

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

#endregion

namespace StoreCard.Converters;

[ValueConversion(typeof(object[]), typeof(bool))]
public sealed class MultiValueEqualityConverter : IMultiValueConverter
{
    #region IValueConverter Members

    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture) =>
        values.All(o => o?.Equals(values[0]) == true) || values.All(o => o == null);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    #endregion
}
