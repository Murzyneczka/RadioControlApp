using System.Globalization;

namespace RadioControlApp.Converters;

/// <summary>
/// Konwerter string na bool - true jeśli string nie jest pusty.
/// </summary>
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Konwerter bool na kolor statusu.
/// </summary>
public class BoolToStatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool hasError)
        {
            return hasError ? Colors.Red : Colors.Green;
        }
        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Konwerter statusu połączenia na kolor.
/// </summary>
public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = value?.ToString() ?? "";
        
        return status.ToLower() switch
        {
            "połączono" or "connected" => Colors.Green,
            "połączono (live)" => Colors.LimeGreen,
            "rozłączono" or "disconnected" => Colors.Red,
            "błąd połączenia" or "connection error" => Colors.Orange,
            _ => Colors.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}