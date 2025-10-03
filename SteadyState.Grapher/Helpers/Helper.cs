using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SteadyState.Grapher.Helpers
{
	public static class Helper
	{
		public static T? FindParentOfType<T>(this DependencyObject child) where T : DependencyObject
		{
			var parent = child;

			do
			{
				parent = VisualTreeHelper.GetParent(parent);

				if (parent is T result)
				{
					return result;
				}
			}
			while (parent != null);

			return null;
		}

		/// <summary>
		/// Получить начало и конец вектора максимальной длины.
		/// </summary>
		/// <param name="points">Точки.</param>
		/// <returns>Начало и конец вектора максимальной длины.</returns>
		public static (Point, Point) GetMaxVectorPoints(this IEnumerable<Point> points)
		{
			var pointsArray = points.ToArray();
			var maxPointStart = pointsArray[0];
			var maxPointEnd = pointsArray[1];
			var maxVectorLength = maxPointStart.DistanceTo(maxPointEnd);

			for (var i = 1; i < pointsArray.Length - 1; i++)
			{
				var pointStart = pointsArray[i];
				var pointEnd = pointsArray[i + 1];
				var length = pointStart.DistanceTo(pointEnd);
				if (length <= maxVectorLength) continue;
				maxVectorLength = length;
				maxPointStart = pointStart;
				maxPointEnd = pointEnd;
			}

			return (maxPointStart, maxPointEnd);
		}

		/// <summary>
		/// Получает дистанцию между двумя точками.
		/// </summary>
		/// <param name="source">Первая точка.</param>
		/// <param name="other">Вторая точка.</param>
		/// <returns>Дистанция между двумя точками.</returns>
		public static double DistanceTo(this Point source, Point other)
		{
			var deltaX = source.X - other.X;
			var deltaY = source.Y - other.Y;
			return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
		}

		/// <summary>
		/// Получает точку по центру между двумя точками.
		/// </summary>
		/// <param name="source">Первая точка.</param>
		/// <param name="other">Вторая точка.</param>
		/// <returns>Центральная точка между двумя точками.</returns>
		public static Point MidPointTo(this Point source, Point other)
		{
			return new Point((source.X + other.X) / 2, (source.Y + other.Y) / 2);
		}

		/// <summary>
		/// Получает угол наклона прямой между двумя точками.
		/// </summary>
		/// <param name="source">Первая точка.</param>
		/// <param name="other">Вторая точка.</param>
		/// <returns>Угол наклона прямой между двумя точками.</returns>
		public static double AngleTo(this Point source, Point other)
		{
			return Math.Atan2(other.Y - source.Y, other.X - source.X) * 180 / Math.PI;
		}

		/// <summary>
		/// Получает точку на заданном расстоянии от другой исходной точки.
		/// </summary>
		/// <param name="source">Первая точка.</param>
		/// <param name="other">Вторая точка.</param>
		/// <param name="distance">Расстояние.</param>
		/// <returns>Точка на заданном расстоянии от другой исходной точки.</returns>
		public static Point PointAtDistance(this Point source, Point other, double distance)
		{
			var vector = Point.Subtract(other, source);
			vector.Normalize();

			return new Point(source.X + vector.X * distance, source.Y + vector.Y * distance);
		}

		/// <summary>
		/// Получить размер прямоугольника, описывающего все точки.
		/// </summary>
		/// <param name="points">Точки.</param>
		/// <param name="offset">Смещение.</param>
		/// <returns>Размер прямоугольника, описывающего все точки.</returns>
		public static Rect GetRectangleSize(this IEnumerable<Point> points, double offset = 0)
		{
			var pointsArray = points.ToArray();
			var minX = pointsArray.Min(p => p.X);
			var minY = pointsArray.Min(p => p.Y);
			var maxX = pointsArray.Max(p => p.X);
			var maxY = pointsArray.Max(p => p.Y);

			return new Rect(minX - offset, minY - offset, maxX - minX + 2 * offset, maxY - minY + 2 * offset);
		}
	}
}
