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
	/// <summary>
	/// Interaction logic for ShnSelectionWindow.xaml
	/// </summary>
	public partial class RpnSelectionWindow : Window
	{
		public IEdge Edge;
		public string RpnType;
		public ICommand AcceptSelectionCommand { get; }
		public ICommand ResetSelectionCommand { get; }

		public RpnSelectionWindow()
		{
			InitializeComponent();
			AcceptSelectionCommand = new RelayCommand(OnAcceptSelectionCommandExecute, OnAcceptSelectionCanExecute);
			ResetSelectionCommand = new RelayCommand(OnResetSelectionCommandExecute);
		}

		protected virtual void OnAcceptSelectionCommandExecute(object item)
		{
			if (item is not IRpn rpn) return;

			if (string.Equals(RpnType, "Rpn1"))
			{
				Edge.Rpn1 = rpn;
				Edge.Rpn1Id = rpn.Id;
			}

			if (string.Equals(RpnType, "Rpn2"))
			{
				Edge.Rpn2 = rpn;
				Edge.Rpn2Id = rpn.Id;
			}

			Close();
		}

		protected virtual void OnResetSelectionCommandExecute(object item)
		{
			if (string.Equals(RpnType, "Rpn1"))
			{
				Edge.Rpn1 = null;
				Edge.Rpn1Id = Guid.Empty;
			}

			if (string.Equals(RpnType, "Rpn2"))
			{
				Edge.Rpn2 = null;
				Edge.Rpn2Id = Guid.Empty;
			}

			Close();
		}

		protected virtual bool OnAcceptSelectionCanExecute(object item)
		{
			return item is IRpn;
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