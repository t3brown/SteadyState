using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SteadyState.Interfaces;
using SteadyState.MainProject.WPF.Commands;

namespace SteadyState.MainProject.WPF.ViewModels
{
	public class ApplicationViewModel: ViewModelBase
	{
		#region свойства

		public IEnumerable<IVertex> Vertices { get; set; }
		public IEnumerable<IEdge> Edges { get; set; }

		public object SelectedTab { get; set; }

		#endregion

		#region команды

		public ICommand RegimeCommand { get; private set; }
		public ICommand OpenInNewWindowCommand { get; private set; }

		#endregion

		public ApplicationViewModel()
		{
			Vertices = new ObservableCollection<IVertex>();
			Edges = new ObservableCollection<IEdge>();
			CalculateSteadyState.SetCollections(Vertices, Edges);

			RegimeCommand = new RelayCommand(OnRegimeCommandExecute);
			OpenInNewWindowCommand = new RelayCommand(OpenInNewWindowCommandExecute);
		}

		private void OnRegimeCommandExecute(object obj)
		{
			CalculateSteadyState.Calculate(0.001f);
		}

		private void OpenInNewWindowCommandExecute(object obj)
		{
			if (SelectedTab is TabItem tab)
			{
				var newTabWindow = new Window
				{
					Owner = Application.Current.MainWindow,
					Content = tab.Content
				};
				tab.Visibility = Visibility.Collapsed;
				tab.Content = null;
				newTabWindow.Tag = tab;
				newTabWindow.Closed += NewTabWindow_Closed;
				newTabWindow.Show();
			}
		}

		private void NewTabWindow_Closed(object? sender, EventArgs e)
		{
			if (sender is Window window)
			{
				if (window.Tag is TabItem tab)
				{
					tab.Visibility = Visibility.Visible;
					tab.Content = window.Content;
					tab.IsSelected = true;
				}
			}
		}
	}
}
