using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SteadyState.MainProject.WPF.Models
{
	public class DisplayPrecision : ICloneable
	{
		public DisplayPrecisionType VoltNom { get; set; } = DisplayPrecisionType.three;

		public DisplayPrecisionType PowerRe { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType PowerIm { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType VoltSus { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType MinQ { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType MaxQ { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType VoltRe { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType VoltIm { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType VoltMagn { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType VoltAngle { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType R { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType X { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType B { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType U1 { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType U2 { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType Angle { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType ReCoef { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType ImCoef { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType AmpRe { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType AmpIm { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType AmpMagn { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType AmpAngle { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType PwrStRe { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType PwrStIm { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType PwrStCh { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType PwrEndRe { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType PwrEndIm { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType PwrEndCh { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType PwrDltRe { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType PwrDltIm { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType LongitudinalPowerReLosses { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType LongitudinalPowerImLosses { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType TransversePowerReLosses { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType TransversePowerImLosses { get; set; } = DisplayPrecisionType.three;
		public DisplayPrecisionType СhargingPower { get; set; } = DisplayPrecisionType.three;

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

	public enum DisplayPrecisionType : byte
	{
		[Description("1 (0)")]
		zero = 0,

		[Description("0.1 (1)")]
		one = 1,

		[Description("0.01 (2)")]
		two = 2,

		[Description("0.001 (3)")]
		three = 3,

		[Description("0.0001 (4)")]
		four = 4,

		[Description("0.00001 (5)")]
		five = 5,

		[Description("0.000001 (6)")]
		six = 6,

		[Description("0.0000001 (7)")]
		seven = 7,

		[Description("0.00000001 (8)")]
		eight = 8,

		[Description("0.000000001 (9)")]
		nine = 9,

	}
}
