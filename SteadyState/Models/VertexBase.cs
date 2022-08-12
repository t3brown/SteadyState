﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteadyState.Interfaces;

namespace SteadyState.Models
{
	internal class VertexBase : IEntity, IVertex
	{
		public Guid Id { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }
		public Guid VertexOwnerId { get; set; }
		public bool IsConnected { get; set; }
		public double? VoltNom { get; set; }
		public bool IsBasic { get; set; }
		public Guid ShnId { get; set; }
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
