using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SteadyState.MainProject.WPF.Models
{
	public class DisplayPrecision : ICloneable
	{
		public byte VoltNom { get; set; } = 3;
		public byte PowerRe { get; set; } = 3;
		public byte PowerIm { get; set; } = 3;
		public byte VoltSus { get; set; } = 3;
		public byte MinQ { get; set; } = 3;
		public byte MaxQ { get; set; } = 3;
		public byte VoltRe { get; set; } = 3;
		public byte VoltIm { get; set; } = 3;
		public byte VoltMagn { get; set; } = 3;
		public byte VoltAngle { get; set; } = 3;
		public byte R { get; set; } = 3;
		public byte X { get; set; } = 3;
		public byte B { get; set; } = 3;
		public byte U1 { get; set; } = 3;
		public byte U2 { get; set; } = 3;
		public byte Angle { get; set; } = 3;
		public byte ReCoef { get; set; } = 3;
		public byte ImCoef { get; set; } = 3;
		public byte AmpRe { get; set; } = 3;
		public byte AmpIm { get; set; } = 3;
		public byte AmpMagn { get; set; } = 3;
		public byte AmpAngle { get; set; } = 3;
		public byte PwrStRe { get; set; } = 3;
		public byte PwrStIm { get; set; } = 3;
		public byte PwrStCh { get; set; } = 3;
		public byte PwrEndRe { get; set; } = 3;
		public byte PwrEndIm { get; set; } = 3;
		public byte PwrEndCh { get; set; } = 3;
		public byte PwrDltRe { get; set; } = 3;
		public byte PwrDltIm { get; set; } = 3;
		public byte LongitudinalPowerReLosses { get; set; } = 3;
		public byte LongitudinalPowerImLosses { get; set; } = 3;
		public byte TransversePowerReLosses { get; set; } = 3;
		public byte TransversePowerImLosses { get; set; } = 3;
		public byte СhargingPower { get; set; } = 3;

		public object Clone() => MemberwiseClone();

		private static readonly string precisionFilename = "precision";

		public static DisplayPrecision GetPrecision()
		{
			var filename = precisionFilename;
			var precision = new DisplayPrecision();
			if (File.Exists(filename))
			{
				using (FileStream fs = new FileStream(filename, FileMode.Open))
				{
					XmlSerializer xser = new XmlSerializer(typeof(DisplayPrecision));
					precision = xser.Deserialize(fs) as DisplayPrecision;
				}
			}

			return precision;
		}

		public void Save()
		{
			var filename = precisionFilename;
			if (File.Exists(filename)) File.Delete(filename);
			using (FileStream fs = new FileStream(filename, FileMode.Create))
			{
				XmlSerializer xser = new XmlSerializer(typeof(DisplayPrecision));
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
	}
}
