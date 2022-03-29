using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteadyState.Interfaces;

namespace SteadyState.Test.Models
{
    internal class Rpn: IRpn
    {
        public string Name { get; set; }
        public byte StepMax { get; set; }
        public double StepRpn { get; set; }
        public byte Step { get; set; }
    }
}
