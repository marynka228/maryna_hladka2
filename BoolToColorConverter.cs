using System.Globalization;

namespace maryna_hladka2.Converters
{
    // Приклад IValueConverter, якщо викладач хоче саме конвертер, а не обчислювану властивість.
    // Прив'язується так: TextColor="{Binding IsHighAverage, Converter={StaticResource BoolToColorConverter}}"
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isHigh = value is bool b && b;
            return isHigh ? Colors.Green : Colors.Red;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
