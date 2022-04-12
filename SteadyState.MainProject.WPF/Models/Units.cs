using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SteadyState.MainProject.WPF.Models
{
	public class Units: ICloneable, INotifyPropertyChanged
	{
	private string _voltNom = "кВ";

	public string VoltNom
	{
		get => _voltNom;
		set
		{
			if (_voltNom == value) return;
			_voltNom = value;
			OnPropertyChanged();
		}
	}

	private string _powerRe = "МВт";

	public string PowerRe
	{
		get => _powerRe;
		set
		{
			if (_powerRe == value) return;
			_powerRe = value;
			OnPropertyChanged();
		}
	}

	private string _powerIm = "Мвар";

	public string PowerIm
	{
		get => _powerIm;
		set
		{
			if (_powerIm == value) return;
			_powerIm = value;
			OnPropertyChanged();
		}
	}

	private string _voltSus = "кВ";

	public string VoltSus
	{
		get => _voltSus;
		set
		{
			if (_voltSus == value) return;
			_voltSus = value;
			OnPropertyChanged();
		}
	}

	private string _minQ = "Мвар";

	public string MinQ
	{
		get => _minQ;
		set
		{
			if (_minQ == value) return;
			_minQ = value;
			OnPropertyChanged();
		}
	}

	private string _maxQ = "Мвар";

	public string MaxQ
	{
		get => _maxQ;
		set
		{
			if (_maxQ == value) return;
			_maxQ = value;
			OnPropertyChanged();
		}
	}

	private string _voltRe = "кВ";

	public string VoltRe
	{
		get => _voltRe;
		set
		{
			if (_voltRe == value) return;
			_voltRe = value;
			OnPropertyChanged();
		}
	}

	private string _voltIm = "кВ";

	public string VoltIm
	{
		get => _voltIm;
		set
		{
			if (_voltIm == value) return;
			_voltIm = value;
			OnPropertyChanged();
		}
	}

	private string _voltMagn = "кВ";

	public string VoltMagn
	{
		get => _voltMagn;
		set
		{
			if (_voltMagn == value) return;
			_voltMagn = value;
			OnPropertyChanged();
		}
	}

	private string _voltAngle = "°";

	public string VoltAngle
	{
		get => _voltAngle;
		set
		{
			if (_voltAngle == value) return;
			_voltAngle = value;
			OnPropertyChanged();
		}
	}

	private string _r = "Ом";

	public string R
	{
		get => _r;
		set
		{
			if (_r == value) return;
			_r = value;
			OnPropertyChanged();
		}
	}

	private string _x = "Ом";

	public string X
	{
		get => _x;
		set
		{
			if (_x == value) return;
			_x = value;
			OnPropertyChanged();
		}
	}

	private string _b = "мкСм";

	public string B
	{
		get => _b;
		set
		{
			if (_b == value) return;
			_b = value;
			OnPropertyChanged();
		}
	}

	private string _u1 = "кВ";

	public string U1
	{
		get => _u1;
		set
		{
			if (_u1 == value) return;
			_u1 = value;
			OnPropertyChanged();
		}
	}

	private string _u2 = "кВ";

	public string U2
	{
		get => _u2;
		set
		{
			if (_u2 == value) return;
			_u2 = value;
			OnPropertyChanged();
		}
	}

	private string _angle = "°";

	public string Angle
	{
		get => _angle;
		set
		{
			if (_angle == value) return;
			_angle = value;
			OnPropertyChanged();
		}
	}

	private string _ampRe = "А";

	public string AmpRe
	{
		get => _ampRe;
		set
		{
			if (_ampRe == value) return;
			_ampRe = value;
			OnPropertyChanged();
		}
	}

	private string _ampIm = "А";

	public string AmpIm
	{
		get => _ampIm;
		set
		{
			if (_ampIm == value) return;
			_ampIm = value;
			OnPropertyChanged();
		}
	}

	private string _ampMagn = "А";

	public string AmpMagn
	{
		get => _ampMagn;
		set
		{
			if (_ampMagn == value) return;
			_ampMagn = value;
			OnPropertyChanged();
		}
	}

	private string _ampAngle = "°";

	public string AmpAngle
	{
		get => _ampAngle;
		set
		{
			if (_ampAngle == value) return;
			_ampAngle = value;
			OnPropertyChanged();
		}
	}

	private string _pwrStRe = "МВт";

	public string PwrStRe
	{
		get => _pwrStRe;
		set
		{
			if (_pwrStRe == value) return;
			_pwrStRe = value;
			OnPropertyChanged();
		}
	}

	private string _pwrStIm = "Мвар";

	public string PwrStIm
	{
		get => _pwrStIm;
		set
		{
			if (_pwrStIm == value) return;
			_pwrStIm = value;
			OnPropertyChanged();
		}
	}

	private string _pwrStCh = "Мвар";

	public string PwrStCh
	{
		get => _pwrStCh;
		set
		{
			if (_pwrStCh == value) return;
			_pwrStCh = value;
			OnPropertyChanged();
		}
	}

	private string _pwrEndRe = "МВт";

	public string PwrEndRe
	{
		get => _pwrEndRe;
		set
		{
			if (_pwrEndRe == value) return;
			_pwrEndRe = value;
			OnPropertyChanged();
		}
	}

	private string _pwrEndIm = "Мвар";

	public string PwrEndIm
	{
		get => _pwrEndIm;
		set
		{
			if (_pwrEndIm == value) return;
			_pwrEndIm = value;
			OnPropertyChanged();
		}
	}

	private string _pwEndCh = "Мвар";

	public string PwrEndCh
	{
		get => _pwEndCh;
		set
		{
			if (_pwEndCh == value) return;
			_pwEndCh = value;
			OnPropertyChanged();
		}
	}

	private string _pwrDltRe = "МВт";

	public string PwrDltRe
	{
		get => _pwrDltRe;
		set
		{
			if (_pwrDltRe == value) return;
			_pwrDltRe = value;
			OnPropertyChanged();
		}
	}

	private string _pwrDltIm = "Мвар";

	public string PwrDltIm
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

	private string _longitudinalPowerReLosses = "МВт";

	public string LongitudinalPowerReLosses
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

	private string _longitudinalPowerImLosses = "Мвар";

	public string LongitudinalPowerImLosses
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

	private string _transversePowerReLosses = "МВт";

	public string TransversePowerReLosses
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

	private string _transversePowerImLosses = "Мвар";

	public string TransversePowerImLosses
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

	private string _chargingPower = "Мвар";

	public string СhargingPower
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
}
