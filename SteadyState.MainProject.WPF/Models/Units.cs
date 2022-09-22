using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SteadyState.MainProject.WPF.Infrastructure;
using System.Xml.Serialization;

namespace SteadyState.MainProject.WPF.Models
{
	public class Units : ICloneable, INotifyPropertyChanged
	{
		private VoltageUnits _voltNom = VoltageUnits.kV;

		public VoltageUnits VoltNom
		{
			get => _voltNom;
			set
			{
				if (_voltNom == value) return;
				_voltNom = value;
				OnPropertyChanged();
			}
		}

		private PowerReUnits _powerRe = PowerReUnits.MW;

		public PowerReUnits PowerRe
		{
			get => _powerRe;
			set
			{
				if (_powerRe == value) return;
				_powerRe = value;
				OnPropertyChanged();
			}
		}

		private PowerImUnits _powerIm = PowerImUnits.Mvar;

		public PowerImUnits PowerIm
		{
			get => _powerIm;
			set
			{
				if (_powerIm == value) return;
				_powerIm = value;
				OnPropertyChanged();
			}
		}

		private VoltageUnits _voltSus = VoltageUnits.kV;

		public VoltageUnits VoltSus
		{
			get => _voltSus;
			set
			{
				if (_voltSus == value) return;
				_voltSus = value;
				OnPropertyChanged();
			}
		}

		private PowerImUnits _minQ = PowerImUnits.Mvar;

		public PowerImUnits MinQ
		{
			get => _minQ;
			set
			{
				if (_minQ == value) return;
				_minQ = value;
				OnPropertyChanged();
			}
		}

		private PowerImUnits _maxQ = PowerImUnits.Mvar;

		public PowerImUnits MaxQ
		{
			get => _maxQ;
			set
			{
				if (_maxQ == value) return;
				_maxQ = value;
				OnPropertyChanged();
			}
		}

		private VoltageUnits _voltRe = VoltageUnits.kV;

		public VoltageUnits VoltRe
		{
			get => _voltRe;
			set
			{
				if (_voltRe == value) return;
				_voltRe = value;
				OnPropertyChanged();
			}
		}

		private VoltageUnits _voltIm = VoltageUnits.kV;

		public VoltageUnits VoltIm
		{
			get => _voltIm;
			set
			{
				if (_voltIm == value) return;
				_voltIm = value;
				OnPropertyChanged();
			}
		}

		private VoltageUnits _voltMagn = VoltageUnits.kV;

		public VoltageUnits VoltMagn
		{
			get => _voltMagn;
			set
			{
				if (_voltMagn == value) return;
				_voltMagn = value;
				OnPropertyChanged();
			}
		}

		private AngleUnits _voltAngle = AngleUnits.d;

		public AngleUnits VoltAngle
		{
			get => _voltAngle;
			set
			{
				if (_voltAngle == value) return;
				_voltAngle = value;
				OnPropertyChanged();
			}
		}

		private ResistanceUnits _r = ResistanceUnits.Ohm;

		public ResistanceUnits R
		{
			get => _r;
			set
			{
				if (_r == value) return;
				_r = value;
				OnPropertyChanged();
			}
		}

		private ResistanceUnits _x = ResistanceUnits.Ohm;

		public ResistanceUnits X
		{
			get => _x;
			set
			{
				if (_x == value) return;
				_x = value;
				OnPropertyChanged();
			}
		}

		private ConductivityUnits _b = ConductivityUnits.mkS;

		public ConductivityUnits B
		{
			get => _b;
			set
			{
				if (_b == value) return;
				_b = value;
				OnPropertyChanged();
			}
		}

		private VoltageUnits _u1 = VoltageUnits.kV;

		public VoltageUnits U1
		{
			get => _u1;
			set
			{
				if (_u1 == value) return;
				_u1 = value;
				OnPropertyChanged();
			}
		}

		private VoltageUnits _u2 = VoltageUnits.kV;

		public VoltageUnits U2
		{
			get => _u2;
			set
			{
				if (_u2 == value) return;
				_u2 = value;
				OnPropertyChanged();
			}
		}

		private AngleUnits _angle = AngleUnits.d;

		public AngleUnits Angle
		{
			get => _angle;
			set
			{
				if (_angle == value) return;
				_angle = value;
				OnPropertyChanged();
			}
		}

		private AmperageUnits _ampRe = AmperageUnits.A;

		public AmperageUnits AmpRe
		{
			get => _ampRe;
			set
			{
				if (_ampRe == value) return;
				_ampRe = value;
				OnPropertyChanged();
			}
		}

		private AmperageUnits _ampIm = AmperageUnits.A;

		public AmperageUnits AmpIm
		{
			get => _ampIm;
			set
			{
				if (_ampIm == value) return;
				_ampIm = value;
				OnPropertyChanged();
			}
		}

		private AmperageUnits _ampMagn = AmperageUnits.A;

		public AmperageUnits AmpMagn
		{
			get => _ampMagn;
			set
			{
				if (_ampMagn == value) return;
				_ampMagn = value;
				OnPropertyChanged();
			}
		}

		private AngleUnits _ampAngle = AngleUnits.d;

		public AngleUnits AmpAngle
		{
			get => _ampAngle;
			set
			{
				if (_ampAngle == value) return;
				_ampAngle = value;
				OnPropertyChanged();
			}
		}

		private PowerReUnits _pwrStRe = PowerReUnits.MW;

		public PowerReUnits PwrStRe
		{
			get => _pwrStRe;
			set
			{
				if (_pwrStRe == value) return;
				_pwrStRe = value;
				OnPropertyChanged();
			}
		}

		private PowerImUnits _pwrStIm = PowerImUnits.Mvar;

		public PowerImUnits PwrStIm
		{
			get => _pwrStIm;
			set
			{
				if (_pwrStIm == value) return;
				_pwrStIm = value;
				OnPropertyChanged();
			}
		}

		private PowerImUnits _pwrStCh = PowerImUnits.Mvar;

		public PowerImUnits PwrStCh
		{
			get => _pwrStCh;
			set
			{
				if (_pwrStCh == value) return;
				_pwrStCh = value;
				OnPropertyChanged();
			}
		}

		private PowerReUnits _pwrEndRe = PowerReUnits.MW;

		public PowerReUnits PwrEndRe
		{
			get => _pwrEndRe;
			set
			{
				if (_pwrEndRe == value) return;
				_pwrEndRe = value;
				OnPropertyChanged();
			}
		}

		private PowerImUnits _pwrEndIm = PowerImUnits.Mvar;

		public PowerImUnits PwrEndIm
		{
			get => _pwrEndIm;
			set
			{
				if (_pwrEndIm == value) return;
				_pwrEndIm = value;
				OnPropertyChanged();
			}
		}

		private PowerImUnits _pwEndCh = PowerImUnits.Mvar;

		public PowerImUnits PwrEndCh
		{
			get => _pwEndCh;
			set
			{
				if (_pwEndCh == value) return;
				_pwEndCh = value;
				OnPropertyChanged();
			}
		}

		private PowerReUnits _pwrDltRe = PowerReUnits.MW;

		public PowerReUnits PwrDltRe
		{
			get => _pwrDltRe;
			set
			{
				if (_pwrDltRe == value) return;
				_pwrDltRe = value;
				OnPropertyChanged();
			}
		}

		private PowerImUnits _pwrDltIm = PowerImUnits.Mvar;

		public PowerImUnits PwrDltIm
		{
			get => _pwrDltIm;
			set
			{
				if (_pwrDltIm == value) return;
				_pwrDltIm = value;
				OnPropertyChanged();
			}
		}

		#region LongitudinalPowerReLosses: string

		private PowerReUnits _longitudinalPowerReLosses = PowerReUnits.MW;

		public PowerReUnits LongitudinalPowerReLosses
		{
			get => _longitudinalPowerReLosses;
			set
			{
				if (_longitudinalPowerReLosses == value) return;
				_longitudinalPowerReLosses = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region LongitudinalPowerImLosses: string

		private PowerImUnits _longitudinalPowerImLosses = PowerImUnits.Mvar;

		public PowerImUnits LongitudinalPowerImLosses
		{
			get => _longitudinalPowerImLosses;
			set
			{
				if (_longitudinalPowerImLosses == value) return;
				_longitudinalPowerImLosses = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region TransversePowerReLosses: string

		private PowerReUnits _transversePowerReLosses = PowerReUnits.MW;

		public PowerReUnits TransversePowerReLosses
		{
			get => _transversePowerReLosses;
			set
			{
				if (_transversePowerReLosses == value) return;
				_transversePowerReLosses = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region TransversePowerImLosses: string

		private PowerImUnits _transversePowerImLosses = PowerImUnits.Mvar;

		public PowerImUnits TransversePowerImLosses
		{
			get => _transversePowerImLosses;
			set
			{
				if (_transversePowerImLosses == value) return;
				_transversePowerImLosses = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region СhargingPower: string

		private PowerImUnits _chargingPower = PowerImUnits.Mvar;

		public PowerImUnits СhargingPower
		{
			get => _chargingPower;
			set
			{
				if (_chargingPower == value) return;
				_chargingPower = value;
				OnPropertyChanged();
			}
		}

		#endregion

		public object Clone() => MemberwiseClone();

		private static readonly string unitsFilename = "units";

		public static Units GetUnits()
		{
			var filename = unitsFilename;
			var units = new Units();
			if (File.Exists(filename))
			{
				using (FileStream fs = new FileStream(filename, FileMode.Open))
				{
					XmlSerializer xser = new XmlSerializer(typeof(Units));
					units = xser.Deserialize(fs) as Units;
				}
			}

			return units;
		}

		public void Save()
		{
			var filename = unitsFilename;
			if (File.Exists(filename)) File.Delete(filename);
			using (FileStream fs = new FileStream(filename, FileMode.Create))
			{
				XmlSerializer xser = new XmlSerializer(typeof(Units));
				xser.Serialize(fs, this);
				fs.Close();
			}
		}

		public void CopyPropertiesValue(object? source)
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

	public enum PowerReUnits
	{
		[Description("о.е.")]
		[Browsable(false)]
		relative,

		[Description("МВт")]
		MW,

		[Description("кВт")]
		kW,
	}

	public enum PowerImUnits
	{
		[Description("о.е.")]
		[Browsable(false)]
		relative,

		[Description("Мвар")]
		Mvar,

		[Description("квар")]
		kvar,
	}

	public enum VoltageUnits
	{
		[Description("о.е.")]
		[Browsable(false)]
		relative,

		[Description("кВ")]
		kV,

		[Description("В")]
		V,
	}

	public enum AmperageUnits
	{
		[Description("о.е.")]
		[Browsable(false)]
		relative,

		[Description("А")]
		A,

		[Description("кА")]
		kA,
	}

	public enum AngleUnits
	{
		[Description("°")]
		d,

		[Description("рад")]
		rad,

		[Description("о.е.")]
		[Browsable(false)]
		relative,
	}

	public enum ResistanceUnits
	{
		[Description("о.е.")]
		[Browsable(false)]
		relative,

		[Description("Ом")]
		Ohm,
	}

	public enum ConductivityUnits
	{
		[Description("о.е.")]
		[Browsable(false)]
		relative,

		[Description("мкСм")]
		mkS,
	}
}
