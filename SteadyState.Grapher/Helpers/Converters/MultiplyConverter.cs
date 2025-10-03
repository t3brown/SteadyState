using System;
using System.Globalization;
using System.Windows.Data;

namespace SteadyState.Grapher.Helpers.Converters
{
	public class MultiplyConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return value;
			return double.Parse((parameter ?? "1").ToString()) * (double)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
