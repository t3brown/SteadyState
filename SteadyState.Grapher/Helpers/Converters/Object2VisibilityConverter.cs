using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SteadyState.Grapher.Helpers.Converters
{
	public class Object2VisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is null ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
