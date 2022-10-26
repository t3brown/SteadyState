using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Xml.Serialization;

namespace SteadyState.MainProject.WPF.Models
{
	public class EnableColumns : ICloneable, INotifyPropertyChanged
	{
		private bool _vertexIndex;

		[Category("Узлы")]
		[DisplayName("Индекс узла")]
		public bool VertexIndex 
		{ 
			get => _vertexIndex; 
			set
			{ 
				if (_vertexIndex == value) return;
				if (!_vertexTitle && !value) return;
				_vertexIndex = value; OnPropertyChanged();
			}
		}

		private bool _vertexTitle = true;

		[Category("Узлы")]
		[DisplayName("Название узла")]
		public bool VertexTitle { get => _vertexTitle; set { if (_vertexTitle == value) return; if (!_vertexIndex && !value) return; _vertexTitle = value; OnPropertyChanged(); } }

		private bool _isBasic = true;

		[Category("Узлы")]
		[DisplayName("Базис")]
		public bool IsBasic { get => _isBasic; set { if (_isBasic == value) return; _isBasic = value; OnPropertyChanged(); } }

		private bool _voltNom = true;

		[Category("Узлы")]
		[DisplayName("Uном")]
		public bool VoltNom { get => _voltNom; set { if (_voltNom == value) return; _voltNom = value; OnPropertyChanged(); } }

		private bool _shn;

		[Category("Узлы")]
		[DisplayName("СХН")]
		public bool Shn { get => _shn; set { if (_shn == value) return; _shn = value; OnPropertyChanged(); } }

		private bool _powerRe = true;

		[Category("Узлы")]
		[DisplayName("Pнагр.")]
		public bool PowerRe { get => _powerRe; set { if (_powerRe == value) return; _powerRe = value; OnPropertyChanged(); } }

		private bool _powerIm = true;

		[Category("Узлы")]
		[DisplayName("Qнагр.")]
		public bool PowerIm { get => _powerIm; set { if (_powerIm == value) return; _powerIm = value; OnPropertyChanged(); } }

		private bool _voltSus;

		[Category("Узлы")]
		[DisplayName("Uпод.")]
		public bool VoltSus { get => _voltSus; set { if (_voltSus == value) return; _voltSus = value; OnPropertyChanged(); } }

		private bool _minQ;

		[Category("Узлы")]
		[DisplayName("Qмин.")]
		public bool MinQ { get => _minQ; set { if (_minQ == value) return; _minQ = value; OnPropertyChanged(); } }

		private bool _maxQ;

		[Category("Узлы")]
		[DisplayName("Qмакс.")]
		public bool MaxQ { get => _maxQ; set { if (_maxQ == value) return; _maxQ = value; OnPropertyChanged(); } }

		private bool _voltRe;

		[Category("Узлы")]
		[DisplayName("U'")]
		public bool VoltRe { get => _voltRe; set { if (_voltRe == value) return; _voltRe = value; OnPropertyChanged(); } }

		private bool _voltIm;

		[Category("Узлы")]
		[DisplayName("U''")]
		public bool VoltIm { get => _voltIm; set { if (_voltIm == value) return; _voltIm = value; OnPropertyChanged(); } }

		private bool _voltMagn = true;

		[Category("Узлы")]
		[DisplayName("U")]
		public bool VoltMagn { get => _voltMagn; set { if (_voltMagn == value) return; _voltMagn = value; OnPropertyChanged(); } }

		private bool _voltAngle = true;

		[Category("Узлы")]
		[DisplayName("δ напр.")]
		public bool VoltAngle { get => _voltAngle; set { if (_voltAngle == value) return; _voltAngle = value; OnPropertyChanged(); } }

		private bool _edgeIndex;
		public bool EdgeIndex { get => _edgeIndex; set { if (_edgeIndex == value) return; if (!_edgeTitle && !value) return; _edgeIndex = value; OnPropertyChanged(); } }

		private bool _edgeTitle = true;
		public bool EdgeTitle { get => _edgeTitle; set { if (_edgeTitle == value) return;
			if (!_edgeIndex && !value) return; _edgeTitle = value; OnPropertyChanged(); } }

		private bool _v1 = true;
		public bool V1 { get => _v1; set { if (_v1 == value) return; _v1 = value; OnPropertyChanged(); } }

		private bool _v2 = true;
		public bool V2 { get => _v2; set { if (_v2 == value) return; _v2 = value; OnPropertyChanged(); } }

		private bool _r = true;
		public bool R { get => _r; set { if (_r == value) return; _r = value; OnPropertyChanged(); } }

		private bool _x = true;
		public bool X { get => _x; set { if (_x == value) return; _x = value; OnPropertyChanged(); } }

		private bool _b = true;
		public bool B { get => _b; set { if (_b == value) return; _b = value; OnPropertyChanged(); } }

		private bool _u1 = true;
		public bool U1 { get => _u1; set { if (_u1 == value) return; _u1 = value; OnPropertyChanged(); } }

		private bool _u2 = true;
		public bool U2 { get => _u2; set { if (_u2 == value) return; _u2 = value; OnPropertyChanged(); } }

		private bool _angle;
		public bool Angle { get => _angle; set { if (_angle == value) return; _angle = value; OnPropertyChanged(); } }

		private bool _rpn1 = true;
		public bool Rpn1 { get => _rpn1; set { if (_rpn1 == value) return; _rpn1 = value; OnPropertyChanged(); } }

		private bool _rpn2;
		public bool Rpn2 { get => _rpn2; set { if (_rpn2 == value) return; _rpn2 = value; OnPropertyChanged(); } }

		private bool _reCoeff;
		public bool ReCoeff { get => _reCoeff; set { if (_reCoeff == value) return; _reCoeff = value; OnPropertyChanged(); } }

		private bool _imCoeff;
		public bool ImCoeff { get => _imCoeff; set { if (_imCoeff == value) return; _imCoeff = value; OnPropertyChanged(); } }

		private bool _ampRe;
		public bool AmpRe { get => _ampRe; set { if (_ampRe == value) return; _ampRe = value; OnPropertyChanged(); } }

		private bool _ampIm;
		public bool AmpIm { get => _ampIm; set { if (_ampIm == value) return; _ampIm = value; OnPropertyChanged(); } }

		private bool _ampMagn = true;
		public bool AmpMagn { get => _ampMagn; set { if (_ampMagn == value) return; _ampMagn = value; OnPropertyChanged(); } }

		private bool _ampAngle;
		public bool AmpAngle { get => _ampAngle; set { if (_ampAngle == value) return; _ampAngle = value; OnPropertyChanged(); } }

		private bool _pwrStRe = true;
		public bool PwrStRe { get => _pwrStRe; set { if (_pwrStRe == value) return; _pwrStRe = value; OnPropertyChanged(); } }

		private bool _pwrStIm = true;
		public bool PwrStIm { get => _pwrStIm; set { if (_pwrStIm == value) return; _pwrStIm = value; OnPropertyChanged(); } }

		private bool _pwrStCh;
		public bool PwrStCh { get => _pwrStCh; set { if (_pwrStCh == value) return; _pwrStCh = value; OnPropertyChanged(); } }

		private bool _pwrEndRe = true;
		public bool PwrEndRe { get => _pwrEndRe; set { if (_pwrEndRe == value) return; _pwrEndRe = value; OnPropertyChanged(); } }

		private bool _pwrEndIm = true;
		public bool PwrEndIm { get => _pwrEndIm; set { if (_pwrEndIm == value) return; _pwrEndIm = value; OnPropertyChanged(); } }

		private bool _pwrEndCh;
		public bool PwrEndCh { get => _pwrEndCh; set { if (_pwrEndCh == value) return; _pwrEndCh = value; OnPropertyChanged(); } }

		private bool _pwrDltRe = true;
		public bool PwrDltRe { get => _pwrDltRe; set { if (_pwrDltRe == value) return; _pwrDltRe = value; OnPropertyChanged(); } }

		private bool _pwrDltIm = true;
		public bool PwrDltIm { get => _pwrDltIm; set { if (_pwrDltIm == value) return; _pwrDltIm = value; OnPropertyChanged(); } }
		public object Clone() => MemberwiseClone();

		private static readonly string enableColumnsFilename = "enableColumns";

		public static EnableColumns GetEnableColumns()
		{
			var filename = enableColumnsFilename;
			var enableColumns = new EnableColumns();
			if (File.Exists(filename))
			{
				using (FileStream fs = new FileStream(filename, FileMode.Open))
				{
					XmlSerializer xser = new XmlSerializer(typeof(EnableColumns));
					enableColumns = xser.Deserialize(fs) as EnableColumns;
				}
			}
			return enableColumns;
		}

		public void Save()
		{
			var filename = enableColumnsFilename;
			if (File.Exists(filename)) File.Delete(filename);
			using (FileStream fs = new FileStream(filename, FileMode.Create))
			{
				XmlSerializer xser = new XmlSerializer(typeof(EnableColumns));
				xser.Serialize(fs, this);
				fs.Close();
			}
		}
		public void CopyPropertiesValue(object source)
		{
			var props = source.GetType().GetProperties();
			foreach (var prop in props)
				this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(source));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public sealed class BooleanToVisibilityConverter : IValueConverter
	{
		private Visibility TrueValue { get; set; }
		private Visibility FalseValue { get; set; }

		public BooleanToVisibilityConverter()
		{
			TrueValue = Visibility.Visible;
			FalseValue = Visibility.Collapsed;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = false;
			if (value is bool)
			{
				flag = (bool)value;
			}
			return flag ? TrueValue : FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (Equals(value, TrueValue))
				return true;
			if (Equals(value, FalseValue))
				return false;
			return null;
		}
	}
}
