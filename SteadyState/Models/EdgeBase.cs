using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteadyState.Interfaces;

namespace SteadyState.Models
{
	internal class EdgeBase :IEntity, IEdge
	{
		public Guid Id { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }
		public bool IsConnected { get; set; }
		public bool On1 { get; set; }
		public bool On2 { get; set; }
		public Guid V1Id { get; set; }
		public Guid V2Id { get; set; }
		public Guid OldV1Id { get; set; }
		public Guid OldV2Id { get; set; }
		public double? R { get; set; }
		public double? X { get; set; }
		public double? B { get; set; }
		public double? U1 { get; set; }
		public double? U2 { get; set; }
		public double? Angle { get; set; }
		public Guid Rpn1Id { get; set; }
		public Guid Rpn2Id { get; set; }
		public double? ReCoeff { get; set; }
		public double? ImCoeff { get; set; }
		public double? AmpRe { get; set; }
		public double? AmpIm { get; set; }
		public double? AmpMagnitude { get; set; }
		public double? AmpAngle { get; set; }
		public double? PwrStRe { get; set; }
		public double? PwrStIm { get; set; }
		public double? PwrStCh { get; set; }
		public double? PwrEndCh { get; set; }
		public double? PwrEndRe { get; set; }
		public double? PwrEndIm { get; set; }
		public double? PwrDltRe { get; set; }
		public double? PwrDltIm { get; set; }
	}
}
