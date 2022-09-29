using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using SteadyState.Grapher.Elements;

namespace SteadyState.MainProject.WPF.Components.Helpers
{
	internal class VertexDataTemplateSelector : DataTemplateSelector
	{
		public DataTemplate? VertexCreatedByEditorTemplate { get; set; }
		public DataTemplate? VertexTemplate { get; set; }

		public override DataTemplate? SelectTemplate(object item, DependencyObject container)
		{

			var cell = container as DataGridCell;

			if (item is Edge edge && VertexCreatedByEditorTemplate != null)
			{
				if (edge.IsCreatedByDataGrid)
				{
					return VertexCreatedByEditorTemplate;
				}
			}

			return base.SelectTemplate(item, container);
		}
	}
}
