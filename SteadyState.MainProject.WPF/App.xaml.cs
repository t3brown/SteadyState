using System.Linq;
using HandyControl.Tools;
using System.Windows;
using System.Windows.Media;
using System.Xml.Schema;

namespace SteadyState.MainProject.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			ConfigHelper.Instance.SetLang("ru");

			var res = (SolidColorBrush)Current.Resources["PrimaryBrush"];
			var basic = res.Color;


			var lighten = Color.FromArgb((byte)255,
				(byte)(basic.R + ((255 - basic.R) / 3)),
				(byte)(basic.G + ((255 - basic.G) / 3)),
				(byte)(basic.B + ((255 - basic.B) / 3)));

			var lightenRes = new SolidColorBrush(lighten);

			Current.Resources.Add("LightPrimaryBrush", lightenRes);

			base.OnStartup(e);
		}

		private Color RedistributeRgb(Color color)
		{
			var array = new[] { color.R, color.G, color.B };
			var m = array.Max();
			var threshold = 255.999;

			//if (m <= threshold)
			//{
			//	return new Color()
			//	{
			//		R = color.R,
			//		G = color.G,
			//		B = color.B,
			//	};
			//}

			var total = color.R + color.G + color.B;

			//if (total >= 3 * threshold)
			//{
			//	return new Color()
			//	{
			//		R = (byte)threshold,
			//		G = (byte)threshold,
			//		B = (byte)threshold,
			//		A = 255,
			//	};
			//}

			var x = (3 * threshold - total) / (3 * m - total);
			var gray = threshold - x * m;

			return new Color()
			{
				R = (byte)(gray + x * color.R),
				G = (byte)(gray + x * color.G),
				B = (byte)(gray + x * color.B),
				A = 255,
			};
		}
	}
}
