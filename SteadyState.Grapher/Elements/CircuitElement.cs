using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteadyState.Grapher.Annotations;
using SteadyState.Interfaces;

namespace SteadyState.Grapher.Elements
{
	[JsonObject(MemberSerialization.OptIn)]
	[Serializable]
	public abstract class CircuitElement : Control, IEntity, INotifyPropertyChanged
	{
		#region свойства

		public static readonly DependencyProperty IsConnectedProperty = DependencyProperty.Register(
			"IsConnected", typeof(bool), typeof(CircuitElement), new PropertyMetadata(default(bool)));

		//[JsonProperty]
		public bool IsConnected
		{
			get { return (bool)GetValue(IsConnectedProperty); }
			set { SetValue(IsConnectedProperty, value); }
		}


		public static readonly DependencyProperty IsPreviewProperty = DependencyProperty.Register(
			"IsPreview", typeof(bool), typeof(CircuitElement), new PropertyMetadata(default(bool)));

		public bool IsPreview
		{
			get { return (bool)GetValue(IsPreviewProperty); }
			set { SetValue(IsPreviewProperty, value); }
		}

		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
			"IsSelected", typeof(bool), typeof(CircuitElement),
			new PropertyMetadata(default(bool), PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is CircuitElement circuitElement)
				if (circuitElement.IsSelected)
					circuitElement.OnSelectionChanged?.Invoke(d, e);
		}

		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
			"Stroke", typeof(Brush), typeof(CircuitElement), new PropertyMetadata(Brushes.Gray));

		public Brush Stroke
		{
			get { return (Brush)GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}

		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
			"StrokeThickness", typeof(double), typeof(CircuitElement), new PropertyMetadata(0.5));


		public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
			nameof(Fill), typeof(Brush), typeof(CircuitElement), new PropertyMetadata(Brushes.Transparent));

		public Brush Fill
		{
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		private protected int _index;

		public double StrokeThickness
		{
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}

		public event Action<DependencyObject, DependencyPropertyChangedEventArgs> OnSelectionChanged;

		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		[JsonProperty]
		public Guid Id { get; set; }

		[JsonProperty]
		public int Index
		{
			get => _index;
			set
			{
				if (value == _index) return;
				_index = value;
				OnPropertyChanged();
			}
		}

		private string _title;

		[JsonProperty]
		public string Title
		{
			get => _title;
			set
			{
				if (_title == value) return;
				_title = value;
				OnPropertyChanged();
			}
		}


		public static readonly DependencyProperty IsCreatedByDataGridProperty = DependencyProperty.Register(nameof(IsCreatedByDataGrid),
			typeof(bool),
			typeof(CircuitElement),
			new FrameworkPropertyMetadata(default(bool)));

		[JsonProperty]
		public bool IsCreatedByDataGrid
		{
			get { return (bool)GetValue(IsCreatedByDataGridProperty); }
			set { SetValue(IsCreatedByDataGridProperty, value); }
		}

		#endregion

		public EventHandler CircuitElementClick;

		private void CircuitElement_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			CircuitElementClick?.Invoke(this, EventArgs.Empty);

			if (IsMouseOver && !IsPreview)
				IsSelected = true;
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		protected CircuitElement()
		{
			Id = Guid.NewGuid();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (GetTemplateChild("InteractiveGrid") is Grid grid)
			{
				grid.MouseLeftButtonDown += CircuitElement_MouseLeftButtonDown;
			}

			if (GetTemplateChild("ElementLabel") is Border border)
			{
				border.MouseLeftButtonDown += DraggableBorder_MouseLeftButtonDown;
				border.MouseLeftButtonUp += DraggableBorder_MouseLeftButtonUp;
				border.MouseMove += DraggableBorder_MouseMove;

				if (ElementLabelPoint.HasValue)
				{
					Canvas.SetLeft(border, ElementLabelPoint.Value.X );
					Canvas.SetTop(border, ElementLabelPoint.Value.Y);
				}
			}
		}

		#region ElementLabel

		/// <summary>
		/// Панель отображения параметров в состоянии переноса.
		/// </summary>
		public static readonly DependencyProperty IsDraggingLabelProperty = DependencyProperty.Register(nameof(IsDraggingLabel),
			typeof(bool), typeof(CircuitElement), new PropertyMetadata(default(bool)));

		/// <summary>
		/// Панель отображения параметров в состоянии переноса.
		/// </summary>
		public bool IsDraggingLabel
		{
			get { return (bool)GetValue(IsDraggingLabelProperty); }
			set { SetValue(IsDraggingLabelProperty, value); }
		}

		/// <summary>
		/// Панель отображения параметров в состоянии переноса.
		/// </summary>
		private bool _isDraggingLabel;

		/// <summary>
		/// Смещение курсора относительно панели отображения параметров.
		/// </summary>
		private Point _clickOffsetLabel;


		/// <summary>
		/// Обработчик нажатия по панели отображения параметров.
		/// </summary>
		private void DraggableBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender is not Border draggableControl) return;
			IsDraggingLabel = true;
			_clickOffsetLabel = e.GetPosition(draggableControl);
			draggableControl.CaptureMouse();
		}

		/// <summary>
		/// Обработчик отпускания панели отображения параметров.
		/// </summary>
		private void DraggableBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (sender is not Border draggableControl) return;
			IsDraggingLabel = false;
			draggableControl.ReleaseMouseCapture();
		}

		/// <summary>
		/// Обработчик перемещения мыши при удержании панели отображения параметров.
		/// </summary>
		private void DraggableBorder_MouseMove(object sender, MouseEventArgs e)
		{
			if (!IsDraggingLabel) return;
			var draggableControl = sender as Border;
			var parent = VisualTreeHelper.GetParent(draggableControl) as FrameworkElement;

			if (draggableControl == null || parent == null) return;
			var currentPosition = e.GetPosition(parent);
			ElementLabelPoint = new Point(currentPosition.X - _clickOffsetLabel.X, currentPosition.Y - _clickOffsetLabel.Y);

			Canvas.SetLeft(draggableControl, ElementLabelPoint.Value.X);
			Canvas.SetTop(draggableControl, ElementLabelPoint.Value.Y);
		}

		[JsonProperty]
		public Point? ElementLabelPoint { get; set; }

		#endregion
	}
}
