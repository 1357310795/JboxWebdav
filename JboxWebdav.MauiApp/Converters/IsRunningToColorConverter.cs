using System.Globalization;

namespace JboxWebdav.MauiApp.Converters
{
    public class IsRunningToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isrunning = (bool) value;
            if (isrunning)
                return new SolidColorBrush(Color.FromRgb(89, 202, 156));
            else
                return new SolidColorBrush(Color.FromRgb(244, 67, 54));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
