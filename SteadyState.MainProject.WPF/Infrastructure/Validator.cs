using SteadyState.Grapher.Elements;
using SteadyState.MainProject.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteadyState.Interfaces;
using HandyControl.Controls;
using HandyControl.Data;
using Newtonsoft.Json.Linq;

namespace SteadyState.MainProject.WPF.Infrastructure
{
	public static class Validator
	{
		public static bool IsConditionsNotCompleted(ICollection<Vertex> vertices, ICollection<Edge> edges,
			bool isRelative)
		{

			#region вершины

			if (vertices.Count < 1)
			{
				Growl.Error("Узлы не заданы");
				return true;
			}

			//if (vertices.FirstOrDefault(o => string.IsNullOrEmpty(o.V)) != null)
			//{
			//	Growl.Error("Не задано название узла");
			//	return true;
			//}

			if (vertices.FirstOrDefault(o => o.IsBasic) == null)
			{
				Growl.Error("Не найден базисный узел");
				return true;
			}

			if (vertices.FirstOrDefault(o => o.VoltNom == null && o.VoltSus == null) != null)
			{
				Growl.Error("Номинальные напряжения заданы не у всех узлов");
				return true;
			}

			//if (vertices.FirstOrDefault(o => !string.IsNullOrEmpty(o.SHN) && shns.FirstOrDefault(p => p.N == o.SHN) == null) != null)
			//{
			//	Growl.Error("Задана несуществующая СХН");
			//	return true;
			//}

			#endregion

			#region ветви

			if (edges.Count < 1)
			{
				Growl.Error("Ветви не заданы");
				return true;
			}

			//if (edges.FirstOrDefault(o => vertices.FirstOrDefault(p => p.V == o.V1) == null) != null)
			//{
			//	Growl.Error("Начало ветви - несуществующий узел");
			//	return true;
			//}

			//if (edges.FirstOrDefault(o => !string.IsNullOrEmpty(o.V2) && vertices.FirstOrDefault(p => p.V == o.V2) == null) != null)
			//{
			//	Growl.Error("Конец ветви - несуществующий узел");
			//	return true;
			//}

			if (edges.FirstOrDefault(o => o.R == null && o.X == null) != null)
			{
				Growl.Error("Не задано сопротивление ветви");
				return true;
			}

			if (edges.FirstOrDefault(o => o.Rpn1Id != Guid.Empty && (o.U1 == null || o.U2 == null)) != null)
			{
				Growl.Error("Задан РПН у ветви без коэффициента трансформации");
				return true;
			}

			if (edges.FirstOrDefault(o => o.Rpn2Id != Guid.Empty && (o.U1 == null || o.U2 == null)) != null)
			{
				Growl.Error("Задан РПН у ветви без коэффициента трансформации");
				return true;
			}

			//if (edges.FirstOrDefault(o => !string.IsNullOrEmpty(o.RPN2) && rpns.FirstOrDefault(p => p.N == o.RPN2) == null) != null)
			//{
			//	Growl.Error("Задано несуществующее РПН (ПБВ) на стороне вторичного напряжения");
			//	return true;
			//}

			if (isRelative && edges.FirstOrDefault(o => o.U1 != null || o.U2 != null) != null)
			{
				Growl.Error(
					"При расчете в относительных единицах невозможно задать коэффициенты трансформации");
				return true;
			}

			if (edges.FirstOrDefault(o => (o.U1 != null && o.U2 == null) || (o.U2 != null && o.U1 == null)) != null)
			{
				Growl.Error("Некорректно заданы коэффициенты трансформации");
				return true;
			}

			#endregion

			return false;
		}
	}
}
