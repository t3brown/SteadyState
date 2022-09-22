using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteadyState.Interfaces;

namespace SteadyState.Test.Models
{
    internal class Edge: IEdge
    {
        public bool IsConnected { get; set; }
        public int Id { get; set; }
        public bool On1 { get; set; } = true;
        public bool On2 { get; set; } = true;
        public string Title { get; set; }
        public IVertex V1 { get; set; }
        public IVertex V2 { get; set; }
        public double? R { get; set; }
        public double? X { get; set; }
        public double? B { get; set; }
        public double? U1 { get; set; }
        public double? U2 { get; set; }
        public double? Angle { get; set; }
        public IRpn Rpn1 { get; set; }
        public IRpn Rpn2 { get; set; }
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
		public Guid V1Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid V2Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid OldV1Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public IVertex OldV1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid OldV2Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public IVertex OldV2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public double? G { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid Rpn1Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid Rpn2Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		Guid IEntity.Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
