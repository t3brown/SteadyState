using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SteadyState.MainProject.WPF.Infrastructure;
using SteadyState.MainProject.WPF.Models;

namespace SteadyState.MainProject.WPF.Converters
{
	internal class VisualTitleMultiBindingConverter : IMultiValueConverter
	{

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var index = string.Empty;
			var title = string.Empty;

			var indexVisibility = true;
			var titleVisibility = true;

			if (values.Length > 2)
			{
				indexVisibility = values[2] is not bool iV || iV;
				titleVisibility = values[3] is not bool tV || tV;
			}

			if (values[0] is int indexValue)
			{
				index = indexValue.ToString();
			}

			if (values[1] is string titleValue)
			{
				title = titleValue;
			}

			if (indexVisibility && titleVisibility && !string.IsNullOrEmpty(index) && !string.IsNullOrEmpty(title))
			{
				return $"{index}. {title}";
			}

			if ((titleVisibility || string.IsNullOrEmpty(index)) && !string.IsNullOrEmpty(title))
			{
				return title;
			}

			if ((indexVisibility || string.IsNullOrEmpty(title)) && !string.IsNullOrEmpty(index))
			{
				return index;
			}

			return string.Empty;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
