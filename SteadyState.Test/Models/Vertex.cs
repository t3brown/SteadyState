using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteadyState.Interfaces;

namespace SteadyState.Test.Models
{
    internal class Vertex: IVertex
    {
        public int Id { get; set; }
        public bool IsConnected { get; set; }
        public string VertexName { get; set; }
        public double? VoltNom { get; set; }
        public bool IsBasic { get; set; }
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
		public bool IsGround { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public Guid ShnId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		Guid IEntity.Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
