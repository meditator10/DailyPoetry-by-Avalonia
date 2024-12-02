using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DailyPoetryA.Client.Converters;

public class CountToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    => value is int count && parameter is string conditionParameter
                          && int.TryParse(conditionParameter, out var condition)
            ? count > condition
            : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}