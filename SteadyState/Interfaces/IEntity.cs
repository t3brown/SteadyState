using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteadyState.Interfaces
{
	public interface IEntity
	{
		public Guid Id { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }
	}
}
