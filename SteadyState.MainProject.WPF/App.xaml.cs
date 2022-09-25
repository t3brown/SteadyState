using HandyControl.Tools;
using System.Windows;

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

			base.OnStartup(e);
		}
	}
}
