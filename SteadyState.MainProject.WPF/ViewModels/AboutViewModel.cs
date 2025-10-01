using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using SteadyState.MainProject.WPF.Commands;
using MessageBox = HandyControl.Controls.MessageBox;
using System.ComponentModel;
using SteadyState.MainProject.WPF.Views;
using SteadyState.MainProject.WPF.Services.VersionService;
using SteadyState.MainProject.WPF.Services.VersionInfrastructure.UpdateFeed;

namespace SteadyState.MainProject.WPF.ViewModels
{
	public class AboutViewModel : ViewModelBase
	{
		#region приватные поля

		private static AboutWindow? _aboutWindow;

		#endregion

		#region свойства

		/// <summary>
		/// Авторские права.
		/// </summary>
		public static readonly string? CopyRight = FileVersionInfo.GetVersionInfo(VersionService.ApplicationPath).LegalCopyright;

		/// <summary>
		/// Версия + версия .net.
		/// </summary>
		public string Version => GetVersion();

		/// <summary>
		/// Название программы.
		/// </summary>
		public string? AssemblyName => Assembly.GetExecutingAssembly().GetName().Name;

		#endregion

		#region приватные методы

		#region получение версии

		/// <summary>
		/// Формирует строку с версией программы.
		/// </summary>
		/// <returns></returns>
		private static string GetVersion()
		{

#if NET6_0
			const string netVersion = ".NET 6.0";
#endif

			return $"v{VersionService.CurrentVersion} {netVersion}";
		}

		#endregion

		#region обработка уведомлений

		/// <summary>
		/// Уведомляет об актуальной версии программы.
		/// </summary>
		private static void NotifyActualVersion()
		{
			MessageBox.Show("Установлена актуальная версия программы.",
				"Информация о версии",
				MessageBoxButton.OK,
				MessageBoxImage.Information);
		}

		/// <summary>
		/// Уведомляет о наличи обновления.
		/// Запускает обновление, если это нужно.
		/// </summary>
		private static void NotifyAvailabilityUpdate(UpdateInfo info)
		{
			if (MessageBox.Show("Обнаружена новая версия программы. Вы хотите обновить?",
					"Разрешение на обновление",
					MessageBoxButton.YesNo,
					MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				_aboutWindow?.Close();

				VersionService.UpdateProgramAsync(info, (message) => MessageBox.Warning(message));
			}
		}

		/// <summary>
		/// Уведомляет об отсутствии интернета.
		/// </summary>
		private static void NotifyLackInternet()
		{
			MessageBox.Show("Не удалось получить обновление.",
				"Сбой подключения",
				MessageBoxButton.OK,
				MessageBoxImage.Error);
		}

		#endregion

		#endregion

		#region команды

		#region проверить наличие обновления

		public ICommand CheckUpdateCommand { get; } = new RelayCommand(OnCheckUpdateCommand);

		private static void OnCheckUpdateCommand(object parameter)
		{
			VersionService.GetActualVersionAsync(NotifyActualVersion, NotifyAvailabilityUpdate, NotifyLackInternet);

			if (parameter is AboutWindow aboutWindow)
			{
				_aboutWindow = aboutWindow;
			}
		}

		#endregion

		#region при закрытии окнка

		public ICommand WindowClosingCommand { get; } = new RelayCommand(OnWindowClosingCommandExecute);

		private static void OnWindowClosingCommandExecute(object obj)
		{
			if (obj is CancelEventArgs e)
			{

			}
		}

		#endregion

		#endregion
	}
}
