using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SteadyState.MainProject.WPF.Models.Update
{
	public static class VersionController
	{
		#region приватные константы

		/// <summary>
		/// Временное название скачиваемого файла.
		/// </summary>
		private const string DownloadFileName = "temp_steady-state_es";

		/// <summary>
		/// Кол-во значений версии программы (1.2.3).
		/// </summary>
		private const byte FieldCount = 3;

		/// <summary>
		/// Ссылка на информацию об актуальной версии.
		/// </summary>
		private const string DescriptorUri = "https://xn----8sbad0brpbh5a2e.xn--p1ai/SteadyStateES/descriptor.json";

		#endregion

		#region свойства

		/// <summary>
		/// Название программы
		/// </summary>
		private static string ApplicationName => $"{AppDomain.CurrentDomain.FriendlyName}.exe";

		/// <summary>
		/// Папка с программой.
		/// </summary>
		public static string CurrentDirectory => AppContext.BaseDirectory;

		/// <summary>
		/// Полный путь к временному файлу.
		/// </summary>
		public static string FullDownloadFileName => $"{CurrentDirectory}{DownloadFileName}";

		/// <summary>
		/// Путь к программе.
		/// </summary>
		public static string? ApplicationPath => @$"{CurrentDirectory}{ApplicationName}";

		/// <summary>
		/// Текущая версия программы.
		/// </summary>
		public static string? CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString(FieldCount);

		/// <summary>
		/// Формат команды в cmd на подмену файла на скаченного.
		/// </summary>
		private static string CommandFormat => "taskkill /f /im \"{0}\" && timeout /t 1 && del \"{1}\" && ren \"{2}\" \"{0}\" && \"{1}\"";

		#endregion

		#region поля

		/// <summary>
		/// Актуальная версия программы.
		/// </summary>
		private static string? _actualVersion;

		/// <summary>
		/// Ссылка на скачивание файла.
		/// </summary>
		private static string? _downloadActualVersionPath;

		#endregion

		#region делегаты

		/// <summary>
		/// Представляет метод уведомления об актуальности версии.
		/// </summary>
		public delegate void NotifyActualVersion();


		/// <summary>
		/// Представляет метод уведомления об наличии обновления.
		/// </summary>
		public delegate void NotifyAvailabilityUpdate();

		/// <summary>
		/// Представляет метод уведомления об отсутствии интернета.
		/// </summary>
		public delegate void NotifyLackInternet();

		#endregion

		#region публичные методы

		/// <summary>
		/// Получает информацию об актуальное версии.
		/// </summary>
		public static async void GetActualVersionAsync(NotifyActualVersion? notifyActualVersion,
			NotifyAvailabilityUpdate? notifyAvailabilityUpdate, NotifyLackInternet? notifyLackInternet)
		{
			if (!Internet.IsAccess())
			{
				notifyLackInternet?.Invoke();
				return;
			}

			var descriptor = await HttpResponse(DescriptorUri);

			_actualVersion = JObject.Parse(descriptor)["version"]?.ToString();
			_downloadActualVersionPath = JObject.Parse(descriptor)["downloadLink"]?.ToString();

			VersionComparison(notifyActualVersion, notifyAvailabilityUpdate);
		}

		/// <summary>
		/// Асинхронное обновление программы.
		/// </summary>
		public static async void UpdateProgramAsync()
		{
			await DeleteTempFilesAsync();

			using var client = new HttpClient();

			if (_downloadActualVersionPath != null)
			{
				await using var stream = await client.GetStreamAsync(_downloadActualVersionPath);
				await using var file = new FileStream(DownloadFileName, FileMode.CreateNew);
				{
					await stream.CopyToAsync(file);
				}
			}

			Сmd(string.Format(CommandFormat, ApplicationName, ApplicationPath, DownloadFileName));
		}

		/// <summary>
		/// Асинхронное удаление временных файлов.
		/// </summary>
		public static async Task DeleteTempFilesAsync()
		{
			await Task.Factory.StartNew(() =>
			{
				if (File.Exists(FullDownloadFileName))
				{
					File.Delete(FullDownloadFileName);
				}
			});
		}

		#endregion

		#region приватные методы

		/// <summary>
		/// Сравнивает версии.
		/// </summary>
		private static void VersionComparison(NotifyActualVersion? notifyActualVersion, NotifyAvailabilityUpdate? notifyAvailabilityUpdate)
		{
			var current = Convert.ToInt32(CurrentVersion?
				.Replace(".", string.Empty)
				.Replace(",", string.Empty));

			var actual = Convert.ToInt32(_actualVersion?
				.Replace(".", string.Empty)
				.Replace(",", string.Empty));

			if (actual == current)
			{
				notifyActualVersion?.Invoke();
			}

			else
			{
				notifyAvailabilityUpdate?.Invoke();
			}
		}

		/// <summary>
		/// Запуск командной строки и выполнение переданных команд.
		/// </summary>
		/// <param name="commands">Команды.</param>
		private static void Сmd(string commands)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "cmd",
				Arguments = $"/c {commands}",
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
			});
		}

		/// <summary>
		/// Асинхронно получает ответ от HttpClient по указанной ссылке. 
		/// </summary>
		/// <param name="path">Ссылка.</param>
		/// <returns>Результат в виде строки.</returns>
		private static async Task<string> HttpResponse(string path)
		{
			using var client = new HttpClient();
			var response = await client.GetAsync(path);

			return response.IsSuccessStatusCode
				? await response.Content.ReadAsStringAsync()
				: string.Empty;
		}

		#endregion
	}
}
