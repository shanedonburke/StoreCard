// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace StoreCard.Converters;

[ValueConversion(typeof(object[]), typeof(bool))]

internal class MultiValueEqualityConverter : IMultiValueConverter
{
    #region IValueConverter Members

    public object Convert(object?[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.All(o => o?.Equals(values[0]) == true) || values.All(o => o == null);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion
}
