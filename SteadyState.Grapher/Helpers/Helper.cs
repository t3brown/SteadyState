using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SteadyState.Grapher.Helpers
{
	public static class Helper
	{
		public static T? FindParentOfType<T>(this DependencyObject child) where T : DependencyObject
		{
			var parent = child;

			do
			{
				parent = VisualTreeHelper.GetParent(parent);

				if (parent is T result)
				{
					return result;
				}
			}
			while (parent != null);

			return null;
		}
	}
}
