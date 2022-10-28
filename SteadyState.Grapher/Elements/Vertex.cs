using System;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using SteadyState.Grapher.Controls;
using SteadyState.Interfaces;

namespace SteadyState.Grapher.Elements
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Vertex : CircuitElement, IVertex
	{
		public event Action<IVertex> BasicVertexChanged;

		public event Action<IVertex> VoltNomChanged;

		public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
			"Angle", typeof(double), typeof(Vertex), new PropertyMetadata(0.0d));

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
		private IShn _shn;

		[JsonProperty]
		public Point StartPoint { get; set; }

		[JsonProperty]
		public double WidthValue { get => Width; set => Width = value; }

		[JsonProperty]
		public double HeightValue { get => Height; set => Height = value; }

		[JsonProperty]
		public double Angle
		{
			get { return (double)GetValue(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}

		static Vertex()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Vertex), new FrameworkPropertyMetadata(typeof(Vertex)));
		}

		#region properties

		[JsonProperty]
		public double? VoltNomTemp { get; set; }

		[JsonProperty]
		public double? VoltNom
		{
			get => _voltNom;
			set
			{
				if (Nullable.Equals(value, _voltNom)) return;
				IsGround = value == 0;
				_voltNom = value;
				//VoltNomChanged?.Invoke(this);
				OnPropertyChanged();
			}
		}

		public static readonly DependencyProperty IsBasicProperty = DependencyProperty.Register(nameof(IsBasic),
			typeof(bool),
			typeof(Vertex),
			new PropertyMetadata(default(bool), BasicValueChangedCallback));

		private static void BasicValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is Vertex vertex)
			{
				vertex.BasicVertexChanged?.Invoke(vertex);
			}
		}

		[JsonProperty]
		public bool IsBasic
		{
			get { return (bool)GetValue(IsBasicProperty); }
			set { SetValue(IsBasicProperty, value); }
		}


		public static readonly DependencyProperty IsGroundProperty = DependencyProperty.Register(
			"IsGround", typeof(bool), typeof(Vertex), new FrameworkPropertyMetadata(default(bool),
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public bool IsGround
		{
			get { return (bool)GetValue(IsGroundProperty); }
			set { SetValue(IsGroundProperty, value); }
		}

		[JsonProperty]
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

		public IShn Shn
		{
			get => _shn;
			set
			{
				if (Equals(value, _shn)) return;
				_shn = value;
				OnPropertyChanged();
			}
		}

		[JsonProperty]
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

		[JsonProperty]
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

		[JsonProperty]
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

		[JsonProperty]
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

		[JsonProperty]
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

		[JsonProperty]
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

		[JsonProperty]
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

		[JsonProperty]
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

		[JsonProperty]
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
	}
}
