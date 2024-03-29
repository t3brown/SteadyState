﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using HandyControl.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;
using SteadyState.Grapher.Controls;
using SteadyState.Grapher.Elements;
using SteadyState.Interfaces;
using SteadyState.MainProject.WPF.Components;
using SteadyState.MainProject.WPF.Infrastructure;
using SteadyState.MainProject.WPF.Models;
using SteadyState.MainProject.WPF.Models.Update;
using SteadyState.MainProject.WPF.Views;
using RelayCommand = SteadyState.MainProject.WPF.Commands.RelayCommand;
using TabItem = System.Windows.Controls.TabItem;
using Window = HandyControl.Controls.Window;

namespace SteadyState.MainProject.WPF.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		#region свойства

		#region режим только для чтения

		private bool _isReadOnly;

		/// <summary>
		/// Режим только на чтение.
		/// </summary>
		public bool IsReadOnly
		{
			get => _isReadOnly;
			set
			{
				if (_isReadOnly == value) return;
				_isReadOnly = value;
				OnPropertyChanged();
			}
		}


		#endregion

		#region значение прогресс бара.

		private int _progress;

		/// <summary>
		/// Значение прогресс бара
		/// </summary>
		public int Progress
		{
			get => _progress;
			set
			{
				if (_progress == value) return;
				_progress = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region видимость панели загрузки новой версии программы.

		private bool _isDownloadPanelVisible;

		/// <summary>
		/// Видимость панели загрузки новой вервсии программы.
		/// </summary>
		public bool IsDownloadPanelVisible
		{
			get => _isDownloadPanelVisible;
			set
			{
				if (_isDownloadPanelVisible == value) return;
				_isDownloadPanelVisible = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region исходные данные

		/// <summary>
		/// Коллекция Вершин.
		/// </summary>
		public ICollection<Vertex> Vertices { get; set; }

		/// <summary>
		/// Коллекция Ветвей.
		/// </summary>
		public ICollection<Edge> Edges { get; set; }

		/// <summary>
		/// Коллекция СХН.
		/// </summary>
		public ICollection<Shn> Shns { get; set; }

		/// <summary>
		/// Коллекция РПН.
		/// </summary>
		public ICollection<Rpn> Rpns { get; set; }

		#endregion

		#region настройки

		/// <summary>
		/// Путь для сохранения настроек по умолчанию.
		/// </summary>
		public static readonly string SettingsFileName = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Steady-State ES\default_settings.json";

		/// <summary>
		/// Настройки видимости колонок.
		/// </summary>
		public EnableColumns EnableColumns { get; set; }

		/// <summary>
		/// Настройки единиц измерения.
		/// </summary>
		public Units Units { get; set; }

		/// <summary>
		/// Настройки точности отображения.
		/// </summary>
		public DisplayPrecision DisplayPrecision { get; set; }

		#region IsRelative: bool - будет ли выполняться расчет в относительных единицах

		/// <summary>
		/// Именованные единицы измерения, которые были до относительных.
		/// </summary>
		public Units? NamedUnits;

		private bool _isRelative;

		public bool IsRelative
		{
			get => _isRelative;
			set
			{
				if (value == _isRelative) return;

				if (_isRelative == false)
				{
					if (Units != null)
					{
						NamedUnits = Units.Clone() as Units;
					}
				}

				if (value)
				{
					var props = Units?.GetType().GetProperties();
					foreach (var prop in props)
					{
						//if (prop.Name == "Angle" || prop.Name == "AmpAngle" || prop.Name == "VoltAngle")
						//	continue;
						prop.SetValue(Units, 0);
					}
				}

				if (value == false)
				{
					Units?.CopyPropertiesValue(NamedUnits);
				}

				_isRelative = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region точность расчета

		private float _calculatePrecision = 0.001f;

		public float CalculatePrecision
		{
			get => _calculatePrecision;
			set
			{
				if (value == _calculatePrecision) return;
				_calculatePrecision = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#endregion

		#region редактор

		/// <summary>
		/// Выбранный элемент графического редактора.
		/// </summary>
		public CircuitElement SelectedElement
		{
			get => _selectedElement;
			set
			{
				if (Equals(value, _selectedElement)) return;
				_selectedElement = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region путь к файлу

		public string FilePath
		{
			get => _filePath;
			private set
			{
				if (value == _filePath) return;
				_filePath = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region текущая вклкад

		public object SelectedTab { get; set; }


		private int _selectedIndex;

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				_selectedIndex = value;
				OnPropertyChanged();
			}
		}

		#endregion


		#endregion

		#region команды

		#region команда запуска расчета.

		/// <summary>
		/// Запуск расчета.
		/// </summary>
		public ICommand RegimeCommand { get; }

		/// <summary>
		/// Запускает расчет.
		/// </summary>
		/// <param name="obj">Параметр команды.</param>
		private void OnRegimeCommandExecute(object obj)
		{
			if (Validator.IsConditionsNotCompleted(Vertices, Edges, IsRelative))
			{
				return;
			}

			if (CalculateSteadyState.Calculate(CalculatePrecision))
			{
				Growl.Success("Расчет выполнен успешно.");
			}
			else
			{
				Growl.Warning("Расчет не был выполнен.");
			}

			CalculateRegimeParams.Calculate(IsRelative);
		}

		#endregion

		#region новый файл

		public ICommand CreateCommand { get; }

		private void OnCreateCommandExecute(object obj)
		{
			ClearAll();
		}

		#endregion

		#region команда открытия вкладки в новом окне

		/// <summary>
		/// Открывает вкладку в новом окне.
		/// </summary>
		public ICommand OpenInNewWindowCommand { get; }

		/// <summary>
		/// Открывает вкладку в новом окне.
		/// </summary>
		/// <param name="obj">Выбранная вкладка?</param>
		private void OnOpenInNewWindowCommandExecute(object obj)
		{
			if (SelectedTab is not TabItem tab) return;

			var newTabWindow = new Window
			{
				DataContext = this,
				//Owner = Application.Current.MainWindow,
				Content = tab.Content,
				Background = Application.Current.FindResource("RegionBrush") as Brush,
			};


			tab.Visibility = Visibility.Collapsed;
			_tabVisibilities[SelectedIndex] = tab.Visibility;
			tab.Content = null;

			for (var i = 0; i < 5; i++)
			{
				SelectedIndex = i;

				if (((TabItem)SelectedTab).Visibility == Visibility.Visible)
				{
					break;
				}
			}

			newTabWindow.Tag = tab;
			newTabWindow.Closed += NewTabWindow_Closed;
			newTabWindow.Show();
		}

		/// <summary>
		/// Проверяет доступность кнопки открытия файла.
		/// </summary>
		/// <param name="obj">Выбранная вкладка?</param>
		/// <returns>true, если кол-во вкладок на форме больше одной, иначе false.</returns>
		private bool OnOpenInNewWindowCanExecuted(object obj)
		{
			return _tabVisibilities.Count(o => o == Visibility.Visible) > 1;
		}

		/// <summary>
		/// Событие закрытия окна с вкладкой.
		/// </summary>
		private void NewTabWindow_Closed(object? sender, EventArgs e)
		{
			if (sender is not Window { Tag: TabItem tab } window) return;

			tab.Visibility = Visibility.Visible;
			tab.Content = window.Content;
			tab.IsSelected = true;

			_tabVisibilities[SelectedIndex] = tab.Visibility;
		}

		#endregion

		#region команда сохранения файла

		/// <summary>
		/// Сохраняет файл.
		/// </summary>
		public ICommand SaveCommand { get; }

		/// <summary>
		/// Сохраняет файл.
		/// </summary>
		/// <param name="filePath">Путь к файлу.</param>
		private void OnSaveCommandExecute(object filePath)
		{
			var fileName = filePath as string;

			if (string.IsNullOrEmpty(fileName))
			{
				var dialog = new SaveFileDialog()
				{
					//Title = $"Сохранение проекта - {Tab.Title}",
					Filter = "Json files (*.json)|*.json",
					//InitialDirectory = Environment.CurrentDirectory,
					RestoreDirectory = true
				};
				if (dialog.ShowDialog() != true) return;

				fileName = dialog.FileName;

				//SafeFileName = dialog.SafeFileName;
			}

			using var writer = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write));

			writer.WriteLine(JsonConvert.SerializeObject(Vertices));
			writer.WriteLine(JsonConvert.SerializeObject(Edges));
			writer.WriteLine(JsonConvert.SerializeObject(Rpns));
			writer.WriteLine(JsonConvert.SerializeObject(Shns));


			writer.WriteLine(JsonConvert.SerializeObject(IsRelative));

			var isRelative = IsRelative;

			if (IsRelative)
			{
				IsRelative = false;
			}

			writer.WriteLine(JsonConvert.SerializeObject(EnableColumns));
			writer.WriteLine(JsonConvert.SerializeObject(DisplayPrecision));
			writer.WriteLine(JsonConvert.SerializeObject(Units));

			writer.WriteLine(JsonConvert.SerializeObject(_isVoltNom));
			writer.WriteLine(JsonConvert.SerializeObject(CalculatePrecision));

			IsRelative = isRelative;




			FilePath = fileName;
		}

		#endregion

		#region команда открытия файла

		/// <summary>
		/// Открывает файл.
		/// </summary>
		public ICommand OpenCommand { get; }

		/// <summary>
		/// Открывает файл.
		/// </summary>
		/// <param name="obj">Параметр команды.</param>
		private void OnOpenCommandExecute(object obj)
		{
			try
			{
				var dialog = new OpenFileDialog()
				{
					Title = "Открытие проекта",
					Filter = "Json files (*.json)|*.json",
					RestoreDirectory = true
				};

				if (dialog.ShowDialog() != true)
				{
					return;
				}

				var fileName = dialog.FileName;

				//SafeFileName = dialog.SafeFileName;

				if (!File.Exists(fileName))
				{
					return;
				}

				using var reader = File.OpenText(fileName);

				var vertices = JsonConvert
					.DeserializeObject<ObservableCollection<Vertex>>(reader.ReadLine() ?? string.Empty);

				var edges = JsonConvert
					.DeserializeObject<ObservableCollection<Edge>>(reader.ReadLine() ?? string.Empty);

				var rpns = JsonConvert
					.DeserializeObject<ObservableCollection<Rpn>>(reader.ReadLine() ?? string.Empty);

				var shns = JsonConvert
					.DeserializeObject<ObservableCollection<Shn>>(reader.ReadLine() ?? string.Empty);

				var isRelative = JsonConvert
					.DeserializeObject(reader.ReadLine() ?? string.Empty);

				var enableColumns = JsonConvert
					.DeserializeObject<EnableColumns>(reader.ReadLine() ?? string.Empty);

				var displayPrecision = JsonConvert
					.DeserializeObject<DisplayPrecision>(reader.ReadLine() ?? string.Empty);

				var units = JsonConvert
					.DeserializeObject<Units>(reader.ReadLine() ?? string.Empty);

				var isVoltNom = JsonConvert
					.DeserializeObject(reader.ReadLine() ?? string.Empty);

				CalculatePrecision = JsonConvert.DeserializeObject<float>(reader.ReadLine() ?? "0.001");

				ClearAll();

				if (shns != null)
				{
					foreach (var shn in shns)
					{
						Shns.Add(shn);
					}
				}

				if (rpns != null)
				{
					foreach (var rpn in rpns)
					{
						Rpns.Add(rpn);
					}
				}

				if (vertices != null)
				{
					foreach (var vertex in vertices)
					{
						if (vertex.ShnId != Guid.Empty)
						{
							vertex.Shn = Shns.FirstOrDefault(o => o.Id == vertex.ShnId)!;
						}

						Vertices.Add(vertex);
					}
				}

				if (edges != null)
				{
					foreach (var edge in edges)
					{
						if (edge.V1Id != Guid.Empty)
						{
							edge.V1 = Vertices.FirstOrDefault(o => o.Id == edge.V1Id)!;
						}

						if (edge.V2Id != Guid.Empty)
						{
							edge.V2 = Vertices.FirstOrDefault(o => o.Id == edge.V2Id)!;
						}

						if (edge.OldV1Id != Guid.Empty)
						{
							edge.OldV1 = Vertices.FirstOrDefault(o => o.Id == edge.OldV1Id)!;
						}

						if (edge.OldV2Id != Guid.Empty)
						{
							edge.OldV2 = Vertices.FirstOrDefault(o => o.Id == edge.OldV2Id)!;
						}

						if (edge.Rpn1Id != Guid.Empty)
						{
							edge.Rpn1 = Rpns.FirstOrDefault(o => o.Id == edge.Rpn1Id)!;
						}

						if (edge.Rpn2Id != Guid.Empty)
						{
							edge.Rpn2 = Rpns.FirstOrDefault(o => o.Id == edge.Rpn2Id)!;
						}

						Edges.Add(edge);
					}
				}

				if (units != null)
				{
					Units.CopyPropertiesValue(units);
				}

				if (isRelative != null)
				{
					IsRelative = (bool)isRelative;
				}

				if (enableColumns != null)
				{
					EnableColumns.CopyPropertiesValue(enableColumns);
				}

				if (displayPrecision != null)
				{
					DisplayPrecision.CopyPropertiesValue(displayPrecision);
				}


				_isVoltNom = (bool?)isVoltNom ?? true;

				FilePath = fileName;

				var basicVertex = Vertices.FirstOrDefault(o => o.IsBasic);



				if (basicVertex != null)
				{
					basicVertex.IsBasic = false;
					basicVertex.IsBasic = true;
					//DepthFirstSearch.DFS(basicVertex);
				}
			}
			catch
			{
				// ignored
			}
		}

		#endregion

		#region команды выбора СХН

		/// <summary>
		/// Открывает окно выбора СХН.
		/// </summary>
		public ICommand OpenShnSelectionWindowCommand { get; }

		/// <summary>
		/// Метод открытия окна для выбора СХН.
		/// </summary>
		/// <param name="parameters">Узел.</param>
		private void OnOpenShnSelectionWindowCommandExecute(object parameters)
		{
			var objects = (object[])parameters;

			if (objects[0] is not IVertex vertex) return;

			var window = new ShnSelectionWindow()
			{
				DataContext = this,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Vertex = vertex,
				Owner = (Window)objects[1],
			};

			window.ShowDialog();

		}

		#endregion

		#region команды выбора РПН

		/// <summary>
		/// Открывает окно выбора РПН.
		/// </summary>
		public ICommand OpenRpnSelectionWindowCommand { get; }

		/// <summary>
		/// Метод открытия окна для выбора РПН.
		/// </summary>
		/// <param name="sender"></param>
		private void OnOpenRpnSelectionWindowCommandExecute(object sender)
		{
			var objects = (object[])sender;

			if (objects[0] is not IEdge edge) return;

			var window = new RpnSelectionWindow()
			{
				DataContext = this,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Edge = edge,
				RpnType = (string)objects[1],
				Owner = (Window)objects[2],
			};

			window.ShowDialog();
		}

		#endregion

		#region открытие меню настроек


		public SettingsWindow SettingsWindow;

		public ICommand OpenSettingsWindowCommand { get; }

		private void OnOpenSettingsWindowCommandExecute(object obj)
		{
			SettingsWindow = new SettingsWindow
			{
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				DataContext = new SettingsViewModel(this),
				Owner = (System.Windows.Window)obj,
			};
			var settingsViewModel = SettingsWindow.DataContext as SettingsViewModel;
			SettingsWindow.Closed += settingsViewModel.OnSettingsWindowCloused;
			settingsViewModel.ApplicationViewModel = this;
			SettingsWindow.ShowDialog();
		}

		private bool OnOpenSettingsWindowCanExecuted(object obj)
		{
			return SettingsWindow == null;
		}


		#endregion

		#region открыть "о программе"

		public ICommand OpenAboutCommand { get; } = new RelayCommand(OnOpenAboutCommandExecute);

		private static void OnOpenAboutCommandExecute(object parameter)
		{
			var about = new AboutWindow
			{
				Owner = Application.Current.MainWindow,
			};
			about.ShowDialog();
		}

		#endregion

		#region загрузилось окно

		public ICommand WindowLoadedCommand { get; } = new RelayCommand(OnWindowLoadedCommandExecuteAsync);

		private static async void OnWindowLoadedCommandExecuteAsync(object parameter)
		{
			await VersionController.DeleteTempFilesAsync();
			VersionController.GetActualVersionAsync(null, NotifyAvailabilityUpdate, null);
		}

		/// <summary>
		/// Уведомляет о наличии новой версии через 0,5 секунды после запуска программы.
		/// </summary>
		private static void NotifyAvailabilityUpdate()
		{
			Task.Factory.StartNew(() =>
			{
				Thread.Sleep(500);

				Growl.Ask("Обнаружена новая версия программы. Вы хотите обновить?",
					result =>
					{
						if (result)
						{
							VersionController.UpdateProgramAsync();
						}

						return true;
					});
			});

		}

		#endregion

		#region команда выбора узла

		public ICommand OpenVertexSelectionWindowCommand { get; }

		private void OnOpenSelectionWindowCommandExecute(object parameters)
		{
			var objects = (object[])parameters;

			if (objects[0] is not IEdge edge) return;

			var window = new VertexSelectionWindow()
			{
				DataContext = this,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Edge = edge,
				VertexType = (string)objects[1],
				Owner = (Window)objects[2],
			};

			window.ShowDialog();
		}

		#endregion

		#region добавлена новая ветвь в таблицу

		public ICommand AddingNewEdgeDataGridCommand { get; } = new RelayCommand(OnAddingNewEdgeDataGridCommandExecute);

		private static void OnAddingNewEdgeDataGridCommandExecute(object obj)
		{
			if (obj is AddingNewItemEventArgs e)
			{
				e.NewItem = new Edge
				{
					IsCreatedByDataGrid = true,
				};
			}
		}

		#endregion

		#region добавлен новый узел в таблицу

		public ICommand AddingNewVertexDataGridCommand { get; } = new RelayCommand(OnAddingNewVertexDataGridCommandExecute);

		private static void OnAddingNewVertexDataGridCommandExecute(object obj)
		{
			if (obj is AddingNewItemEventArgs e)
			{
				e.NewItem = new Vertex()
				{
					IsCreatedByDataGrid = true,
				};
			}
		}

		#endregion

		#region принимает текущий расчет в качесвте первого приближения

		// При повторном нажатии возвращает номинальные напряжения.

		private  bool _isVoltNom = true;

		public ICommand SetFirstApproximationCommand { get; }

		private void OnSetFirstApproximationCommandExecute(object parameter)
		{
			if (Vertices.Count(o => o.IsConnected) <= 0) return;

			if (_isVoltNom)
			{
				foreach (var vertex in Vertices)
				{
					if (!vertex.IsConnected) continue;
					if (vertex.IsBasic) continue;
					if (vertex.IsGround) continue;

					vertex.VoltNomTemp = vertex.VoltNom;
					vertex.VoltNom = vertex.VoltMagn;
					_isVoltNom = false;
				}
			}
			else
			{
				foreach (var vertex in Vertices)
				{
					if (!vertex.IsConnected) continue;
					if (vertex.IsBasic) continue;
					if (vertex.IsGround) continue;

					vertex.VoltNom = vertex.VoltNomTemp;
					vertex.VoltNomTemp = null;
					_isVoltNom = true;
				}
			}
		}

		private bool OnSetFirstApproximationCanExecute(object parameter)
		{
			var vertexNull = Vertices.FirstOrDefault(o => o.VoltMagn == null && o.IsConnected);
			var verticesCount = Vertices.Count(o => o.IsConnected);
			return vertexNull == null && verticesCount > 0;
		}

		#endregion

		#endregion

		#region поля

		/// <summary>
		/// Выбранный элемент схемы.
		/// </summary>
		private CircuitElement _selectedElement;

		private readonly Visibility[] _tabVisibilities = {
			Visibility.Visible,
			Visibility.Visible,
			Visibility.Visible,
			Visibility.Visible,
			Visibility.Visible,
		};

		private string _filePath;

		#endregion

		public ApplicationViewModel()
		{
			EnableColumns = new EnableColumns();
			Units = new Units();
			DisplayPrecision = new DisplayPrecision();

			LoadDefaultSettings();

			Vertices = new ObservableCollection<Vertex>();
			Edges = new ObservableCollection<Edge>();
			Shns = new ObservableCollection<Shn>();
			Rpns = new ObservableCollection<Rpn>();
			CalculateSteadyState.SetCollections(Vertices, Edges);

			((ObservableCollection<Rpn>)Rpns).CollectionChanged += Rpns_CollectionChanged;
			((ObservableCollection<Shn>)Shns).CollectionChanged += Shns_CollectionChanged;

			RegimeCommand = new RelayCommand(OnRegimeCommandExecute);
			OpenInNewWindowCommand = new RelayCommand(OnOpenInNewWindowCommandExecute, OnOpenInNewWindowCanExecuted);
			OpenSettingsWindowCommand = new RelayCommand(OnOpenSettingsWindowCommandExecute, OnOpenSettingsWindowCanExecuted);

			CreateCommand = new RelayCommand(OnCreateCommandExecute);
			SaveCommand = new RelayCommand(OnSaveCommandExecute);
			OpenCommand = new RelayCommand(OnOpenCommandExecute);

			OpenShnSelectionWindowCommand = new RelayCommand(OnOpenShnSelectionWindowCommandExecute);
			OpenRpnSelectionWindowCommand = new RelayCommand(OnOpenRpnSelectionWindowCommandExecute);
			OpenVertexSelectionWindowCommand = new RelayCommand(OnOpenSelectionWindowCommandExecute);

			SetFirstApproximationCommand = new RelayCommand(OnSetFirstApproximationCommandExecute, OnSetFirstApproximationCanExecute);

			VersionController.HttpReceiveProgress += (progress) => Progress = progress;
			VersionController.HttpReceiveProgress += OnDownloadNewVersionProgress;
		}

		/// <summary>
		/// Обработчик изменения процентов скачиваня программы.
		/// </summary>
		/// <param name="progress"></param>
		private void OnDownloadNewVersionProgress(int progress)
		{
			VersionController.HttpReceiveProgress -= OnDownloadNewVersionProgress;
			IsReadOnly = true;
			IsDownloadPanelVisible = true;
		}

		#region приватные методы

		/// <summary>
		/// Загружает настройки по умолчанию.
		/// </summary>
		public void LoadDefaultSettings()
		{
			var fileName = SettingsFileName;

			if (!File.Exists(fileName))
			{
				IsRelative = false;
				CalculatePrecision = 0.001f;
				EnableColumns.CopyPropertiesValue(new EnableColumns());
				DisplayPrecision.CopyPropertiesValue(new DisplayPrecision());
				Units?.CopyPropertiesValue(new Units());
				return;
			}

			using var reader = File.OpenText(fileName);
			EnableColumns.CopyPropertiesValue(JsonConvert.DeserializeObject<EnableColumns>(reader.ReadLine()!)!);
			DisplayPrecision.CopyPropertiesValue(JsonConvert.DeserializeObject<DisplayPrecision>(reader.ReadLine()!)!);
			Units?.CopyPropertiesValue(JsonConvert.DeserializeObject<Units>(reader.ReadLine()!));
			IsRelative = Convert.ToBoolean(reader.ReadLine());
			CalculatePrecision = JsonConvert.DeserializeObject<float>(reader.ReadLine() ?? "0.001");
		}

		/// <summary>
		/// Очищает все, чтобы проект был пуст.
		/// </summary>
		private void ClearAll()
		{
			while (Vertices.Count > 0)
			{
				Vertices.Remove(Vertices.ElementAt(Vertices.Count - 1));
			}

			while (Edges.Count > 0)
			{
				Edges.Remove(Edges.ElementAt(Edges.Count - 1));
			}

			while (Rpns.Count > 0)
			{
				Rpns.Remove(Rpns.ElementAt(Rpns.Count - 1));
			}

			while (Shns.Count > 0)
			{
				Shns.Remove(Shns.ElementAt(Shns.Count - 1));
			}

			_isVoltNom = true;

			FilePath = string.Empty;
		}

		private async void Shns_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems?[0] is IShn entity)
				{
					if (entity.Index == 0 && sender != null)
					{
						await CalculateSteadyState.SetIndex(Shns, entity);
					}
				}
			}
		}

		private async void Rpns_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems?[0] is IRpn entity)
				{
					if (entity.Index == 0 && sender != null)
					{
						await CalculateSteadyState.SetIndex(Rpns, entity);
					}
				}
			}
		}

		#endregion
	}
}
