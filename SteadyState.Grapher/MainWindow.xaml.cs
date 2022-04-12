using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SteadyState.Grapher.Annotations;
using SteadyState.Grapher.Elements;
using SteadyState.Interfaces;

namespace SteadyState.Grapher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private readonly List<(IVertex, IVertex)> _replacedVertices;
        public IEnumerable<IVertex> Vertices { get; set; }
        public IEnumerable<IEdge> Edges { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Vertices = new ObservableCollection<IVertex>();
            Edges = new ObservableCollection<IEdge>();
            CalculateSteadyState.SetCollection(Vertices, Edges);


        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Window newTabWindow = new Window();
            newTabWindow.Owner = this;
            newTabWindow.Content = TabControl.SelectedContent;
            ((TabItem) TabControl.SelectedItem).Visibility = Visibility.Collapsed;
            ((TabItem) TabControl.SelectedItem).Content = null;
            newTabWindow.Tag = ((TabItem) TabControl.SelectedItem);
            newTabWindow.Closed += NewTabWindow_Closed;
            newTabWindow.Show();
        }

        private void NewTabWindow_Closed(object? sender, EventArgs e)
        {
            if (sender is Window window)
            {
                ((TabItem)window.Tag).Visibility = Visibility.Visible;
                ((TabItem)window.Tag).Content = window.Content;
                ((TabItem) window.Tag).IsSelected = true;
            }
        }
    }
}
