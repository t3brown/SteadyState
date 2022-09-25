using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SteadyState.MainProject.WPF.Models.Update
{
	public class Internet
	{
		/// <summary>
		/// Проверка наличия интернета.
		/// </summary>
		/// <returns>true, если интернет есть, иначе - false.</returns>
		public static bool IsAccess()
		{
			try
			{
				Dns.GetHostEntry("xn----8sbad0brpbh5a2e.xn--p1ai");

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
