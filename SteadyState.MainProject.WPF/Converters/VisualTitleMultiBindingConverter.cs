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
	internal class VisualTitleMultiBindingConverter : Freezable, IMultiValueConverter
	{
		#region Видимость колонок

		public static readonly DependencyProperty EnableColumnsProperty = DependencyProperty.Register(
			nameof(EnableColumns), typeof(EnableColumns), typeof(VisualTitleMultiBindingConverter), new PropertyMetadata(default(EnableColumns)));

		public EnableColumns EnableColumns
		{
			get => (EnableColumns)GetValue(EnableColumnsProperty);
			set => SetValue(EnableColumnsProperty, value);
		}

		#endregion

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var index = string.Empty;
			var title = string.Empty;

			var indexVisibility = true;
			var titleVisibility = true;

			if (parameter is string props)
			{
				var propertyNames = props.Split(';');
				indexVisibility = (bool)EnableColumns.GetType().GetProperty(propertyNames[0])!.GetValue(EnableColumns)!;
				titleVisibility = (bool)EnableColumns.GetType().GetProperty(propertyNames[1])!.GetValue(EnableColumns)!;
			}


			if (values[0] is int indexValue)
			{
				index = indexValue.ToString();
			}

			if (values[1] is string titleValue)
			{
				title = titleValue;
			}

			if (titleVisibility && string.IsNullOrEmpty(index) && !string.IsNullOrEmpty(title))
			{
				return title;
			}

			if (indexVisibility && !string.IsNullOrEmpty(index) && string.IsNullOrEmpty(title))
			{
				return index;
			}

			if (indexVisibility && titleVisibility && !string.IsNullOrEmpty(index) && !string.IsNullOrEmpty(title))
			{
				return $"{index}. {title}";
			}

			return string.Empty;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		protected override Freezable CreateInstanceCore()
		{
			return new BindingProxy();
		}
	}
}
