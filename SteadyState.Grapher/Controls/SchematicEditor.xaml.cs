using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SteadyState.Grapher.Annotations;
using SteadyState.Grapher.Elements;
using SteadyState.Interfaces;
using Point = System.Windows.Point;
using Switch = SteadyState.Grapher.Elements.Switch;

namespace SteadyState.Grapher.Controls
{
	/// <summary>
	/// Логика взаимодействия для SchematicEditor.xaml
	/// </summary>
	public partial class SchematicEditor : UserControl, INotifyPropertyChanged
	{
		private Point? _lastCenterPositionOnTarget;
		private Point? _lastMousePositionOnTarget;
		private Point? _lastDragPoint;

		/// <summary>
		/// Возмодность сброса выбора (на первое выделение).
		/// </summary>
		private bool _canResetSelection;

		#region Режим рисования.

		private bool _isDrawing;

		/// <summary>
		/// Режим рисования.
		/// </summary>
		public bool IsDrawing
		{
			get => _isDrawing;
			private set
			{
				if (value == false)
				{
					_horizontaLine.Visibility = Visibility.Collapsed;
					_verticalLine.Visibility = Visibility.Collapsed;

					_currentElement.IsPreview = false;
					_setCurrentElementPoint = null;
					_currentElement = null;
				}
				if (value)
				{
					_horizontaLine.Visibility = Visibility.Visible;
					_verticalLine.Visibility = Visibility.Visible;
				}

				_isDrawing = value;
			}
		}

		#endregion

		#region Прицел

		private Line _horizontaLine;
		private Line _verticalLine;

		#endregion

		/// <summary>
		/// Элементы схемы.
		/// </summary>
		private IEnumerable<CircuitElement> _elements;

		#region Выбранный элемент.

		private CircuitElement _selectedElement;

		/// <summary>
		/// Выбранный элемент.
		/// </summary>
		public CircuitElement SelectedElement
		{
			get => _selectedElement;
			set
			{
				if (_selectedElement == value) return;
				_selectedElement = value;
				OnPropertyChanged();
			}
		}

		#endregion

		/// <summary>
		/// Базисный узел.
		/// </summary>
		internal static IVertex? BasicVertex;

		/// <summary>
		/// Текущий элемент для редактирования.
		/// </summary>
		private CircuitElement? _currentElement;

		/// <summary>
		/// Начальная точка теккущего элемента редактирования.
		/// </summary>
		private Point? _setCurrentElementPoint;

		#region Иcточник узлов.

		public static readonly DependencyProperty VerticesSourceProperty = DependencyProperty.Register(
			"VerticesSource", typeof(ICollection<IVertex>),
			typeof(SchematicEditor),
			new PropertyMetadata(null));

		public ICollection<IVertex>? VerticesSource
		{
			get { return (ICollection<IVertex>)GetValue(VerticesSourceProperty); }
			set { SetValue(VerticesSourceProperty, value); }
		}

		#endregion

		#region Итсточник ветвей.

		public static readonly DependencyProperty EdgesSourceProperty = DependencyProperty.Register(
			"EdgesSource", typeof(ICollection<IEdge>),
			typeof(SchematicEditor),
			new PropertyMetadata(null));

		public ICollection<IEdge>? EdgesSource
		{
			get { return (ICollection<IEdge>)GetValue(EdgesSourceProperty); }
			set { SetValue(EdgesSourceProperty, value); }
		}

		#endregion

		public event Action<CircuitElement> OnElementAdd;

		public SchematicEditor()
		{
			InitializeComponent();

			_horizontaLine = new Line()
			{
				Stroke = Brushes.Black,
				Opacity = 0.2,
				X1 = 0,
				X2 = Grid.Width,
				Visibility = Visibility.Collapsed
			};
			_verticalLine = new Line()
			{
				Stroke = Brushes.Black,
				Opacity = 0.2,
				Y1 = 0,
				Y2 = Grid.Height,
				Visibility = Visibility.Collapsed
			};

			Canvas.Children.Add(_horizontaLine);
			Canvas.Children.Add(_verticalLine);

			_elements = Canvas.Children.Cast<UIElement>().Where(a => a is CircuitElement).Cast<CircuitElement>();

			ScrollViewer.ScrollChanged += ScrollViewer_OnScrollViewerScrollChanged;
			ScrollViewer.MouseUp += ScrollViewer_OnMouseUp;
			ScrollViewer.PreviewMouseUp += ScrollViewer_OnMouseUp;
			ScrollViewer.PreviewMouseWheel += ScrollViewer_OnPreviewMouseWheel;
			ScrollViewer.PreviewMouseDown += ScrollViewer_OnMouseDown;
			ScrollViewer.MouseMove += ScrollViewer_OnMouseMove;
			slider.ValueChanged += Slider_OnSliderValueChanged;
			slider.Value = 1;

			OnElementAdd += SchematicEditor_OnElementAdd;
		}

		/// <summary>
		/// Элемент добавлен на схему.
		/// </summary>
		private void SchematicEditor_OnElementAdd(CircuitElement obj)
		{
			if (obj is Vertex vertex)
			{
				if (VerticesSource != null)
				{
					VerticesSource.Add(vertex);
					vertex.Index = VerticesSource.Max(a => a.Index) + 1;
				}
			}

			if (obj is Edge edge)
			{

				if (EdgesSource != null)
				{
					EdgesSource.Add(edge);
					edge.Index = EdgesSource.Max(a => a.Index) + 1;
				}

				edge.OldV1Id = edge.V1Id;
				edge.OldV1 = edge.V1;
				edge.OldV2Id = edge.V2Id;
				edge.OldV2 = edge.V2;

				edge.SwitchPositionChanged += Edge_OnSwitchPositionChanged;

				//запускается поиск в глубину при добавлении новой ветви
				if (edge.V1.IsConnected || edge.V2.IsConnected)
					if (BasicVertex != null)
						DepthFirstSearch.DFS(BasicVertex);

			}
		}

		private void Edge_OnSwitchPositionChanged(IEdge edge, Switch sw, SwitchPosition pos)
		{
			switch (pos)
			{
				case SwitchPosition.On:
					switch (sw)
					{
						case Switch.Q1:
							VerticesSource?.Remove(edge.V1);
							edge.V1Id = edge.OldV1Id;
							edge.V1 = edge.OldV1;
							break;
						case Switch.Q2:
							VerticesSource?.Remove(edge.V2);
							edge.V2Id = edge.OldV2Id;
							edge.V2 = edge.OldV2;
							break;
					}
					break;

				case SwitchPosition.Off:
					var vertex = new Vertex() { Name = "temp", Id = Guid.NewGuid() };
					switch (sw)
					{
						case Switch.Q1:
							edge.OldV1Id = edge.V1Id;
							edge.OldV1 = edge.V1;
							vertex.VoltNom = edge.OldV1.VoltNom;
							edge.V1Id = vertex.Id;
							edge.V1 = vertex;
							break;

						case Switch.Q2:
							edge.OldV2Id = edge.V2Id;
							edge.OldV2 = edge.V2;
							vertex.VoltNom = edge.OldV2.VoltNom;
							edge.V2Id = vertex.Id;
							edge.V2 = vertex;
							break;
					}
					VerticesSource?.Add(vertex);
					break;
			}
			//запускается поиск в глубину при переключениях 
			if (BasicVertex != null)
			{
				DepthFirstSearch.DFS(Controls.SchematicEditor.BasicVertex);
			}
		}

		#region zoom and sroll

		private void ScrollViewer_OnMouseMove(object sender, MouseEventArgs e)
		{
			if (_lastDragPoint.HasValue)
			{
				Point posNow = e.GetPosition(ScrollViewer);

				double dX = posNow.X - _lastDragPoint.Value.X;
				double dY = posNow.Y - _lastDragPoint.Value.Y;

				_lastDragPoint = posNow;

				ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - dX);
				ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - dY);
			}
		}

		private void ScrollViewer_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			#region передвижение по канве

			if (e.ChangedButton == MouseButton.Middle)
			{
				var mousePos = e.GetPosition(ScrollViewer);
				if (mousePos.X <= ScrollViewer.ViewportWidth &&
					mousePos.Y < ScrollViewer.ViewportHeight)
				{
					ScrollViewer.Cursor = Cursors.SizeAll;
					_lastDragPoint = mousePos;
					Mouse.Capture(ScrollViewer);
				}
			}

			#endregion
		}
		private void ScrollViewer_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				_lastMousePositionOnTarget = Mouse.GetPosition(Grid);

				if (e.Delta > 0)
				{
					slider.Value += slider.Value < 1 ? slider.Value < 0.5 ? 0.01 : 0.02 : 0.1;
				}

				if (e.Delta < 0)
				{
					slider.Value -= slider.Value < 1 ? slider.Value < 0.5 ? 0.01 : 0.02 : 0.1;
				}

				e.Handled = true;
			}

			if (Keyboard.Modifiers == ModifierKeys.Shift)
			{
				ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - e.Delta / 2);

				e.Handled = true;
			}
		}

		private void ScrollViewer_OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Middle) return;
			ScrollViewer.Cursor = Cursors.Arrow;
			ScrollViewer.ReleaseMouseCapture();
			_lastDragPoint = null;
		}

		private void Slider_OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ScaleTransform.ScaleX = e.NewValue;
			ScaleTransform.ScaleY = e.NewValue;

			var centerOfViewport = new Point(ScrollViewer.ViewportWidth / 2, ScrollViewer.ViewportHeight / 2);
			_lastCenterPositionOnTarget = ScrollViewer.TranslatePoint(centerOfViewport, Grid);
		}

		private void ScrollViewer_OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
			{
				Point? targetBefore = null;
				Point? targetNow = null;

				if (!_lastMousePositionOnTarget.HasValue)
				{
					if (_lastCenterPositionOnTarget.HasValue)
					{
						var centerOfViewport =
							new Point(ScrollViewer.ViewportWidth / 2, ScrollViewer.ViewportHeight / 2);
						Point centerOfTargetNow = ScrollViewer.TranslatePoint(centerOfViewport, Grid);

						targetBefore = _lastCenterPositionOnTarget;
						targetNow = centerOfTargetNow;
					}
				}
				else
				{
					targetBefore = _lastMousePositionOnTarget;
					targetNow = Mouse.GetPosition(Grid);

					_lastMousePositionOnTarget = null;
				}

				if (targetBefore.HasValue)
				{
					double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
					double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

					double multiplicatorX = e.ExtentWidth / Grid.Width;
					double multiplicatorY = e.ExtentHeight / Grid.Height;

					double newOffsetX = ScrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
					double newOffsetY = ScrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

					if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
					{
						return;
					}

					ScrollViewer.ScrollToHorizontalOffset(newOffsetX);
					ScrollViewer.ScrollToVerticalOffset(newOffsetY);
				}
			}
		}

		#endregion

		private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//отмена выделения выбраного элемента, при условии что элемент не был выбран только что
			if (_canResetSelection)
				ResetSelectedElement();
			_canResetSelection = true;

			//выполняется в том сулчае, если есть текущий элемент для редактирования и для задана начальная точка
			if (_currentElement != null && _setCurrentElementPoint.HasValue)
			{
				//если это вершина, то редактирование прекращается
				if (_currentElement is Vertex vertex)
				{
					IsDrawing = false;
					OnElementAdd?.Invoke(vertex);
				}
				//если это ветвь, то добавляется следующая точка, а предыдущая становистя начальной
				if (_currentElement is Edge edge)
				{
					edge.PointCollection.Add(new Point(_verticalLine.X1, _horizontaLine.Y1));
					_setCurrentElementPoint = edge.PointCollection[^2];
				}
			}
			//выполняется в том случае, если есть текущий элемент для редактирования, а начальная точка не задана
			if (_currentElement != null && !_setCurrentElementPoint.HasValue)
			{
				//если это вершина, то для нее задается начальная точка
				if (_currentElement is Vertex vertex)
				{
					_setCurrentElementPoint = new Point(_verticalLine.X1 - 5, _horizontaLine.Y1 - 5);
				}
			}
		}

		private void Vertex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//если имеется элемент для редактирования и для него задана начальная точка
			if (_currentElement != null && _setCurrentElementPoint.HasValue)
			{
				//если этот элемент ветвь, то редактирование прекращается
				if (_currentElement is Edge edge)
				{
					if (sender is Vertex vertex)
					{
						if (edge.V1Id != vertex.Id)
						{
							//если последняя точка ветви не равняется точке курса, то добавляется конечная точка в поцизии курсора
							if (edge.PointCollection[^1] != new Point(_verticalLine.X1, _horizontaLine.Y1))
							{
								edge.PointCollection.Add(new Point(_verticalLine.X1, _horizontaLine.Y1));
							}

							edge.V2Id = vertex.Id;
							edge.V2 = vertex;

							IsDrawing = false;

							OnElementAdd?.Invoke(edge);
						}
					}
				}
			}
			//если имеется элемент для редактирования и для него не задана начальная точка
			if (_currentElement != null && !_setCurrentElementPoint.HasValue)
			{
				//если этот элемент ветвь, то для нее добавляется точка в позиции курсора, а предыдущая точка становится начальной
				if (_currentElement is Edge edge)
				{
					if (sender is Vertex vertex)
					{
						edge.PointCollection.Add(new Point(_verticalLine.X1, _horizontaLine.Y1));
						_setCurrentElementPoint = edge.PointCollection[^2];

						edge.V1Id = vertex.Id;
						edge.V1 = vertex;
					}
				}
			}
		}

		private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
		{
			Point mousePosition = e.GetPosition(Canvas);
			_horizontaLine.Y1 = Round(mousePosition.Y) + 5;
			_horizontaLine.Y2 = Round(mousePosition.Y) + 5;
			_verticalLine.X1 = Round(mousePosition.X) + 5;
			_verticalLine.X2 = Round(mousePosition.X) + 5;

			/* если имеется элемент для редактирования и для него задана начальная точка, то расчитывается
             изменение положения курсора относительно начальной точки */
			if (_currentElement != null && _setCurrentElementPoint.HasValue)
			{
				double dX = _verticalLine.X1 - _setCurrentElementPoint.Value.X - 5;
				double dY = _horizontaLine.Y1 - _setCurrentElementPoint.Value.Y - 5;
				//если этот элемент вершина, то вычилсяется ее ширина
				if (_currentElement is Vertex vertex)
				{
					if (Math.Abs(dX) > Math.Abs(dY))
					{
						if (dX > 0)
						{
							vertex.Angle = 0;
							vertex.Width = dX + 10;
						}

						if (dX < 0)
						{
							vertex.Angle = 180;
							vertex.Width = -dX + 10;
						}
					}

					if (Math.Abs(dX) < Math.Abs(dY))
					{
						if (dY > 0)
						{
							vertex.Angle = 90;
							vertex.Width = dY + 10;
						}

						if (dY < 0)
						{
							vertex.Angle = 270;
							vertex.Width = -dY + 10;
						}
					}

					if (dX == 0 && dY == 0)
					{
						vertex.Width = 10;
					}
				}
				//если этот элемент ветвь, то определяется ее текущая конечная точка (положение курсора)
				if (_currentElement is Edge edge)
				{
					if (Math.Abs(dX + 5) > Math.Abs(dY))
					{
						edge.PointCollection[^1] = new Point(_verticalLine.X1, _setCurrentElementPoint.Value.Y);
					}

					if (Math.Abs(dX) < Math.Abs(dY + 5))
					{
						edge.PointCollection[^1] = new Point(_setCurrentElementPoint.Value.X, _horizontaLine.Y1);
					}

					if (Math.Abs(dX + 5) == Math.Abs(dY + 5))
					{
						edge.PointCollection[^1] = new Point(_verticalLine.X1, _horizontaLine.Y1);
					}

				}
			}
			//если имеется элемент для редактирования и для него не задана начальная точка
			if (_currentElement != null && !_setCurrentElementPoint.HasValue)
			{
				//если этот элемент вершина, то оперделяется точка, где она будет начинаться
				if (_currentElement is Vertex vertex)
				{
					Canvas.SetLeft(vertex, _verticalLine.X1 - 5);
					Canvas.SetTop(vertex, _horizontaLine.Y1 - 5);
				}
				//если это ветвь, то определяется точка, где она будет начинаться
				if (_currentElement is Edge edge)
				{
					edge.PointCollection[^1] = new Point(_verticalLine.X1, _horizontaLine.Y1);
				}
			}
		}

		private void AddVertex_OnClick(object sender, RoutedEventArgs e)
		{
			//создается вершина в режиме редактирвания
			IsDrawing = true;
			var vertex = new Vertex()
			{
				Id = Guid.NewGuid(),
				Width = 10,
				IsPreview = true
			};
			_currentElement = vertex;
			Canvas.SetTop(vertex, _horizontaLine.Y1 - 5);
			Canvas.SetLeft(vertex, _verticalLine.X1 - 5);
			vertex.PreviewMouseLeftButtonDown += CircuitElement_PreviewMouseLeftButtonDown;
			vertex.MouseLeftButtonDown += Vertex_MouseLeftButtonDown;
			vertex.OnSelectionChanged += OnSelectionChanged;

			Canvas.Children.Add(vertex);
		}

		private void AddEdge_OnClick(object sender, RoutedEventArgs e)
		{
			//создается ветвь в режиме редактирования
			IsDrawing = true;
			var edge = new Edge()
			{
				Id = Guid.NewGuid(),
				IsPreview = true
			};
			_currentElement = edge;
			edge.PointCollection.Add(new Point(_verticalLine.X1, _horizontaLine.Y1));
			edge.PreviewMouseLeftButtonDown += CircuitElement_PreviewMouseLeftButtonDown; ;
			edge.OnSelectionChanged += OnSelectionChanged;

			Canvas.Children.Add(edge);
		}

		private void CircuitElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//если элемент является элементом схемы и он выбраный, то отключается возможность отмены выделения
			if (sender is CircuitElement element)
			{
				if (element.IsSelected)
				{
					_canResetSelection = false;
				}
			}

		}

		private void ContextMenu_OnOpened(object sender, RoutedEventArgs e)
		{
			if (AddEdgeItem.Visibility == Visibility.Collapsed &&
				_elements.Count(a => a is Vertex) >= 2)
			{
				AddEdgeItem.Visibility = Visibility.Visible;
			}

			if (AddEdgeItem.Visibility == Visibility.Visible &&
				_elements.Count(a => a is Vertex) < 2)
			{
				AddEdgeItem.Visibility = Visibility.Collapsed;
			}

			if (SelectedElement != null)
			{
				DeleteElement.Visibility = Visibility.Visible;
			}
			else
			{
				DeleteElement.Visibility = Visibility.Collapsed;
			}

			if (_currentElement != null)
			{
				Canvas.Children.Remove(_currentElement);
				IsDrawing = false;
			}
		}

		private void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ResetSelectedElement();
			SelectedElement = d as CircuitElement;
			_canResetSelection = false;
		}

		private void ResetSelectedElement()
		{
			if (SelectedElement != null)
			{
				SelectedElement.IsSelected = false;
				SelectedElement = null;
			}
		}

		private int Round(double a) => (int)Math.Round((a - 5) / 10) * 10;


		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void DeleteElement_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedElement != null)
			{
				Canvas.Children.Remove(SelectedElement);

				if (SelectedElement is Vertex vertex)
				{
					VerticesSource?.Remove(vertex);
				}

				if (SelectedElement is Edge edge)
				{
					EdgesSource?.Remove(edge);
				}
			}
		}
	}
}
