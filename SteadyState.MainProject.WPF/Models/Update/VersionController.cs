using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json.Linq;

namespace SteadyState.MainProject.WPF.Models.Update
{
	public static class VersionController
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

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
		private static string ApplicationName => $"{Process.GetCurrentProcess().ProcessName}.exe";

		/// <summary>
		/// Папка с программой.
		/// </summary>
		public static string CurrentDirectory => AppContext.BaseDirectory;

		/// <summary>
		/// Путь к папке, в которой хранится временно загруженный файл.
		/// </summary>
		public static readonly string DownloadFilePath = $"{Path.GetTempPath()}";

		/// <summary>
		/// Полный путь к временному файлу.
		/// </summary>
		public static string FullDownloadFileName => $"{DownloadFilePath}{DownloadFileName}";

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
		private static string CommandFormat => "taskkill /f /im \"{0}\" && timeout /t 1 && del \"{1}{0}\" && move \"{2}{3}\" \"{1}\" && ren \"{1}{3}\" \"{0}\" && \"{1}{0}\"";

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

		#region события

		public static event Action<int>? HttpReceiveProgress;

		#endregion

		#region публичные методы

		/// <summary>
		/// Получает информацию об актуальное версии.
		/// </summary>
		public static async void GetActualVersionAsync(NotifyActualVersion? notifyActualVersion,
			NotifyAvailabilityUpdate? notifyAvailabilityUpdate, NotifyLackInternet? notifyLackInternet)
		{
			try
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
			catch (Exception e)
			{
				Log.Warn($"Не удалось установить соединение с удаленным сервером для получения обновления: \"{e.Message}\"");
			}
		}

		/// <summary>
		/// Асинхронное обновление программы.
		/// </summary>
		public static async void UpdateProgramAsync()
		{
			Log.Info("Выполняется обновление программы");
			await DeleteTempFilesAsync();

			var progressMessageHandler = new ProgressMessageHandler(new HttpClientHandler());
			progressMessageHandler.HttpReceiveProgress += (sender, e) =>
			{
				HttpReceiveProgress?.Invoke(e.ProgressPercentage);
			};

			Log.Info($"Ссылка на скачивание файла: {_downloadActualVersionPath}");

			using var client = new HttpClient(progressMessageHandler);
			if (_downloadActualVersionPath != null)
			{
				await using var stream = await client.GetStreamAsync(_downloadActualVersionPath);
				await using var file = new FileStream(FullDownloadFileName, FileMode.CreateNew);
				{
					await stream.CopyToAsync(file);
				}
			}
			
			var cmdString = string.Format(CommandFormat, ApplicationName, CurrentDirectory, DownloadFilePath, DownloadFileName);
			Log.Info($"Команда актуализации исполняемого файла: {cmdString}");
			Сmd(cmdString);
		}

		/// <summary>
		/// Асинхронное удаление временных файлов.
		/// </summary>
		public static async Task DeleteTempFilesAsync()
		{
			await Task.Factory.StartNew(() =>
			{
				var file = $"{CurrentDirectory}{DownloadFileName}";
				Log.Info("Поиск временных файлов");
				Log.Info($"Путь к временному файлу в папке TEMP: {FullDownloadFileName}");
				Log.Info($"Путь к временному файлу в корневой папке: {file}");

				if (File.Exists(FullDownloadFileName))
				{
					File.Delete(FullDownloadFileName);
					Log.Info($"Был обнаружен и удален файл в папке TEMP: {FullDownloadFileName}");
				}

				if (File.Exists(file))
				{
					File.Delete(file);
					Log.Info($"Был обнаружен и удален файл в корневой папке: {file}");
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
			Log.Info("Выполняется проверка наличия обновлений");
			Log.Info($"Текущая версия программы: {CurrentVersion}");
			Log.Info($"Актуальная версия программы: {_actualVersion}");

			var current = Convert.ToInt32(CurrentVersion?
				.Replace(".", string.Empty)
				.Replace(",", string.Empty));

			var actual = Convert.ToInt32(_actualVersion?
				.Replace(".", string.Empty)
				.Replace(",", string.Empty));

			if (actual <= current)
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
