using SteadyState.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SteadyState.MainProject.WPF.Components.Helpers
{
	internal class PropertyValueDataTemplateSelector: DataTemplateSelector
	{
		public DataTemplate? TextTemplate { get; set; }
		public DataTemplate? BoolTemplate { get; set; }
		public DataTemplate? EnumTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
			=> (item switch
			{
				bool => BoolTemplate,
				Enum => EnumTemplate,
				_ => TextTemplate
			})!;
	}
}
