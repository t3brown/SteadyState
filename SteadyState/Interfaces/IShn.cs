using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteadyState.Interfaces
{
    public interface IShn
    {
        public string Name { get; set; }

        public double A0 { get; set; }

        public double A1 { get; set; }

        public double A2 { get; set; }

        public double B0 { get; set; }

        public double B1 { get; set; }

        public double B2 { get; set; }
    }
}
