using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteadyState.Interfaces;
using static SteadyState.CalculateSteadyState;

namespace SteadyState
{
    internal static class Jacobi
    {

        /// <summary>
        /// Возвращает матрицу dWp/dU'
        /// </summary>
        private static double[,] A()
        {
            double[,] matrix = new double[n - 1, n - 1];
            double[] tmp1 = new double[n];
            //диагональ
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    if (i != j)
                        tmp1[i] += g[i, j] * reU[j] - b[i, j] * imU[j];
                if (i < n - 1)
                    matrix[i, i] = -2 * g[i, i] * reU[i] - tmp1[i];
            }
            //остальные
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                    if (i != j)
                        matrix[i, j] = -g[i, j] * reU[i] - b[i, j] * imU[i];
            }
            return matrix;
        }
        /// <summary>
        /// Возвращает матрицу dWp/dU"
        /// </summary>
        private static double[,] B()
        {
            double[,] matrix = new double[n - 1, n - 1];
            double[] tmp1 = new double[n];
            //диагональ
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    if (i != j)
                        tmp1[i] += g[i, j] * imU[j] + b[i, j] * reU[j];
                if (i < n - 1)
                    matrix[i, i] = -2 * g[i, i] * imU[i] - tmp1[i];
            }
            //остальные
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                    if (i != j)
                        matrix[i, j] = -g[i, j] * imU[i] + b[i, j] * reU[i];
            }
            return matrix;
        }
        /// <summary>
        /// Возвращает матрицу dWq/dU'
        /// </summary>
        private static double[,] C()
        {
            double[,] matrix = new double[n - 1, n - 1];
            double[] tmp1 = new double[n];
            //диагональ
            for (int i = 0; i < n; i++)
            {
                if (refIndices.Contains(i))
                {
                    matrix[i, i] = 2 * reU[i];
                }
                else
                {
                    for (int j = 0; j < n; j++)
                        if (i != j)
                            tmp1[i] += b[i, j] * reU[j] + g[i, j] * imU[j];
                    if (i < n - 1)
                        matrix[i, i] = 2 * b[i, i] * reU[i] + tmp1[i];
                }
            }
            //остальные
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                    if (i != j)
                        matrix[i, j] = b[i, j] * reU[i] - g[i, j] * imU[i];
            }
            return matrix;
        }
        /// <summary>
        /// Возвращает матрицу dWq/dU"
        /// </summary>
        private static double[,] D()
        {
            double[,] matrix = new double[n - 1, n - 1];
            double[] tmp1 = new double[n];
            //диагональ
            for (int i = 0; i < n; i++)
            {
                if (refIndices.Contains(i))
                {
                    matrix[i, i] = 2 * imU[i];
                }
                else
                {
                    for (int j = 0; j < n; j++)
                        if (i != j)
                            tmp1[i] += b[i, j] * imU[j] - g[i, j] * reU[j];
                    if (i < n - 1)
                        matrix[i, i] = 2 * b[i, i] * imU[i] + tmp1[i];
                }
            }
            //остальные
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                    if (i != j)
                        matrix[i, j] = b[i, j] * imU[i] + g[i, j] * reU[i];
            }
            return matrix;
        }
        /// <summary>
        /// Возвращает матрицу Якоби
        /// </summary>
        internal static double[,] MatrixJacobi()
        {
            double[,][,] _J = new double[2, 2][,]; //матрица Якоби из подмассивов
            for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
                _J[i, j] = new double[n - 1, n - 1];
            _J[0, 0] = A();
            _J[0, 1] = B();
            _J[1, 0] = C();
            _J[1, 1] = D();
            double[,] J = new double[2 * (n - 1), 2 * (n - 1)]; //слияние матрицы Якоби в один массив
            for (int i = 0; i < 2 * (n - 1); i++)
            for (int j = 0; j < 2 * (n - 1); j++)
            {
                if (i < n - 1 && j < n - 1)
                    J[i, j] = _J[0, 0][i, j];
                if (i >= n - 1 && j >= n - 1)
                    J[i, j] = _J[1, 1][i - (n - 1), j - (n - 1)];
                if (i < n - 1 && j >= n - 1)
                    J[i, j] = _J[0, 1][i, j - (n - 1)];
                if (i >= n - 1 && j < n - 1)
                    J[i, j] = _J[1, 0][i - (n - 1), j];
            }
            return J;
        }
    }
}
