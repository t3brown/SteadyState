using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HandyControl.Controls;
using SteadyState.Grapher.Elements;
using SteadyState.Interfaces;
using SteadyState.MainProject.WPF.Commands;
using SteadyState.MainProject.WPF.Models;

namespace SteadyState.MainProject.WPF.Views
{
	public partial class VertexSelectionWindow : Window
	{
		public IEdge Edge;
		public string VertexType;
		public ICommand AcceptSelectionCommand { get; }
		public ICommand ResetSelectionCommand { get; }

		public VertexSelectionWindow()
		{
			InitializeComponent();
			AcceptSelectionCommand = new RelayCommand(OnAcceptSelectionCommandExecute, OnAcceptSelectionCanExecute);
			ResetSelectionCommand = new RelayCommand(OnResetSelectionCommandExecute);
		}

		protected virtual void OnAcceptSelectionCommandExecute(object item)
		{
			if (item is not IVertex vertex) return;

			if (string.Equals(VertexType, "V1"))
			{
				Edge.V1Id = vertex.Id;
				Edge.V1 = vertex;
			}

			if (string.Equals(VertexType, "V2"))
			{
				Edge.V2Id = vertex.Id;
				Edge.V2 = vertex;
			}

			Close();
		}

		protected virtual void OnResetSelectionCommandExecute(object item)
		{
			if (string.Equals(VertexType, "V1"))
			{
				Edge.V1Id = Guid.Empty;
				Edge.V1 = null;
			}

			if (string.Equals(VertexType, "V2"))
			{
				Edge.V2Id = Guid.Empty;
				Edge.V2 = null;
			}


			Close();
		}

		protected virtual bool OnAcceptSelectionCanExecute(object item)
		{
			return item is IVertex;
		}

		private void DataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter) return;

			if (!OnAcceptSelectionCanExecute(DataGrid.SelectedItem)) return;

			DataGrid.CommitEdit(DataGridEditingUnit.Row, true);

			OnAcceptSelectionCommandExecute(DataGrid.SelectedItem);
		}
	}
}