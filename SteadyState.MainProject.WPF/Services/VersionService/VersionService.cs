using log4net;
using SteadyState.MainProject.WPF.Services.VersionInfrastructure.UpdateFeed;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Handlers;
using System;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace SteadyState.MainProject.WPF.Services.VersionService
{
	internal class VersionService
	{
		private const string GlobalUpdateMutexName = @"Global\SteadyStateES_Update_Lock";

		private const string LocalUpdateMutexName = @"Local\SteadyStateES_Update_Lock";

		private static readonly SemaphoreSlim _inProcessUpdateGate = new(1, 1);

		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

		private static readonly IUpdateFeed UpdateFeed = new GitHubReleaseUpdateFeed("t3brown", "SteadyState");

		private const string DownloadFileTemp = "temp_steady-state_es";

		private static readonly string DownloadFileName = $"{DownloadFileTemp}_{Guid.NewGuid()}";

		private static readonly string ApplicationName = $"{Process.GetCurrentProcess().ProcessName}.exe";

		public static readonly string CurrentDirectory = AppContext.BaseDirectory;

		public static readonly string DownloadFilePath = $"{Path.GetTempPath()}";

		public static readonly string FullDownloadFileName = $"{DownloadFilePath}{DownloadFileName}";

		public static readonly string ApplicationPath = @$"{CurrentDirectory}{ApplicationName}";

		public static readonly string? CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);

		private const string CommandFormat = "taskkill /f /im \"{0}\" && timeout /t 1 && del \"{1}{0}\" && move \"{2}{3}\" \"{1}\" && ren \"{1}{3}\" \"{0}\" && \"{1}{0}\"";

		public static Action<int>? HttpDownloadFileProgress;

		public static async void GetActualVersionAsync(
			Action? notifyActualVersion,
			Action<UpdateInfo>? notifyAvailabilityUpdate,
			Action? notifyLackInternet)
		{
			UpdateInfo info;

			try
			{
				info = await UpdateFeed.GetLatestAsync();
			}
			catch (Exception e)
			{
				Log.Warn($"Не удалось получить обновление: \"{e.Message}\"");
				notifyLackInternet?.Invoke();
				return;
			}

			VersionComparison(info, notifyActualVersion, notifyAvailabilityUpdate);
		}

		private static void VersionComparison(
			UpdateInfo info,
			Action? notifyActualVersion,
			Action<UpdateInfo>? notifyAvailabilityUpdate)
		{
			Log.Info("Выполняется проверка наличия обновлений");
			Log.Info($"Текущая версия программы: {CurrentVersion}");
			Log.Info($"Актуальная версия программы: {info.Version}");

			if (!Version.TryParse(CurrentVersion, out var cur) ||
				!Version.TryParse(info.Version, out var act))
			{
				Log.Warn("Не удалось распарсить версии — пропуск обновления.");
				return;
			}

			if (act <= cur) notifyActualVersion?.Invoke();
			else notifyAvailabilityUpdate?.Invoke(info);
		}

		public static async void UpdateProgramAsync(UpdateInfo info, Action<string>? notifyInformation)
		{
			Mutex? mtx = null;
			bool acquired = false;

			if (!await _inProcessUpdateGate.WaitAsync(0))
			{
				Log.Info("Обновление уже выполняется в текущем процессе.");
				return;
			}

			try
			{
				try { mtx = new Mutex(false, GlobalUpdateMutexName); }
				catch (UnauthorizedAccessException) { mtx = new Mutex(false, LocalUpdateMutexName); }

				try { acquired = mtx.WaitOne(0); }
				catch (AbandonedMutexException)
				{
					Log.Warn($"Обнолвнение разрешено, но Mutex не освобожден");
					acquired = true;
				}

				if (!acquired)
				{
					Log.Info("Обновление уже выполняется в другом экземпляре.");
					notifyInformation?.Invoke("Обновление выполняется в другом экземпляре приложения.");
					return;
				}

				Log.Info("Выполняется обновление программы");
				await DeleteTempFilesAsync();

				var progressMessageHandler = new ProgressMessageHandler(new HttpClientHandler());
				progressMessageHandler.HttpReceiveProgress += (sender, e) => HttpDownloadFileProgress?.Invoke(e.ProgressPercentage);

				Log.Info($"Ссылка на скачивание файла: {info.DownloadUrl}");

				using var client = new HttpClient(progressMessageHandler);
				using var stream = await client.GetStreamAsync(info.DownloadUrl);
				using var file = new FileStream(FullDownloadFileName, FileMode.CreateNew);
				{
					await stream.CopyToAsync(file);
				}

				var cmdString = string.Format(CommandFormat, ApplicationName, CurrentDirectory, DownloadFilePath, DownloadFileName);
				Log.Info($"Команда актуализации исполняемого файла: {cmdString}");
				Сmd(cmdString);
			}
			catch (Exception ex)
			{
				Log.Error($"Ошибка при обновлении: {ex.Message}");
			} 
			finally
			{
				if (acquired)
				{
					try { mtx?.ReleaseMutex(); } catch {/* ignored */ }
				}

				_inProcessUpdateGate.Release();
				mtx?.Dispose();
			}
		}

		public static async Task DeleteAllDownloadTempsAsync(string pattern = DownloadFileTemp + "*")
		{
			await Task.Run(() =>
			{
				var directions = new[] { Path.GetTempPath(), CurrentDirectory };

				foreach (var dir in directions)
				{
					try
					{
						if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
							continue;

						var files = Directory.EnumerateFiles(dir, pattern, SearchOption.TopDirectoryOnly);

						Log.Info($"Найдено {files.Count()} временных файлов в директории: {dir}");

						foreach (var file in files)
						{
							try
							{
								File.Delete(file);
							}
							catch (IOException ex)
							{
								Log.Warn($"Не удалось удалить файл '{file}': {ex.Message}");
							}
							catch (UnauthorizedAccessException ex)
							{
								Log.Warn($"Нет доступа к удалению файла '{file}': {ex.Message}");
							}
						}
					}
					catch (Exception ex)
					{
						Log.Warn($"Ошибка при перечислении папки '{dir}': {ex.Message}");
					}
				}
			});
		}

		private static async Task DeleteTempFilesAsync()
		{
			await DeleteAllDownloadTempsAsync(DownloadFileName);
		}

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
	}
}
