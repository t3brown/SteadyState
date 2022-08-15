using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SteadyState
{
	internal static class MatrixOperation
	{
		#region вычетание матриц
		/// <summary>
		/// Возвращает массив чисел типа double[,], находя разность a и b.
		/// </summary>
		internal static double[,] Difference(double[,] a, double[,] b)
		{
			double[,] r = new double[a.GetLength(0), a.GetLength(1)];
			for (int i = 0; i < a.GetLength(0); i++)
				for (int j = 0; j < b.GetLength(1); j++)
					r[i, j] = a[i, j] - b[i, j];
			return r;
		}
		#endregion

		#region умножение матриц
		/// <summary>
		/// Возвращает массив чисел типа complex[,], перемножая массивы a и b.
		/// </summary>
		internal static Complex[,] Multiplication(Complex[,] a, Complex[,] b)
		{
			if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
			Complex[,] r = new Complex[a.GetLength(0), b.GetLength(1)];
			for (int i = 0; i < a.GetLength(0); i++)
			{
				for (int j = 0; j < b.GetLength(1); j++)
				{
					for (int k = 0; k < b.GetLength(0); k++)
					{
						r[i, j] += a[i, k] * b[k, j];
					}
				}
			}
			return r;
		}
		/// <summary>
		/// Возвращает массив чисел типа double[,], перемножая массивы a и b.
		/// </summary>
		internal static double[,] Multiplication(double[,] a, double[,] b)
		{
			if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
			double[,] r = new double[a.GetLength(0), b.GetLength(1)];
			for (int i = 0; i < a.GetLength(0); i++)
			{
				for (int j = 0; j < b.GetLength(1); j++)
				{
					for (int k = 0; k < b.GetLength(0); k++)
					{
						r[i, j] += a[i, k] * b[k, j];
					}
				}
			}
			return r;
		}
		#endregion

		#region Метод вычисления обратной матрицы Гаусса-Жордана
		/// <summary>
		/// Метод вычисления обратной матрицы Гаусса-Жордана.
		/// </summary>
		internal static Complex[,] GaussJordan(Complex[,] Matrix)
		{
			int n = Matrix.GetLength(0); //Размерность начальной матрицы

			Complex[,] xirtaM = new Complex[n, n]; //Единичная матрица (искомая обратная матрица)
			for (int i = 0; i < n; i++)
				xirtaM[i, i] = 1;

			Complex[,] Matrix_Big = new Complex[n, 2 * n]; //Общая матрица, получаемая скреплением Начальной матрицы и единичной
			for (int i = 0; i < n; i++)
				for (int j = 0; j < n; j++)
				{
					Matrix_Big[i, j] = Matrix[i, j];
					Matrix_Big[i, j + n] = xirtaM[i, j];
				}
			//Прямой ход (Зануление нижнего левого угла)
			for (int k = 0; k < n; k++) //k-номер строки
			{
				for (int i = 0; i < 2 * n; i++) //i-номер столбца
					Matrix_Big[k, i] = Matrix_Big[k, i] / Matrix[k, k]; //Деление k-строки на первый член !=0 для преобразования его в единицу
				for (int i = k + 1; i < n; i++) //i-номер следующей строки после k
				{
					Complex K = Matrix_Big[i, k] / Matrix_Big[k, k]; //Коэффициент
					for (int j = 0; j < 2 * n; j++) //j-номер столбца следующей строки после k
						Matrix_Big[i, j] = Matrix_Big[i, j] - Matrix_Big[k, j] * K; //Зануление элементов матрицы ниже первого члена, преобразованного в единицу
				}
				for (int i = 0; i < n; i++) //Обновление, внесение изменений в начальную матрицу
					for (int j = 0; j < n; j++)
						Matrix[i, j] = Matrix_Big[i, j];
			}
			//Обратный ход (Зануление верхнего правого угла)
			for (int k = n - 1; k > -1; k--) //k-номер строки
			{
				for (int i = 2 * n - 1; i > -1; i--) //i-номер столбца
					Matrix_Big[k, i] = Matrix_Big[k, i] / Matrix[k, k];
				for (int i = k - 1; i > -1; i--) //i-номер следующей строки после k
				{
					Complex K = Matrix_Big[i, k] / Matrix_Big[k, k];
					for (int j = 2 * n - 1; j > -1; j--) //j-номер столбца следующей строки после k
						Matrix_Big[i, j] = Matrix_Big[i, j] - Matrix_Big[k, j] * K;
				}
			}
			//Отделяем от общей матрицы
			for (int i = 0; i < n; i++)
				for (int j = 0; j < n; j++)
					xirtaM[i, j] = Matrix_Big[i, j + n];
			return xirtaM;
		}
		/// <summary>
		/// Метод вычисления обратной матрицы Гаусса-Жордана.
		/// </summary>
		internal static double[,] GaussJordan(double[,] Matrix)
		{
			int n = Matrix.GetLength(0); //Размерность начальной матрицы

			double[,] xirtaM = new double[n, n]; //Единичная матрица (искомая обратная матрица)
			for (int i = 0; i < n; i++)
				xirtaM[i, i] = 1;

			double[,] Matrix_Big = new double[n, 2 * n]; //Общая матрица, получаемая скреплением Начальной матрицы и единичной
			for (int i = 0; i < n; i++)
				for (int j = 0; j < n; j++)
				{
					Matrix_Big[i, j] = Matrix[i, j];
					Matrix_Big[i, j + n] = xirtaM[i, j];
				}
			//Прямой ход (Зануление нижнего левого угла)
			for (int k = 0; k < n; k++) //k-номер строки
			{
				for (int i = 0; i < 2 * n; i++) //i-номер столбца
					Matrix_Big[k, i] = Matrix_Big[k, i] / Matrix[k, k]; //Деление k-строки на первый член !=0 для преобразования его в единицу
				for (int i = k + 1; i < n; i++) //i-номер следующей строки после k
				{
					double K = Matrix_Big[i, k] / Matrix_Big[k, k]; //Коэффициент
					for (int j = 0; j < 2 * n; j++) //j-номер столбца следующей строки после k
						Matrix_Big[i, j] = Matrix_Big[i, j] - Matrix_Big[k, j] * K; //Зануление элементов матрицы ниже первого члена, преобразованного в единицу
				}
				for (int i = 0; i < n; i++) //Обновление, внесение изменений в начальную матрицу
					for (int j = 0; j < n; j++)
						Matrix[i, j] = Matrix_Big[i, j];
			}
			//Обратный ход (Зануление верхнего правого угла)
			for (int k = n - 1; k > -1; k--) //k-номер строки
			{
				for (int i = 2 * n - 1; i > -1; i--) //i-номер столбца
					Matrix_Big[k, i] = Matrix_Big[k, i] / Matrix[k, k];
				for (int i = k - 1; i > -1; i--) //i-номер следующей строки после k
				{
					double K = Matrix_Big[i, k] / Matrix_Big[k, k];
					for (int j = 2 * n - 1; j > -1; j--) //j-номер столбца следующей строки после k
						Matrix_Big[i, j] = Matrix_Big[i, j] - Matrix_Big[k, j] * K;
				}
			}
			//Отделяем от общей матрицы
			for (int i = 0; i < n; i++)
				for (int j = 0; j < n; j++)
					xirtaM[i, j] = Matrix_Big[i, j + n];
			return xirtaM;
		}
		#endregion
	}
}
