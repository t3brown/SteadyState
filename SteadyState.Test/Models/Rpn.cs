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
	    private sbyte _step;
	    public string Title { get; set; }
        public string VisualTitle { get; }
        public byte StepMax { get; set; }
        public double StepRpn { get; set; }

        sbyte IRpn.Step
        {
	        get => _step;
	        set => _step = value;
        }

        public byte Step { get; set; }
		public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
