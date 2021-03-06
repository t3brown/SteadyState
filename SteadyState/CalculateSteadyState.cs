using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SteadyState.Interfaces;
using SteadyState.Models;
using static SteadyState.MatrixOperation;

namespace SteadyState
{
    public static class CalculateSteadyState
    {
        internal static int n;
        internal static int _count;
        internal static float _eps;
        internal static double[] reU;
        internal static double[] imU;
        internal static double[] P;
        internal static double[] Q;
        internal static double[,] g;
        internal static double[,] b;
        internal static double[,] X;
        internal static int[] refIndices;

        internal static IEnumerable<IVertex> _vertices;
        internal static IEnumerable<IEdge> _edges;

        internal static IList<IVertex> vertices;
        internal static IList<IEdge> edges;



        public static void SetCollections(IEnumerable<IVertex> vertices, IEnumerable<IEdge> edges)
        {
            _vertices = vertices;
            _edges = edges;
        }

        public static void Calculate(float eps, int count = 100)
        {
	        vertices = _vertices.Where(a => a.IsConnected).ToList();
	        edges = _edges.Where(a => a.IsConnected).ToList();

	        var basic = vertices.FirstOrDefault(o => o.IsBasic);
            if (basic is null)
            {
                return; //если отсутсвует базисный узел
            }
            vertices.Remove(basic);
            vertices.Add(basic);
            _count = count;
            _eps = eps;
            n = vertices.Count;
            P = new double[n];
            Q = new double[n];
            (g, b) = CreateVertexConductivityMatrix();
            X = new double[2 * (n - 1),1]; //матрица-стобец X - матрица-столбец искомых узловых напряжений
            reU = new double[n]; //активная часть узловых напряжений
            reU[n - 1] = (double)vertices[n - 1].VoltNom; //активная часть напряжения базисного узла = const
            imU = new double[n]; //реактиная часть узловых напряжений 
            imU[n - 1] = 0; //реактивная часть напряжения базисного узла = const
            refIndices = new int[vertices.Count(a => a.VoltSus != null)];
            int _i = 0;
            for (int i = 0; i < 2 * (n - 1); i++) //для первой итерации присваются приближенные значения
            {
                if (i < n - 1)
                {
                    var vertex = vertices[i];
                    P[i] = vertex.PowerRe ?? 0;
                    Q[i] = vertex.PowerIm ?? 0;
                    if (vertex.VoltSus != null)
                    {
                        X[i,0] = (double)vertex.VoltSus;
                        vertex.MinQ ??= double.NegativeInfinity;
                        vertex.MaxQ ??= double.PositiveInfinity;
                        refIndices[_i] = vertices.IndexOf(vertex);
                        _i++;
                    }
                    else
                        X[i,0] = (double)vertex.VoltNom;
                    reU[i] = X[i, 0];
                }
                if (i >= n - 1)
                    imU[i - (n - 1)] = X[i, 0];
            }
            Newton.FindRoots();
            for (int i = 0; i < n; i++)
            {
                vertices[i].VoltRe = reU[i];
                vertices[i].VoltIm = imU[i];
            }
        }
        private static double CalculatePowerImGen(int i)
        {
            double A = b[i, i] * Math.Sqrt(reU[i] * reU[i] + imU[i] * imU[i]) * Math.Sqrt(reU[i] * reU[i] + imU[i] * imU[i]);
            double B = 0;
            double C = 0;
            for (int j = 0; j < n; j++)
                if (j != i)
                    B += b[i, j] * (reU[i] * reU[j] + imU[i] * imU[j]);
            for (int j = 0; j < n; j++)
                if (j != i)
                    C += g[i, j] * (reU[i] * imU[j] - imU[i] * reU[j]);
            return A + B + C;
        }
        private static (double?,double?) CalcCoeff(IEdge edge)
        {
            double? reCoeff, imCoeff;
            if (edge.U1 != null && edge.U2 != null)
            {
                double Magn = 0;
                double Phase = (double)(edge.Angle != null ? edge.Angle * Math.PI / 180 : 0);
                if (edge.Rpn1 == null && edge.Rpn2 == null)
                {
                    Magn = (double)(edge.U1 / edge.U2);
                }
                if (edge.Rpn1 != null && edge.Rpn2 == null)
                {
                    var rpn1 = edge.Rpn1;
                    Magn = (double)((edge.U1 + rpn1.Step * (rpn1.StepRpn / 100) * edge.U1) / edge.U2);
                }
                else if (edge.Rpn1 == null && edge.Rpn2 != null)
                {
                    var rpn2 = edge.Rpn2;
                    Magn = (double)(edge.U1 / (edge.U2 + rpn2.Step * (rpn2.StepRpn / 100) * edge.U2));
                }
                else if (edge.Rpn1 != null && edge.Rpn2 != null)
                {
                    var rpn1 = edge.Rpn1;
                    var rpn2 = edge.Rpn2;
                    Magn = (double)((edge.U1 + (rpn1.Step * (rpn1.StepRpn / 100) * edge.U1)) / (edge.U2 + (rpn2.Step * (rpn2.StepRpn / 100) * edge.U2)));
                }
                reCoeff = Magn * Math.Cos(Phase);
                imCoeff = Magn * Math.Sin(Phase);
            }
            else
            {
                reCoeff = null; imCoeff = null;
            }
            return (reCoeff, imCoeff);
        }
        /// <summary>
        /// Возвращает матрицу собсвтенных и взаимных узловых проводимостей.
        /// </summary>
        private static (double[,], double[,]) CreateVertexConductivityMatrix()
        {
            Complex[,] matrix = new Complex[vertices.Count, edges.Count];
            Complex[,] matrixT = new Complex[edges.Count, vertices.Count];
            Complex[,] Z = new Complex[edges.Count, edges.Count];
            Complex[,] B = new Complex[edges.Count, edges.Count];
            foreach (var edge in edges)
            {
                int j = edges.IndexOf(edge);
                int i = vertices.IndexOf(edge.V1);
                matrix[i, j] = 1;
                matrixT[j, i] = 1;
                i = vertices.IndexOf(edge.V2);
                (double? reCoef, double? imCoef) = CalcCoeff(edge);
                if (reCoef != null || imCoef != null)
                {
                    matrix[i, j] = new Complex((double)-reCoef, (double)-imCoef);
                    matrixT[j, i] = new Complex((double)-reCoef, (double)imCoef);
                }
                else
                {
                    matrix[i, j] = -1;
                    matrixT[j, i] = -1;
                }

                Z[j, j] = new Complex(edge.R != null ? (double) edge.R : 0.0000001,
                    edge.X != null ? (double) edge.X : 0.0000001);
                B[j, j] = new Complex(0, edge.B != null ? (double) (edge.B / 1000000) : 0);
            }

            Complex[,] Y = GaussJordan(Z);
            Complex[,] By = Multiplication(Multiplication(matrix, B), matrixT);
            var Yy = Multiplication(Multiplication(matrix, Y), matrixT);

            int row = Yy.GetLength(0), col = Yy.GetLength(1);
            double[,] g = new double[row, col];
            double[,] b = new double[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (i == j)
                    {
                        g[i, j] = Yy[i, j].Real + By[i, i].Real / 2;
                        b[i, j] = Yy[i, j].Imaginary + By[i, i].Imaginary / 2;
                    }
                    else
                    {
                        g[i, j] = Yy[i, j].Real;
                        b[i, j] = Yy[i, j].Imaginary;
                    }
                }
            }
            return (g, b);
        }
        /// <summary>
        /// Пересчитывает мощность нагрузки в соответствии с СХН
        /// </summary>
        internal static void CalculatePowerShn()
        {
            for (int i = 0; i < n; i++)
            {
                var vertex = vertices[i];
                if (vertex.Shn != null)
                {
                    IShn shn = vertex.Shn;
                    double voltage = Math.Sqrt(reU[i] * reU[i] + imU[i] * imU[i]);
                    vertex.PowerRe = (double)(vertex.PowerRe * (shn.A0 + shn.A1 * (voltage / vertex.VoltNom) + shn.A2 * (voltage / vertex.VoltNom) * (voltage / vertex.VoltNom)));
                    vertex.PowerIm = (double)(vertex.PowerIm * (shn.B0 + shn.B1 * (voltage / vertex.VoltNom) + shn.B2 * (voltage / vertex.VoltNom) * (voltage / vertex.VoltNom)));
                }
            }
        }
        /// <summary>
        /// Вычеляет мощность генерации опорных узлов
        /// </summary>
        internal static void CalculatePowerGen()
        {
            for (int i = 0; i < refIndices.Length; i++)
            {
                var vertex = vertices[refIndices[i]];
                double genQ = CalculatePowerImGen(i);
                if (genQ < vertex.MinQ)
                {
                    genQ = (double)vertex.MinQ;
                    refIndices[i] = -1;
                }
                else if (genQ > vertex.MaxQ)
                {
                    genQ = (double)vertex.MaxQ;
                    refIndices[i] = -1;
                }
                Q[i] = genQ;
            }
        }
        /// <summary>
        /// Матрица столбец небаланса мощностей.
        /// </summary>
        internal static double[,] W()
        {
            double[] Wp = new double[n];
            double[] Ap = new double[n];
            double[] Bp = new double[n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    if (i != j)
                    {
                        Ap[i] += g[i, j] * (reU[i] * reU[j] + imU[i] * imU[j]);
                        Bp[i] += b[i, j] * (reU[i] * imU[j] - imU[i] * reU[j]);
                    }

                if (i < n - 1)
                    Wp[i] = -P[i] - g[i, i] * (Math.Pow(reU[i], 2) + Math.Pow(imU[i], 2)) - Ap[i] + Bp[i];
            }

            double[] Wq = new double[n];
            double[] Aq = new double[n];
            double[] Bq = new double[n];
            for (int i = 0; i < n; i++)
            {
                if (refIndices.Contains(i))
                {
                    var ver = vertices[i];
                    double volt = (double) ver.VoltSus;
                    Wq[i] = reU[i] * reU[i] + imU[i] * imU[i] - volt * volt;
                }
                else
                {
                    for (int j = 0; j < n; j++)
                        if (i != j)
                        {
                            Aq[i] += b[i, j] * (reU[i] * reU[j] + imU[i] * imU[j]);
                            Bq[i] += g[i, j] * (reU[i] * imU[j] - imU[i] * reU[j]);
                        }

                    if (i < n - 1)
                        Wq[i] = -Q[i] + b[i, i] * (Math.Pow(reU[i], 2) + Math.Pow(imU[i], 2)) + Aq[i] + Bq[i];
                }
            }

            double[,] W = new double[2 * (n - 1), 1];
            for (int i = 0; i < n - 1; i++)
                W[i, 0] = Wp[i];
            for (int i = n - 1; i < 2 * (n - 1); i++)
                W[i, 0] = Wq[i - (n - 1)];
            return W;
        }

        //private static readonly string[] VertexPropertyNames = new string[] { "VoltNom", "IsBasic" };

        //private static IVertex CopyProperties()
        //{
        //    IVertex vertex = new VertexBase();
        //    var properties = vertex.GetType().GetProperties().Where(a => )
        //}
    }
}
