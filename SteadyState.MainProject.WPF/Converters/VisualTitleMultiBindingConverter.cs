using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SteadyState.MainProject.WPF.Converters
{
	internal class VisualTitleMultiBindingConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var index = string.Empty;
			var title = string.Empty;


			if (values[0] is int indexValue)
			{
				index = indexValue.ToString();
			}

			if (values[1] is string titleValue)
			{
				title = titleValue;
			}

			if (string.IsNullOrEmpty(index) && !string.IsNullOrEmpty(title))
			{
				return title;
			}

			if (!string.IsNullOrEmpty(index) && string.IsNullOrEmpty(title))
			{
				return index;
			}

			if (!string.IsNullOrEmpty(index) && !string.IsNullOrEmpty(title))
			{
				return $"{index}. {title}";
			}

			return string.Empty;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
