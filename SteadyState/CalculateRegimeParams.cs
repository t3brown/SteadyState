using SteadyState.Interfaces;
using static SteadyState.CalculateSteadyState;
using System.Numerics;

namespace SteadyState
{
	public class CalculateRegimeParams
	{
		public static (double?, double?, double?, double?, double?) Calculate(bool isRelative)//, DoubleConverter converter)
		{
			var cf = isRelative ? 1 : 1000;

			double LRe = 0, LIm = 0, TRe = 0, TIm = 0, C = 0;

			var amp = new Complex(0, 0);

			foreach (IEdge edge in edges)
			{
				if (edge.ReCoeff != null || edge.ImCoeff != null)
				{
					amp = (new Complex((double)edge.V1.VoltRe, (double)edge.V1.VoltIm) - new Complex((double)edge.V2.VoltRe, (double)edge.V2.VoltIm) * new Complex((double)edge.ReCoeff, (double)-edge.ImCoeff)) / Math.Sqrt(3) / new Complex(edge.R != null ? (double)edge.R : 0, edge.X != null ? (double)edge.X : 0) * cf;

					edge.AmpMagnitude = amp.Magnitude;
					edge.AmpAngle = amp.Phase * 180 / Math.PI;
					edge.AmpRe = amp.Real;
					edge.AmpIm = amp.Imaginary;


					edge.PwrStRe = Math.Sqrt(3) * (edge.V1.VoltRe * edge.AmpRe + edge.V1.VoltIm * edge.AmpIm) / 1000;
					edge.PwrStIm = Math.Sqrt(3) * (edge.V1.VoltIm * edge.AmpRe - edge.V1.VoltRe * edge.AmpIm) / 1000;

					edge.PwrEndRe = Math.Sqrt(3) *
						((edge.ReCoeff * edge.AmpRe - edge.ImCoeff * edge.AmpIm) * edge.V2.VoltRe +
						(edge.ReCoeff * edge.AmpIm + edge.ImCoeff * edge.AmpRe) * edge.V2.VoltIm) / 1000;
					edge.PwrEndIm = Math.Sqrt(3) *
						((edge.ReCoeff * edge.AmpRe - edge.ImCoeff * edge.AmpIm) * edge.V2.VoltIm -
						(edge.ReCoeff * edge.AmpIm + edge.ImCoeff * edge.AmpRe) * edge.V2.VoltRe) / 1000;


					edge.PwrDltRe = 3 * edge.AmpMagnitude * edge.AmpMagnitude * edge.R / 1000000;
					edge.PwrDltIm = 3 * edge.AmpMagnitude * edge.AmpMagnitude * edge.X / 1000000;
				}
				else
				{
					amp = (new Complex((double)edge.V1.VoltRe, (double)edge.V1.VoltIm) - new Complex((double)edge.V2.VoltRe, (double)edge.V2.VoltIm)) / Math.Sqrt(3) / new Complex(edge.R != null ? (double)edge.R : 0, edge.X != null ? (double)edge.X : 0) * cf;

					edge.AmpMagnitude = amp.Magnitude;
					edge.AmpAngle = amp.Phase * 180 / Math.PI;
					edge.AmpRe = amp.Real;
					edge.AmpIm = amp.Imaginary;

					edge.PwrStCh = edge.V1.VoltMagn * edge.V1.VoltMagn * edge.B / 1000000 / 2;
					edge.PwrEndCh = edge.V2.VoltMagn * edge.V2.VoltMagn * edge.B / 1000000 / 2;

					edge.PwrStRe = Math.Sqrt(3) * (edge.V1.VoltRe * edge.AmpRe + edge.V1.VoltIm * edge.AmpIm) / cf;
					edge.PwrStIm = Math.Sqrt(3) * (edge.V1.VoltIm * edge.AmpRe - edge.V1.VoltRe * edge.AmpIm) / cf;

					edge.PwrEndRe = Math.Sqrt(3) * (edge.V2.VoltRe * edge.AmpRe + edge.V2.VoltIm * edge.AmpIm) / cf;
					edge.PwrEndIm = Math.Sqrt(3) * (edge.V2.VoltIm * edge.AmpRe - edge.V2.VoltRe * edge.AmpIm) / cf;

					edge.PwrDltRe = 3 * edge.AmpMagnitude * edge.AmpMagnitude * edge.R / cf / cf;
					edge.PwrDltIm = 3 * edge.AmpMagnitude * edge.AmpMagnitude * edge.X / cf / cf;
				}

				if (edge.V2.IsGround)
				{
					TRe += edge.PwrDltRe ?? 0; TIm += edge.PwrDltIm ?? 0;
				}

				else
				{
					LRe += edge.PwrDltRe ?? 0; LIm += edge.PwrDltIm ?? 0;
				}

				if (edge.B != null)
				{
					C += (double)edge.PwrStCh + (double)edge.PwrEndCh;
				}

			}
			return (LRe == 0 ? null : LRe, LIm == 0 ? null : LIm,
					TRe == 0 ? null : TRe, TIm == 0 ? null : TIm,
					C == 0 ? null : C);
		}
	}
}
