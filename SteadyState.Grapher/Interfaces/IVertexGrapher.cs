using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SteadyState.Interfaces;

namespace SteadyState.Grapher.Interfaces
{
	public interface IVertexGrapher: IVertex
	{
		public Point StartPoint { get; set; }
		public double Width { get; set; }
		public double Angle { get; set; }
    }
}
