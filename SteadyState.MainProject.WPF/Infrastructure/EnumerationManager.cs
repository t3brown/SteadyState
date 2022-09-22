using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SteadyState.MainProject.WPF.Infrastructure
{
	public static class EnumerationManager
	{
		public static string GetEnumDescription(this Enum enumValue)
		{
			var field = enumValue.GetType().GetField(enumValue.ToString());
			if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
			{
				return attribute.Description;
			}
			throw new ArgumentException("Item not found.", nameof(enumValue));
		}
	}
}
