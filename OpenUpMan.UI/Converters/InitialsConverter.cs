using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Linq;

namespace OpenUpMan.UI.Converters;

public class InitialsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string username || string.IsNullOrWhiteSpace(username))
            return "??";

        // Si el username tiene espacios, tomar las primeras letras de cada palabra
        var words = username.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length > 1)
        {
            return string.Concat(words.Take(2).Select(w => char.ToUpper(w[0])));
        }

        // Si no tiene espacios, tomar las primeras dos letras
        return username.Length >= 2 
            ? username.Substring(0, 2).ToUpper() 
            : username.ToUpper();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

