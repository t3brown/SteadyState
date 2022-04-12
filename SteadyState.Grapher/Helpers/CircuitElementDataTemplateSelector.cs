using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SteadyState.Grapher.Elements;
using SteadyState.Interfaces;

namespace SteadyState.Grapher.Helpers
{
    internal class CircuitElementDataTemplateSelector: DataTemplateSelector
    {
        public DataTemplate VertexTemplate { get; set; }
        public DataTemplate EdgeTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
            => item is IVertex ? VertexTemplate : item is IEdge ? EdgeTemplate : null;
    }
}
