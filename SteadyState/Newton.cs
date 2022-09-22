using static SteadyState.MatrixOperation;
using static SteadyState.CalculateSteadyState;

namespace SteadyState
{
	internal static class Newton
	{
		private static double[,] J;
		private static double[,] A;
		private static double[,] B;
		private static double[,] C;

		internal static bool FindRoots()
		{
			for (var k = 0; k < _count; k++) //реализация итерационного метода Ньютона
			{
				CalculatePowerGen();
				CalculatePowerShn();
				J = Jacobi.MatrixJacobi(); //матрицы коэффициентов нелинейного УУН
				A = GaussJordan(J); //обратаня матрицы Якоби
				B = W(); //небаланс мощнсотей
				C = Multiplication(A, B); //продольная и поперечная части падения напряжения
				X = Difference(X, C); //уточнение напряжений узлов
				for (int i = 0; i < 2 * (n - 1); i++) //уточнение напряжений узлов для матрицы Якоби
				{
					if (i < n - 1)
						reU[i] = X[i, 0];
					if (i >= n - 1)
						imU[i - (n - 1)] = X[i, 0];
				}

				var j = 0; //индекс большего по модулю числа
				for (var i = 1; i < 2 * (n - 1); i++)
					if (Math.Abs(B[i, 0]) > Math.Abs(B[j, 0]))
						j = i;
				if (Math.Abs(B[j, 0]) < _eps) //проверка сходимости небаланса
				{
					return true;
				}
			}

			return false;
		}
	}
}
