using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using SteadyState.Interfaces;
using Vector = System.Windows.Vector;

namespace SteadyState.Grapher.Elements
{
    public enum Switch
    {
        Q1,Q2
    }
    public enum SwitchPosition
    {
        Off, On
    }
    public class Edge : CircuitElement , IEdge
    {
        public event Action<IEdge,Switch,SwitchPosition> SwitchPositionChanged;

        public static readonly DependencyProperty PointCollectionProperty = DependencyProperty.Register(
            "PointCollection", typeof(PointCollection ), typeof(Edge), new PropertyMetadata(default(PointCollection )));

        private int _id;
        private bool _on1 = true;
        private bool _on2 = true;
        private IVertex _v1;
        private IVertex _v2;
        private double? _r;
        private double? _x;
        private double? _b;
        private double? _u1;
        private double? _u2;

        private CheckBox Q1;
        private CheckBox Q2;
        private Path Transformer;
        private Ellipse DotV1;
        private Ellipse DotV2;

        public PointCollection PointCollection
        {
            get { return (PointCollection ) GetValue(PointCollectionProperty); }
            set { SetValue(PointCollectionProperty, value); }
        }

        public static readonly DependencyProperty IsTrasnformerProperty = DependencyProperty.Register(
            "IsTrasnformer", typeof(bool), typeof(Edge), new PropertyMetadata(default(bool)));

        public bool IsTrasnformer
        {
            get { return (bool) GetValue(IsTrasnformerProperty); }
            set { SetValue(IsTrasnformerProperty, value); }
        }

        static Edge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Edge), new FrameworkPropertyMetadata(typeof(Edge)));
        }

        public override void OnApplyTemplate()
        {
            Q1 = GetTemplateChild("Q1") as CheckBox;
            Q2 = GetTemplateChild("Q2") as CheckBox;
            Transformer = GetTemplateChild("Transformer") as Path;
            DotV1 = GetTemplateChild("DotV1") as Ellipse;
            DotV2 = GetTemplateChild("DotV2") as Ellipse;
        }

        public Edge()
        {
            PointCollection = new PointCollection();
        }

        public int Id
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public bool On1
        {
            get => _on1;
            set
            {
                if (value == _on1) return;
                _on1 = value;
                var pos = SwitchPosition.Off;
                if (value)
                    pos = SwitchPosition.On;
                SwitchPositionChanged?.Invoke(this, Switch.Q1, pos);
                OnPropertyChanged();
            }
        }

        public bool On2
        {
            get => _on2;
            set
            {
                if (value == _on2) return;
                _on2 = value;
                var pos = SwitchPosition.Off;
                if (value)
                    pos = SwitchPosition.On;
                SwitchPositionChanged?.Invoke(this, Switch.Q2, pos);
                OnPropertyChanged();
            }
        }

        public IVertex V1
        {
            get => _v1;
            set
            {
                if (Equals(value, _v1)) return;
                _v1 = value;
                OnPropertyChanged();
            }
        }

        public IVertex V2
        {
            get => _v2;
            set
            {
                if (Equals(value, _v2)) return;
                _v2 = value;

                DotV1.RenderTransformOrigin = new Point(0.5, 0.5);
                DotV1.RenderTransform = new TranslateTransform(-DotV1.ActualWidth / 2, -DotV1.ActualHeight / 2);
                Canvas.SetLeft(DotV1, PointCollection[0].X);
                Canvas.SetTop(DotV1, PointCollection[0].Y);

                DotV2.RenderTransformOrigin = new Point(0.5, 0.5);
                DotV2.RenderTransform = new TranslateTransform(-DotV2.ActualWidth / 2, -DotV2.ActualHeight / 2);
                Canvas.SetLeft(DotV2, PointCollection[^1].X);
                Canvas.SetTop(DotV2, PointCollection[^1].Y);

                var points = PointCollection.Distinct().ToList();

                Point point1 = points[0];
                Point point2 = points[1];

                Vector vector = Point.Subtract(point2, point1);
                Complex complex = new Complex(vector.X, vector.Y);
                double angle = complex.Phase * 180 / Math.PI;
                

                var pointQ1 = point1;
                var h = Q1.ActualHeight / 2;
                var w = Q1.ActualWidth / 2;
                Q1.RenderTransformOrigin = new Point(0, 0.5);
                TransformGroup transform = new TransformGroup();
               // transform.Children.Add(new ScaleTransform(0.5,0.5));
                transform.Children.Add(new RotateTransform(angle));
                transform.Children.Add(new TranslateTransform(0, -h));
                Q1.RenderTransform = transform;
                Canvas.SetLeft(Q1, pointQ1.X);
                Canvas.SetTop(Q1, pointQ1.Y);


                Point point3 = points[^1];
                Point point4 = points[^2];

                Vector vector2 = Point.Subtract(point4, point3);
                Complex complex2 = new Complex(vector2.X, vector2.Y);
                double angle2 = complex2.Phase * 180 / Math.PI;


                var pointQ2 = point3;
                //var h = Q1.ActualHeight / 2;
                //var w = Q1.ActualWidth / 2;
                Q2.RenderTransformOrigin = new Point(0, 0.5);
                TransformGroup transform1 = new TransformGroup();
                //transform1.Children.Add(new ScaleTransform(0.5, 0.5));
                transform1.Children.Add(new RotateTransform(angle2));
                transform1.Children.Add(new TranslateTransform(0, -h));
                Q2.RenderTransform = transform1;
                Canvas.SetLeft(Q2, pointQ2.X);
                Canvas.SetTop(Q2, pointQ2.Y);

                Vector maxVector = Point.Subtract(points[0], points[1]);
                Complex c1 = new Complex(maxVector.X, maxVector.Y);
                Point point5 = points[0];
                Point point6 = points[1];
                for (int i = 1; i < points.Count - 1; i++)
                {
                  Vector  temp = Point.Subtract(points[i], points[i+1]);
                  Complex c2 = new Complex(temp.X, temp.Y);
                  if (c2.Magnitude > c1.Magnitude)
                  {
                      maxVector = temp;
                      c1 = c2;
                      point5 = points[i];
                      point6 = points[i+1];
                  }
                }
                h = Transformer.ActualHeight / 2;
                w = Transformer.ActualWidth / 2;
                double angle3 = c1.Phase * 180 / Math.PI;
                Transformer.RenderTransformOrigin = new Point(0.5, 0.5);
                TransformGroup transform2 = new TransformGroup();
                //transform1.Children.Add(new ScaleTransform(0.5, 0.5));
                transform2.Children.Add(new RotateTransform(angle3));
                transform2.Children.Add(new TranslateTransform(-w, -h));
                Transformer.RenderTransform = transform2;
                Canvas.SetLeft(Transformer, (point5.X+point6.X)/2);
                Canvas.SetTop(Transformer, (point5.Y + point6.Y) / 2);

                OnPropertyChanged();
            }
        }

        public static readonly DependencyProperty OldV1Property = DependencyProperty.Register(
            "OldV1", typeof(Vertex), typeof(Edge), new PropertyMetadata(default(Vertex)));

        public Vertex OldV1
        {
            get { return (Vertex) GetValue(OldV1Property); }
            set { SetValue(OldV1Property, value); }
        }

        public static readonly DependencyProperty OldV2Property = DependencyProperty.Register(
            "OldV2", typeof(Vertex), typeof(Edge), new PropertyMetadata(default(Vertex)));

        public Vertex OldV2
        {
            get { return (Vertex) GetValue(OldV2Property); }
            set { SetValue(OldV2Property, value); }
        }

        public double? R
        {
            get => _r;
            set
            {
                if (Nullable.Equals(value, _r)) return;
                _r = value;
                OnPropertyChanged();
            }
        }

        public double? X
        {
            get => _x;
            set
            {
                if (Nullable.Equals(value, _x)) return;
                _x = value;
                OnPropertyChanged();
            }
        }

        public double? B
        {
            get => _b;
            set
            {
                if (Nullable.Equals(value, _b)) return;
                _b = value;
                OnPropertyChanged();
            }
        }

        public double? U1
        {
            get => _u1;
            set
            {
                if (Nullable.Equals(value, _u1)) return;
                _u1 = value;
                if (_u1 != null && _u2 != null)
                    IsTrasnformer = true;
                else
                    IsTrasnformer = false;
                OnPropertyChanged();
            }
        }

        public double? U2
        {
            get => _u2;
            set
            {
                if (Nullable.Equals(value, _u2)) return;
                _u2 = value;
                if (_u1 != null && _u2 != null)
                    IsTrasnformer = true;
                else
                    IsTrasnformer = false;
                OnPropertyChanged();
            }
        }

        public double? Angle { get; set; }
        public IRpn Rpn1 { get; set; }
        public IRpn Rpn2 { get; set; }
        public double? ReCoeff { get; set; }
        public double? ImCoeff { get; set; }
        public double? AmpRe { get; set; }
        public double? AmpIm { get; set; }
        public double? AmpMagnitude { get; set; }
        public double? AmpAngle { get; set; }
        public double? PwrStRe { get; set; }
        public double? PwrStIm { get; set; }
        public double? PwrStCh { get; set; }
        public double? PwrEndCh { get; set; }
        public double? PwrEndRe { get; set; }
        public double? PwrEndIm { get; set; }
        public double? PwrDltRe { get; set; }
        public double? PwrDltIm { get; set; }
    }
}
