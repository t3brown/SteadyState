using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SteadyState.Grapher.Annotations;
using SteadyState.Grapher.Controls;
using SteadyState.Grapher.Interfaces;
using SteadyState.Interfaces;

namespace SteadyState.Grapher.Elements
{
	public class Vertex : CircuitElement, IVertexGrapher
	{
		public event Action<IVertex> BasicVertexChanged;

		public event Action<IVertex> VoltNomChanged;

		public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
			"Angle", typeof(double), typeof(Vertex), new PropertyMetadata(default(double)));

		private double? _voltNom;
		private bool _isBasic;
		private Guid _shnId;
		private double? _powerRe;
		private double? _powerIm;
		private double? _voltSus;
		private double? _minQ;
		private double? _maxQ;
		private double? _voltRe;
		private double? _voltIm;
		private double? _voltMagn;
		private double? _voltAngle;

		public Point StartPoint { get; set; }

		public double Angle
		{
			get { return (double)GetValue(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}

		static Vertex()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Vertex), new FrameworkPropertyMetadata(typeof(Vertex)));
		}

		public Vertex()
		{
			BasicVertexChanged += VertexBasicVertexChanged;
			VoltNomChanged += Vertex_VoltNomChanged;
		}

		private void Vertex_VoltNomChanged(IVertex obj)
		{
			if (SchematicEditor.BasicVertex != null)
			{
				DepthFirstSearch.DFS(SchematicEditor.BasicVertex);
			}

		}

		#region properties

		public double? VoltNom
		{
			get => _voltNom;
			set
			{
				if (Nullable.Equals(value, _voltNom)) return;
				_voltNom = value;
				VoltNomChanged?.Invoke(this);
				OnPropertyChanged();
			}
		}

		public bool IsBasic
		{
			get => _isBasic;
			set
			{
				if (value == _isBasic) return;
				if (value == true)
					if (SchematicEditor.BasicVertex != null)
						SchematicEditor.BasicVertex.IsBasic = false;
				SchematicEditor.BasicVertex = this;
				_isBasic = value;
				BasicVertexChanged?.Invoke(this);
				OnPropertyChanged();
			}
		}

		public Guid ShnId
		{
			get => _shnId;
			set
			{
				if (Equals(value, _shnId)) return;
				_shnId = value;
				OnPropertyChanged();
			}
		}

		public double? PowerRe
		{
			get => _powerRe;
			set
			{
				if (Nullable.Equals(value, _powerRe)) return;
				_powerRe = value;
				OnPropertyChanged();
			}
		}

		public double? PowerIm
		{
			get => _powerIm;
			set
			{
				if (Nullable.Equals(value, _powerIm)) return;
				_powerIm = value;
				OnPropertyChanged();
			}
		}

		public double? VoltSus
		{
			get => _voltSus;
			set
			{
				if (Nullable.Equals(value, _voltSus)) return;
				_voltSus = value;
				OnPropertyChanged();
			}
		}

		public double? MinQ
		{
			get => _minQ;
			set
			{
				if (Nullable.Equals(value, _minQ)) return;
				_minQ = value;
				OnPropertyChanged();
			}
		}

		public double? MaxQ
		{
			get => _maxQ;
			set
			{
				if (Nullable.Equals(value, _maxQ)) return;
				_maxQ = value;
				OnPropertyChanged();
			}
		}

		public double? VoltRe
		{
			get => _voltRe;
			set
			{
				if (Nullable.Equals(value, _voltRe)) return;
				_voltRe = value;
				OnPropertyChanged();
			}
		}

		public double? VoltIm
		{
			get => _voltIm;
			set
			{
				if (Nullable.Equals(value, _voltIm)) return;
				_voltIm = value;
				OnPropertyChanged();
			}
		}

		public double? VoltMagn
		{
			get => _voltMagn;
			set
			{
				if (Nullable.Equals(value, _voltMagn)) return;
				_voltMagn = value;
				OnPropertyChanged();
			}
		}

		public double? VoltAngle
		{
			get => _voltAngle;
			set
			{
				if (Nullable.Equals(value, _voltAngle)) return;
				_voltAngle = value;
				OnPropertyChanged();
			}
		}

		#endregion

		private void VertexBasicVertexChanged(IVertex obj)
		{
			//запускается посик в глубину при смене базисного узла.
			DepthFirstSearch.DFS(obj);
		}

	}
}
