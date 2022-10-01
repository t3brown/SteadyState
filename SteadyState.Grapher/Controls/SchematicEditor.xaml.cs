using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
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
using System.Diagnostics;
using System.Threading.Tasks;
using SteadyState.Grapher.Helpers;
using System.Windows.Media.Media3D;
using Newtonsoft.Json.Linq;

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

					if (_currentElement != null)
					{
						_currentElement.IsPreview = false;
						_setCurrentElementPoint = null;
					}

					_currentElement = null;

					//Cursor = Cursors.Arrow;
				}
				if (value)
				{
					_horizontaLine.Visibility = Visibility.Visible;
					_verticalLine.Visibility = Visibility.Visible;

					//Cursor = Cursors.None;
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

		public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register(
			nameof(SelectedElement), typeof(CircuitElement), typeof(SchematicEditor), new PropertyMetadata(default(CircuitElement)));

		public CircuitElement SelectedElement
		{
			get { return (CircuitElement)GetValue(SelectedElementProperty); }
			set { SetValue(SelectedElementProperty, value); }
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
			"VerticesSource", typeof(ICollection<Vertex>),
			typeof(SchematicEditor),
			new FrameworkPropertyMetadata(null,
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				new PropertyChangedCallback(VerticesSourceProperty_Changed)));

		private static void VerticesSourceProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
			{
				if (d is SchematicEditor editor)
				{
					if (editor.VerticesSource is ObservableCollection<Vertex> vertices)
					{
						vertices.CollectionChanged += editor.VerticesSource_CollectionChanged;
					}
				}
			}
		}

		public ICollection<Vertex>? VerticesSource
		{
			get { return (ICollection<Vertex>)GetValue(VerticesSourceProperty); }
			set { SetValue(VerticesSourceProperty, value); }
		}

		#endregion

		#region Итсточник ветвей.

		public static readonly DependencyProperty EdgesSourceProperty = DependencyProperty.Register(
			"EdgesSource", typeof(ICollection<Edge>),
			typeof(SchematicEditor),
			new FrameworkPropertyMetadata(null,
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				new PropertyChangedCallback(EdgesSourceProperty_Changed)));

		private static void EdgesSourceProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != null)
			{
				if (d is SchematicEditor editor)
				{
					if (editor.EdgesSource is ObservableCollection<Edge> edges)
					{
						edges.CollectionChanged += editor.EdgesSource_CollectionChanged;
					}
				}
			}
		}

		public ICollection<Edge>? EdgesSource
		{
			get { return (ICollection<Edge>)GetValue(EdgesSourceProperty); }
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
			Slider.ValueChanged += Slider_OnSliderValueChanged;
			Slider.Value = 4;

			ScrollViewer.ScrollToRightEnd();

			OnElementAdd += SchematicEditor_OnElementAdd;

			Loaded += SchematicEditor_Loaded;
		}

		private bool _isFirstLoaded = true;


		private void SchematicEditor_Loaded(object sender, RoutedEventArgs e)
		{
			if (_isFirstLoaded)
			{
				ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ScrollableHeight / 2);
				ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.ScrollableWidth / 2);

				_isFirstLoaded = false;
			}
		}

		//vertex.MouseLeftButtonDown += Vertex_MouseLeftButtonDown;
		private async void VerticesSource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems?[0] is Vertex vertex)
				{
					if (!string.IsNullOrEmpty(vertex.Title) && vertex.Title.Contains("temp"))
					{
						goto m1;
					}

					//Нет на схеме и не создан через редактор
					if (!Canvas.Children.Contains(vertex) && !vertex.IsCreatedByDataGrid)
					{
						Canvas.Children.Add(vertex);
						Canvas.SetLeft(vertex, vertex.StartPoint.X);
						Canvas.SetTop(vertex, vertex.StartPoint.Y);
					}

					if (!vertex.IsCreatedByDataGrid)
					{
						vertex.PreviewMouseLeftButtonDown += CircuitElement_PreviewMouseLeftButtonDown;
						vertex.OnSelectionChanged += OnSelectionChanged;
					}
					m1:
					vertex.BasicVertexChanged += Vertex_BasicVertexChanged;

					if (vertex.Index == 0 && VerticesSource != null)
					{
						await CalculateSteadyState.SetIndex(VerticesSource, vertex);
					}
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems?[0] is Vertex vertex)
				{
					if (Canvas.Children.Contains(vertex))
					{
						Canvas.Children.Remove(vertex);
					}
				}
			}
		}

		private void Vertex_BasicVertexChanged(IVertex obj)
		{
			if (!obj.IsBasic)
			{
				BasicVertex = null;
				//не передавая узел, сбрасывает IsConnected.
				DepthFirstSearch.DFS();

				return;
			}

			if (BasicVertex != null)
			{
				BasicVertex.IsBasic = false;
			}

			BasicVertex = obj;
			DepthFirstSearch.DFS(BasicVertex);
		}

		private async void EdgesSource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems?[0] is Edge edge)
				{
					//Нет на схеме и не создан через редактор.
					if (!Canvas.Children.Contains(edge) && !edge.IsCreatedByDataGrid)
					{
						Canvas.Children.Add(edge);
					}

					if (!edge.IsCreatedByDataGrid)
					{
						edge.PreviewMouseLeftButtonDown += CircuitElement_PreviewMouseLeftButtonDown;
						edge.OnSelectionChanged += OnSelectionChanged;
					}

					edge.SwitchPositionChanged += Edge_OnSwitchPositionChanged;
					edge.VertexChanged += Edge_VertexChanged;

					if (edge.Index == 0 && EdgesSource != null)
					{
						await CalculateSteadyState.SetIndex(EdgesSource, edge);
					}
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems?[0] is Edge edge)
				{
					if (Canvas.Children.Contains(edge))
					{
						Canvas.Children.Remove(edge);
					}
				}
			}
		}

		private static void Edge_VertexChanged()
		{
			if (BasicVertex != null)
			{
				DepthFirstSearch.DFS(BasicVertex);
			}
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
				}
			}

			if (obj is Edge edge)
			{

				if (EdgesSource != null)
				{
					EdgesSource.Add(edge);
				}

				edge.OldV1Id = edge.V1Id;
				edge.OldV1 = edge.V1;

				edge.OldV2Id = edge.V2Id;
				edge.OldV2 = edge.V2;

				////запускается поиск в глубину при добавлении новой ветви
				//if (edge.V1.IsConnected || edge.V2.IsConnected)
				//	if (BasicVertex != null)
				//		DepthFirstSearch.DFS(BasicVertex);

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
							VerticesSource?.Remove((Vertex)edge.V1);
							edge.V1Id = edge.OldV1Id;
							edge.V1 = edge.OldV1;
							break;

						case Switch.Q2:
							VerticesSource?.Remove((Vertex)edge.V2);
							edge.V2Id = edge.OldV2Id;
							edge.V2 = edge.OldV2;
							break;
					}
					break;

				case SwitchPosition.Off:
					var vertex = new Vertex();
					switch (sw)
					{
						case Switch.Q1:
							edge.OldV1Id = edge.V1Id;
							edge.OldV1 = edge.V1;
							VerticesSource?.Add(vertex);

							if (edge.V2.IsGround)
							{
								vertex.VoltNom = edge.OldV1.VoltNom;
							}
							else
							{
								vertex.VoltNom = edge.U1 is null && edge.U2 is null ? edge.V2.VoltNom : edge.V2.VoltNom * edge.U1 / edge.U2;
							}

							edge.V1Id = vertex.Id;
							edge.V1 = vertex;

							vertex.Title = $"{edge.OldV1.Index}.{edge.OldV1.Title}_temp";
							break;

						case Switch.Q2:
							edge.OldV2Id = edge.V2Id;
							edge.OldV2 = edge.V2;
							VerticesSource?.Add(vertex);

							if (edge.V1.IsGround)
							{
								vertex.VoltNom = edge.OldV2.VoltNom;
							}
							else
							{
								vertex.VoltNom = edge.U1 is null && edge.U2 is null ? edge.V1.VoltNom : edge.V1.VoltNom * edge.U2 / edge.U1;
							}

							edge.V2Id = vertex.Id;
							edge.V2 = vertex;

							vertex.Title = $"{edge.OldV2.Index}.{edge.OldV2.Title}_temp";
							break;
					}

					break;
			}

			////запускается поиск в глубину при переключениях 
			//if (BasicVertex != null)
			//{
			//	DepthFirstSearch.DFS(BasicVertex);
			//}
		}

		#region zoom and sroll

		private void ScrollViewer_OnMouseMove(object sender, MouseEventArgs e)
		{
			if (_lastDragPoint.HasValue)
			{
				var posNow = e.GetPosition(ScrollViewer);

				var dX = posNow.X - _lastDragPoint.Value.X;
				var dY = posNow.Y - _lastDragPoint.Value.Y;

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
					Slider.Value += Slider.Value < 1 ? Slider.Value < 0.5 ? 0.01 : 0.02 : 0.1;
				}

				if (e.Delta < 0)
				{
					Slider.Value -= Slider.Value < 1 ? Slider.Value < 0.5 ? 0.01 : 0.02 : 0.1;
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
						var centerOfTargetNow = ScrollViewer.TranslatePoint(centerOfViewport, Grid);

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
					var dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
					var dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

					var multiplicatorX = e.ExtentWidth / Grid.Width;
					var multiplicatorY = e.ExtentHeight / Grid.Height;

					var newOffsetX = ScrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
					var newOffsetY = ScrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

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

		/// <summary>
		/// Объект, по которому попал луч (вершина)
		/// </summary>
		private DependencyObject? _hitObject;

		private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			VisualTreeHelper.HitTest(Canvas,
				VertexHitTestFilter,
				MyHitTestResult,
				new PointHitTestParameters(new Point(_verticalLine.X1, _horizontaLine.Y1)));


			if (_hitObject is Vertex hetVertex)
			{
				_hitObject = null;

				if (SetVerticesForEdge(hetVertex))
				{
					//выход из метода если узел установился.
					return;
				}
			}

			if (_currentElement == null)
			{
				//отмена выделения выбранного элемента, при условии что элемент не был выбран только что
				if (_canResetSelection)
					ResetSelectedElement();
				_canResetSelection = true;
			}

			//выполняется в том случае, если есть текущий элемент для редактирования и для него задана начальная точка
			if (_currentElement != null && _setCurrentElementPoint.HasValue)
			{
				//если это вершина, то редактирование прекращается
				if (_currentElement is Vertex vertex)
				{
					IsDrawing = false;
					OnElementAdd?.Invoke(vertex);
				}
				//если это ветвь, то добавляется следующая точка, а предыдущая становится начальной
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
					vertex.StartPoint = (Point)_setCurrentElementPoint;
				}
			}
		}

		private HitTestFilterBehavior VertexHitTestFilter(DependencyObject obj)
		{
			// Test for the object value you want to filter.
			if (obj.GetType() == typeof(Vertex))
			{
				_hitObject = obj;

				// Visual object and descendants are NOT part of hit test results enumeration.
				return HitTestFilterBehavior.Stop;
			}

			// Visual object is part of hit test results enumeration.
			return HitTestFilterBehavior.ContinueSkipSelf;
		}

		private HitTestResultBehavior MyHitTestResult(HitTestResult result)
		{
			// Add the hit test result to the list that will be processed after the enumeration.
			//_hitResultsList.Add(result.VisualHit);

			// Set the behavior to return visuals at all z-order levels.
			return HitTestResultBehavior.Continue;
		}

		/// <summary>
		/// Устанавливает узлы для ветви.
		/// </summary>
		/// <param name="vertex">Узел.</param>
		/// <returns>true, если установился узел, иначе false</returns>
		private bool SetVerticesForEdge(Vertex vertex)
		{
			//если имеется элемент для редактирования и для него задана начальная точка
			if (_currentElement != null && _setCurrentElementPoint.HasValue)
			{
				//если этот элемент ветвь, то редактирование прекращается
				if (_currentElement is Edge edge)
				{
					if (edge.V1Id != vertex.Id)
					{
						//если последняя точка ветви не равняется точке курса, то добавляется конечная точка в позиции курсора
						if (edge.PointCollection[^1] != new Point(_verticalLine.X1, _horizontaLine.Y1))
						{
							edge.PointCollection.Add(new Point(_verticalLine.X1, _horizontaLine.Y1));
						}

						edge.V2Id = vertex.Id;
						edge.V2 = vertex;

						OnElementAdd?.Invoke(edge);
						IsDrawing = false;

						if (BasicVertex != null)
						{
							DepthFirstSearch.DFS(BasicVertex);
						}

						return true;
					}

				}
			}
			//если имеется элемент для редактирования и для него не задана начальная точка
			if (_currentElement != null && !_setCurrentElementPoint.HasValue)
			{
				//если этот элемент ветвь, то для нее добавляется точка в позиции курсора, а предыдущая точка становится начальной
				if (_currentElement is Edge edge)
				{
					edge.PointCollection.Add(new Point(_verticalLine.X1, _horizontaLine.Y1));
					_setCurrentElementPoint = edge.PointCollection[^2];

					edge.V1Id = vertex.Id;
					edge.V1 = vertex;

					return true;

				}
			}

			return false;
		}

		private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
		{
			var mousePosition = e.GetPosition(Canvas);
			_horizontaLine.Y1 = Round(mousePosition.Y) + 5;
			_horizontaLine.Y2 = Round(mousePosition.Y) + 5;
			_verticalLine.X1 = Round(mousePosition.X) + 5;
			_verticalLine.X2 = Round(mousePosition.X) + 5;

			/* если имеется элемент для редактирования и для него задана начальная точка, то расчитывается
			 изменение положения курсора относительно начальной точки */
			if (_currentElement != null && _setCurrentElementPoint.HasValue)
			{
				var dX = _verticalLine.X1 - _setCurrentElementPoint.Value.X - 5;
				var dY = _horizontaLine.Y1 - _setCurrentElementPoint.Value.Y - 5;
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
				Width = 10,
				IsPreview = true,
			};
			_currentElement = vertex;
			Canvas.SetTop(vertex, _horizontaLine.Y1 - 5);
			Canvas.SetLeft(vertex, _verticalLine.X1 - 5);
			//vertex.PreviewMouseLeftButtonDown += CircuitElement_PreviewMouseLeftButtonDown;
			//vertex.MouseLeftButtonDown += Vertex_MouseLeftButtonDown;
			//vertex.OnSelectionChanged += OnSelectionChanged;

			Canvas.Children.Add(vertex);
		}

		private void AddEdge_OnClick(object sender, RoutedEventArgs e)
		{
			//создается ветвь в режиме редактирования
			IsDrawing = true;
			var edge = new Edge()
			{
				IsPreview = true,
			};
			_currentElement = edge;
			edge.PointCollection.Add(new Point(_verticalLine.X1, _horizontaLine.Y1));
			//edge.PreviewMouseLeftButtonDown += CircuitElement_PreviewMouseLeftButtonDown; ;
			//edge.OnSelectionChanged += OnSelectionChanged;
			//edge.VertexChanged += Edge_VertexChanged;

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
