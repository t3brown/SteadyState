using Newtonsoft.Json;
using SteadyState.Grapher.Elements;
using SteadyState.Interfaces;
using SteadyState.MainProject.WPF.Commands;
using SteadyState.MainProject.WPF.Models;
using SteadyState.MainProject.WPF.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SteadyState.MainProject.WPF.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		#region Списки выбора ед. измерения и точности отображения
		public List<string> VoltageUnits { get { return new() { "кВ", "В", "о.е." }; } }
		public List<string> PowerReUnits { get { return new() { "МВт", "кВт", "о.е." }; } }
		public List<string> PowerImUnits { get { return new() { "Мвар", "квар", "о.е." }; } }
		public List<string> AmperageUnits { get { return new() { "А", "кА", "о.е." }; } }
		public List<string> AngleUnits { get { return new() { "°", "рад", "о.е." }; } }
		public List<Precision> DisplayPresicions => new()
		{
			new() { Label = "1 (0)", Value = 0 },
			new() { Label = "0.1 (1)", Value = 1 },
			new() { Label = "0.01 (2)", Value = 2 },
			new() { Label = "0.001 (3)", Value = 3 },
			new() { Label = "0.0001 (4)", Value = 4 },
			new() { Label = "0.00001 (5)", Value = 5 },
			new() { Label = "0.000001 (6)", Value = 6 },
			new() { Label = "0.0000001 (7)", Value = 7 },
			new() { Label = "0.00000001 (8)", Value = 8 },
			new() { Label = "0.000000001 (9)", Value = 9 }
		};
		#endregion

		public ApplicationViewModel ApplicationViewModel { get; set; }


		private bool canCancel = true;

		private bool isRelative;
		private EnableColumns enableColumns;
		private Units? units;
		private DisplayPrecision displayPrecision;

		private float calcPrecision;
		private int iterationCount;



		#region ApplyCommand: RelayCommand - команда применения настроек и последующим закрытием окна

		private RelayCommand _applyCommand;
		public RelayCommand ApplyCommand
		{
			get
			{
				return _applyCommand ??
				  (_applyCommand = new RelayCommand(obj =>
				  {
					  var settingsWindow = obj as SettingsWindow;
					  canCancel = false;

					  //ApplicationViewModel.NeedSave = true;

					  #region обновление данных в таблицах после принятия настроек

					  foreach (Edge edge in ApplicationViewModel.Edges)
					  {
						  var props = edge.GetType().GetProperties();
						  foreach (var prop in props)
							  edge.OnPropertyChanged(prop.Name);
					  }
					  foreach (Vertex vertex in ApplicationViewModel.Vertices)
					  {
						  var props = vertex.GetType().GetProperties();
						  foreach (var prop in props)
							  vertex.OnPropertyChanged(prop.Name);
					  }

					  #endregion

					  settingsWindow.Close();
				  }));
			}
		}
		#endregion

		#region CancelCommand: RelayCommand - команда отмены изменненых настроек, а так же сопутствующие закрытию окну настроек 
		private RelayCommand _cancelCommand;
		public RelayCommand CancelCommand
		{
			get
			{
				return _cancelCommand ??
				  (_cancelCommand = new RelayCommand(obj =>
				  {
					  CopySettingsPropirties();
					  var settingsWindow = obj as SettingsWindow;
					  settingsWindow.Close();
				  }));
			}
		}
		internal void OnSettingsWindowCloused(object sender, EventArgs e)
		{
			if (canCancel)
			{
				CopySettingsPropirties();
			}
			var settingsWindow = sender as SettingsWindow;
			settingsWindow.Closed -= OnSettingsWindowCloused;
			ApplicationViewModel.SettingsWindow = null;
		}
		private void CopySettingsPropirties()
		{
			ApplicationViewModel.IsRelative = isRelative;
			ApplicationViewModel.EnableColumns.CopyPropertiesValue(enableColumns);
			ApplicationViewModel.Units.CopyPropertiesValue(units);
			ApplicationViewModel.DisplayPrecision.CopyPropertiesValue(displayPrecision);

			ApplicationViewModel.CalcPrecision = calcPrecision;
			ApplicationViewModel.IterationCount = iterationCount;
		}
		#endregion

		#region LoadDefaultSettingsCommand: ICommand - команда возвращения настроек к значениям по умоляанию
		public ICommand LoadDefaultSettingsCommand { get; set; }
		#endregion

		#region SetDefaultSettingsCommand: ICommand - команда устанавливающая текущие натсройки как настройки по умолчанию
		public ICommand SetDefaultSettingsCommand { get; set; }
		#endregion

		#region DeleteDefaultSettingsCommand: ICommand - команда удаления текущих пользовательских настроек по умолчанию
		public ICommand DeleteDefaultSettingsCommand { get; set; }
		#endregion

		public SettingsViewModel(ApplicationViewModel viewModel)
		{
			ApplicationViewModel = viewModel;

			SetDefaultSettingsCommand = new RelayCommand(OnSetDefaultSettingsCommandExecuted);
			LoadDefaultSettingsCommand = new RelayCommand(OnLoadDefaultSettingsCommandExecuted);
			DeleteDefaultSettingsCommand = new RelayCommand(OnDeleteDefaultSettingsCommandExucited);

			isRelative = ApplicationViewModel.IsRelative;
			units = ApplicationViewModel.Units.Clone() as Units;
			displayPrecision = ApplicationViewModel.DisplayPrecision.Clone() as DisplayPrecision;
			enableColumns = ApplicationViewModel.EnableColumns.Clone() as EnableColumns;

			calcPrecision = ApplicationViewModel.CalcPrecision;
			iterationCount = ApplicationViewModel.IterationCount;
		}


		public SettingsViewModel() { }

		private void OnDeleteDefaultSettingsCommandExucited(object obj)
		{
			string file_name = ApplicationViewModel.SettingsFileName;
			if (File.Exists(file_name))
			{
				File.Delete(file_name);
			}
		}

		public void OnSetDefaultSettingsCommandExecuted(object p)
		{
			string file_name = ApplicationViewModel.SettingsFileName;

			if (File.Exists(file_name)) File.Delete(file_name);

			using (var writer = new StreamWriter(new FileStream(file_name, FileMode.Create, FileAccess.Write)))
			{
				writer.WriteLine(JsonConvert.SerializeObject(ApplicationViewModel.EnableColumns));
				writer.WriteLine(JsonConvert.SerializeObject(ApplicationViewModel.DisplayPrecision));
				writer.WriteLine(JsonConvert.SerializeObject(ApplicationViewModel.IsRelative ? ApplicationViewModel.NamedUnits : ApplicationViewModel.Units));
				writer.WriteLine(ApplicationViewModel.IsRelative);

				writer.WriteLine(ApplicationViewModel.CalcPrecision);
				writer.WriteLine(ApplicationViewModel.IterationCount);
			}
		}

		public void OnLoadDefaultSettingsCommandExecuted(object p)
		{
			ApplicationViewModel.LoadDefaultSettings();
		}
	}

	public class Precision
	{
		private string _label;
		public string Label { get => _label; set => _label = value; }

		private byte _value;
		public byte Value { get => _value; set => _value = value; }
	}
}
