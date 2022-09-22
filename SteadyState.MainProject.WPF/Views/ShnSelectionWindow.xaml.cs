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
using SteadyState.Interfaces;
using SteadyState.MainProject.WPF.Commands;

namespace SteadyState.MainProject.WPF.Views
{
	/// <summary>
	/// Interaction logic for ShnSelectionWindow.xaml
	/// </summary>
	public partial class ShnSelectionWindow : Window
	{
		public IVertex Vertex;
		public ICommand AcceptSelectionCommand { get; }
		public ICommand ResetSelectionCommand { get; }

		public ShnSelectionWindow()
		{
			InitializeComponent();
			AcceptSelectionCommand = new RelayCommand(OnAcceptSelectionCommandExecute, OnAcceptSelectionCanExecute);
			ResetSelectionCommand = new RelayCommand(OnResetSelectionCommandExecute);
		}

		protected virtual void OnAcceptSelectionCommandExecute(object item)
		{
			if (item is not IShn shn) return;

			Vertex.Shn = shn;
			Vertex.ShnId = shn.Id;

			Close();
		}

		protected virtual void OnResetSelectionCommandExecute(object item)
		{
			Vertex.Shn = null!;
			Vertex.ShnId = Guid.Empty;

			Close();
		}


		protected virtual bool OnAcceptSelectionCanExecute(object item)
		{
			return item is IShn;
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
