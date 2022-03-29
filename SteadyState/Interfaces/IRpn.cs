using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteadyState.Interfaces
{
    public interface IRpn
    {
        public string Name { get; set; }
        public byte StepMax { get; set; }
        public double StepRpn { get; set; }
        public byte Step { get; set; }
    }
}
