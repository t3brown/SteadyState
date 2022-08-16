using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft;
using Newtonsoft.Json;
using SteadyState.Grapher.Elements;
using SteadyState.Interfaces;
using SteadyState.MainProject.WPF.Commands;

namespace SteadyState.MainProject.WPF.ViewModels
{
	public class ApplicationViewModel : ViewModelBase
	{
		#region свойства

		public ICollection<IVertex> Vertices { get; set; }
		public ICollection<IEdge> Edges { get; set; }

		public object SelectedTab { get; set; }

		#endregion

		#region команды

		public ICommand RegimeCommand { get; private set; }
		public ICommand OpenInNewWindowCommand { get; private set; }
		public ICommand SaveCommand { get; private set; }

		#endregion

		public ApplicationViewModel()
		{
			Vertices = new ObservableCollection<IVertex>();
			Edges = new ObservableCollection<IEdge>();
			CalculateSteadyState.SetCollections(Vertices, Edges);

			RegimeCommand = new RelayCommand(OnRegimeCommandExecute);
			OpenInNewWindowCommand = new RelayCommand(OpenInNewWindowCommandExecute);
			SaveCommand = new RelayCommand(OnSaveCommandExecute);
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

		private void OnSaveCommandExecute(object obj)
		{
			try
			{

				var savedVer = new Vertex();
				var ver = Vertices.ElementAt(0) as Vertex;

				if (ver != null)
				{
					savedVer.StartPoint = ver.StartPoint;
					savedVer.Width = ver.Width;
					savedVer.Height = ver.Height;
					savedVer.Angle = ver.Angle;

					savedVer.Id = ver.Id;
				}

				Vertices.Add(savedVer);



				var savedEdge = new Edge();
				var edge = Edges.ElementAt(0) as Edge;
				if (edge != null)
				{
					savedEdge.PointCollection = edge.PointCollection;
					savedEdge.Id = edge.Id;
					savedEdge.V1Id = edge.V1Id;
					savedEdge.V1 = edge.V1;
					savedEdge.V2Id = edge.V2Id;
					savedEdge.V2 = edge.V2;
					savedEdge.OldV1Id = edge.V1Id;
					savedEdge.OldV1 = edge.V1;
					savedEdge.OldV2Id = edge.V2Id;
					savedEdge.OldV2 = edge.V2;
				}
				Edges.Add(savedEdge);
			}
			catch
			{

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
