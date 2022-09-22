using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteadyState.Interfaces
{
	public interface IRpn: IEntity
	{
		public byte StepMax { get; set; }
		public double StepRpn { get; set; }
		public sbyte Step { get; set; }
	}
}
