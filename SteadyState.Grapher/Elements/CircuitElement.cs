using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SteadyState.Grapher.Annotations;

namespace SteadyState.Grapher.Elements
{
    public abstract class CircuitElement: Control, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsConnectedProperty = DependencyProperty.Register(
            "IsConnected", typeof(bool), typeof(CircuitElement), new PropertyMetadata(default(bool)));

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
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness", typeof(double), typeof(CircuitElement), new PropertyMetadata(0.5));

        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> OnSelectionChanged;

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public CircuitElement()
        {
            MouseLeftButtonDown += CircuitElement_MouseLeftButtonDown;
        }

        private void CircuitElement_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsMouseOver && !IsPreview)
                IsSelected = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
