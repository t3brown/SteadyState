using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteadyState.Interfaces
{
	public interface IVertex : IEntity
	{
		public bool IsConnected { get; set; }
		public double? VoltNom { get; set; }
		public bool IsBasic { get; set; }
		public bool IsGround { get; set; }
		public Guid ShnId { get; set; }
		public IShn Shn { get; set; }
		public double? PowerRe { get; set; }
		public double? PowerIm { get; set; }
		public double? VoltSus { get; set; }
		public double? MinQ { get; set; }
		public double? MaxQ { get; set; }
		public double? VoltRe { get; set; }
		public double? VoltIm { get; set; }
		public double? VoltMagn { get; set; }
		public double? VoltAngle { get; set; }
	}
}
