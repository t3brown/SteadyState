using System;
using System.Collections.Generic;
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

namespace SteadyState.MainProject.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для ElectricalCircuit.xaml
    /// </summary>
    public partial class ElectricalCircuit : UserControl
    {
        #region StrokeThickness

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness",
                typeof(double),
                typeof(ElectricalCircuit),
                new PropertyMetadata(3.5, OnValueStrokeThicknessPropertyChanged));

        private static void OnValueStrokeThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eCircuit = d as ElectricalCircuit;
            double newValue = (double)e.NewValue;
            eCircuit.start_node.StrokeThickness = newValue;
            eCircuit.start_node_ch_pwr.StrokeThickness = newValue;
            eCircuit.resistor.StrokeThickness = newValue;
            eCircuit.inductance.StrokeThickness = newValue;
            eCircuit.transformer.StrokeThickness = newValue;
            eCircuit.end_node.StrokeThickness = newValue;
            eCircuit.ground_noud.StrokeThickness = newValue;
            eCircuit.end_node_ch_pwr.StrokeThickness = newValue;
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        #endregion

        #region Stroke

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke",
                typeof(Brush),
                typeof(ElectricalCircuit),
                new PropertyMetadata(Brushes.Red, OnValueStrokePropertyChanged));

        private static void OnValueStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eCircuit = d as ElectricalCircuit;
            Brush newValue = (Brush)e.NewValue;
            eCircuit.start_node.Stroke = newValue;
            eCircuit.start_node.Fill = newValue;
            eCircuit.start_node_ch_pwr.Stroke = newValue;
            eCircuit.start_node_ch_pwr.Fill = newValue;
            eCircuit.resistor.Stroke = newValue;
            eCircuit.inductance.Stroke = newValue;
            eCircuit.transformer.Stroke = newValue;
            eCircuit.end_node.Stroke = newValue;
            eCircuit.ground_noud.Stroke = newValue;
            eCircuit.end_node.Fill = newValue;
            eCircuit.end_node_ch_pwr.Stroke = newValue;
            eCircuit.end_node_ch_pwr.Fill = newValue;
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        #endregion

        #region VisibilityR

        public static readonly DependencyProperty VisibilityRProperty =
            DependencyProperty.Register("VisibilityR",
                typeof(Visibility),
                typeof(ElectricalCircuit),
                new PropertyMetadata(Visibility.Visible, OnValueVisibilityRPropertyChanged));

        private static void OnValueVisibilityRPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eCircuit = d as ElectricalCircuit;
            Visibility newValue = (Visibility)e.NewValue;
            if (newValue == Visibility.Collapsed)
            {
                eCircuit.resistor.Visibility = newValue;
                eCircuit.text_r.Visibility = newValue;

            }
            if (newValue == Visibility.Visible)
            {
                eCircuit.resistor.Visibility = eCircuit.R == null ? Visibility.Collapsed : Visibility.Visible;
                eCircuit.text_r.Visibility = eCircuit.R == null ? Visibility.Collapsed : Visibility.Visible;
                eCircuit.text_r.Text = eCircuit.R.ToString();
            }
        }

        public Visibility VisibilityR
        {
            get { return (Visibility)GetValue(VisibilityRProperty); }
            set { SetValue(VisibilityRProperty, value); }
        }

        #endregion

        #region V2

        public static readonly DependencyProperty V2Property =
            DependencyProperty.Register("V2",
                typeof(string),
                typeof(ElectricalCircuit),
                new PropertyMetadata("", OnValueV2PropertyChanged));

        private static void OnValueV2PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eCircuit = d as ElectricalCircuit;
            if (string.IsNullOrEmpty(eCircuit.V2))
            {
                eCircuit.ground_noud.Visibility = Visibility.Visible;
                eCircuit.end_node.Visibility = Visibility.Collapsed;
            }
            else
            {
                eCircuit.ground_noud.Visibility = Visibility.Collapsed;
                eCircuit.end_node.Visibility = Visibility.Visible;
            }
        }

        public string V2
        {
            get { return (string)GetValue(V2Property); }
            set { SetValue(V2Property, value); }
        }

        #endregion

        #region R

        public static readonly DependencyProperty RProperty =
            DependencyProperty.Register("R",
                typeof(double?),
                typeof(ElectricalCircuit),
                new PropertyMetadata(null, OnValueRPropertyChanged));

        private static void OnValueRPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eCircuit = d as ElectricalCircuit;
            if (eCircuit.VisibilityR == Visibility.Visible)
            {
                double? newValue = (double?)e.NewValue;
                eCircuit.resistor.Visibility = newValue == null ? Visibility.Collapsed : Visibility.Visible;
                eCircuit.text_r.Visibility = newValue == null ? Visibility.Collapsed : Visibility.Visible;
                eCircuit.text_r.Text = eCircuit.R.ToString();
            }
        }

        public double? R
        {
            get { return (double?)GetValue(RProperty); }
            set { SetValue(RProperty, value); }
        }

        #endregion

        #region X

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X",
                typeof(double?),
                typeof(ElectricalCircuit),
                new PropertyMetadata(null, OnValueXPropertyChanged));

        private static void OnValueXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eCircuit = d as ElectricalCircuit;
            double? newValue = (double?)e.NewValue;
            eCircuit.inductance.Visibility = newValue == null ? Visibility.Collapsed : Visibility.Visible;
            eCircuit.text_x.Visibility = newValue == null ? Visibility.Collapsed : Visibility.Visible;
            eCircuit.text_x.Text = $"j{newValue}";
        }

        public double? X
        {
            get { return (double?)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        #endregion

        #region B

        public static readonly DependencyProperty BProperty =
            DependencyProperty.Register("B",
                typeof(double?),
                typeof(ElectricalCircuit),
                new PropertyMetadata(null, OnValueBPropertyChanged));

        private static void OnValueBPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eCircuit = d as ElectricalCircuit;
            double? newValue = (double?)e.NewValue;
            if (newValue != null)
            {
                eCircuit.start_node_ch_pwr.Visibility = Visibility.Visible;
                eCircuit.end_node_ch_pwr.Visibility = Visibility.Visible;
                eCircuit.text_сh_pwr_st.Visibility = Visibility.Visible;
                eCircuit.text_сh_pwr_end.Visibility = Visibility.Visible;
                eCircuit.text_сh_pwr_st.Text = $"j{newValue / 2}";
                eCircuit.text_сh_pwr_end.Text = $"j{newValue / 2}";
            }
            else
            {
                eCircuit.start_node_ch_pwr.Visibility = Visibility.Collapsed;
                eCircuit.end_node_ch_pwr.Visibility = Visibility.Collapsed;
                eCircuit.text_сh_pwr_st.Visibility = Visibility.Collapsed;
                eCircuit.text_сh_pwr_end.Visibility = Visibility.Collapsed;
            }
        }

        public double? B
        {
            get { return (double?)GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }

        #endregion

        #region U1 & U2

        public static readonly DependencyProperty U1Property =
            DependencyProperty.Register("U1",
                typeof(double?),
                typeof(ElectricalCircuit),
                new PropertyMetadata(null, OnCoefPropertyChanged));

        public double? U1
        {
            get { return (double?)GetValue(U1Property); }
            set { SetValue(U1Property, value); }
        }

        public static readonly DependencyProperty U2Property =
            DependencyProperty.Register("U2",
                typeof(double?),
                typeof(ElectricalCircuit),
                new PropertyMetadata(null, OnCoefPropertyChanged));

        public double? U2
        {
            get { return (double?)GetValue(U2Property); }
            set { SetValue(U2Property, value); }
        }

        private static void OnCoefPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eCircuit = d as ElectricalCircuit;
            if (eCircuit.U1 == null || eCircuit.U2 == null)
            {
                eCircuit.transformer.Visibility = Visibility.Collapsed;
                eCircuit.text_cf.Visibility = Visibility.Collapsed;
            }
            if (eCircuit.U1 != null && eCircuit.U2 != null)
            {
                eCircuit.transformer.Visibility = Visibility.Visible;
                eCircuit.text_cf.Visibility = Visibility.Visible;

                double _u1 = (double)eCircuit.U1;
                double _u2 = (double)eCircuit.U2;

                eCircuit.text_cf.Text = $"{_u1}/{_u2}";
            }
        }

        #endregion

        #region PwrStRe & PwrStIm

        public static readonly DependencyProperty PwrStReProperty =
            DependencyProperty.Register("PwrStRe",
                typeof(double?),
                typeof(ElectricalCircuit),
                new PropertyMetadata(null, OnValuePwrStPropertyChanged));

        public double? PwrStRe
        {
            get { return (double?)GetValue(PwrStReProperty); }
            set { SetValue(PwrStReProperty, value); }
        }

        public static readonly DependencyProperty PwrStImProperty =
            DependencyProperty.Register("PwrStIm",
                typeof(double?),
                typeof(ElectricalCircuit),
                new PropertyMetadata(null, OnValuePwrStPropertyChanged));

        public double? PwrStIm
        {
            get { return (double?)GetValue(PwrStImProperty); }
            set { SetValue(PwrStImProperty, value); }
        }

        public static readonly DependencyProperty VisibilityPwrStReProperty =
            DependencyProperty.Register("VisibilityPwrStRe",
                typeof(Visibility),
                typeof(ElectricalCircuit),
                new PropertyMetadata(Visibility.Visible, OnValuePwrStPropertyChanged));

        public Visibility VisibilityPwrStRe
        {
            get { return (Visibility)GetValue(VisibilityPwrStReProperty); }
            set { SetValue(VisibilityPwrStReProperty, value); }
        }

        public static readonly DependencyProperty VisibilityPwrStImProperty =
            DependencyProperty.Register("VisibilityPwrStIm",
                typeof(Visibility),
                typeof(ElectricalCircuit),
                new PropertyMetadata(Visibility.Visible, OnValuePwrStPropertyChanged));

        public Visibility VisibilityPwrStIm
        {
            get { return (Visibility)GetValue(VisibilityPwrStImProperty); }
            set { SetValue(VisibilityPwrStImProperty, value); }
        }
        private static void OnValuePwrStPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ElectricalCircuit eCircuit = d as ElectricalCircuit;
            PwrTextVisibilityChanged(eCircuit.PwrStRe, eCircuit.PwrStIm, eCircuit.VisibilityPwrStRe, eCircuit.VisibilityPwrStIm, eCircuit.arrow_st, eCircuit.callout_left, eCircuit.space_row, eCircuit.text_pwr_st, eCircuit);
        }

        #endregion

        #region PwrEndRe & PwrEndIm

        public static readonly DependencyProperty PwrEndReProperty =
            DependencyProperty.Register("PwrEndRe",
                typeof(double?),
                typeof(ElectricalCircuit),
                new PropertyMetadata(null, OnValuePwrEndPropertyChanged));

        public double? PwrEndRe
        {
            get { return (double?)GetValue(PwrEndReProperty); }
            set { SetValue(PwrEndReProperty, value); }
        }

        public static readonly DependencyProperty PwrEndImProperty =
            DependencyProperty.Register("PwrEndIm",
                typeof(double?),
                typeof(ElectricalCircuit),
                new PropertyMetadata(null, OnValuePwrEndPropertyChanged));

        public double? PwrEndIm
        {
            get { return (double?)GetValue(PwrEndImProperty); }
            set { SetValue(PwrEndImProperty, value); }
        }

        public static readonly DependencyProperty VisibilityPwrEndReProperty =
            DependencyProperty.Register("VisibilityPwrEndRe",
                typeof(Visibility),
                typeof(ElectricalCircuit),
                new PropertyMetadata(Visibility.Visible, OnValuePwrEndPropertyChanged));

        public Visibility VisibilityPwrEndRe
        {
            get { return (Visibility)GetValue(VisibilityPwrEndReProperty); }
            set { SetValue(VisibilityPwrEndReProperty, value); }
        }

        public static readonly DependencyProperty VisibilityPwrEndImProperty =
            DependencyProperty.Register("VisibilityPwrEndIm",
                typeof(Visibility),
                typeof(ElectricalCircuit),
                new PropertyMetadata(Visibility.Visible, OnValuePwrEndPropertyChanged));

        public Visibility VisibilityPwrEndIm
        {
            get { return (Visibility)GetValue(VisibilityPwrEndImProperty); }
            set { SetValue(VisibilityPwrEndImProperty, value); }
        }
        private static void OnValuePwrEndPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ElectricalCircuit eCircuit = d as ElectricalCircuit;
            PwrTextVisibilityChanged(eCircuit.PwrEndRe, eCircuit.PwrEndIm, eCircuit.VisibilityPwrEndRe, eCircuit.VisibilityPwrEndIm, eCircuit.arrow_end, eCircuit.callout_right, eCircuit.space_row, eCircuit.text_pwr_end, eCircuit);
        }

        #endregion

        private static void PwrTextVisibilityChanged(double? pwrRe, double? pwrIm, Visibility visibilityPwrRe, Visibility visibilityPwrIm, Path  arrow, Path callout, RowDefinition spaceRow, TextBlock textBlock, ElectricalCircuit circuit)
        {
            if ((circuit.PwrStRe == null || circuit.VisibilityPwrStRe == Visibility.Collapsed) &&
                (circuit.PwrStIm == null || circuit.VisibilityPwrStIm == Visibility.Collapsed) &&
                (circuit.PwrEndRe == null || circuit.VisibilityPwrEndRe == Visibility.Collapsed) &&
                (circuit.PwrEndIm == null || circuit.VisibilityPwrEndIm == Visibility.Collapsed))
            {
                spaceRow.Height = GridLength.Auto;

                circuit.arrow_st.Visibility = Visibility.Collapsed;
                circuit.arrow_end.Visibility = Visibility.Collapsed;
                circuit.callout_left.Visibility = Visibility.Collapsed;
                circuit.callout_right.Visibility = Visibility.Collapsed;
                circuit.text_pwr_st.Visibility = Visibility.Collapsed;
                circuit.text_pwr_end.Visibility = Visibility.Collapsed;
            }
            if ((pwrRe == null && pwrIm == null) ||
                    (visibilityPwrRe == Visibility.Collapsed && visibilityPwrIm == Visibility.Collapsed))
            {
               // heightSpaceRow = GridLength.Auto;

                arrow.Visibility = Visibility.Collapsed;
                callout.Visibility = Visibility.Collapsed;
                textBlock.Visibility = Visibility.Collapsed;
            }
            if (pwrRe != null && (pwrIm == null || visibilityPwrIm == Visibility.Collapsed) && visibilityPwrRe == Visibility.Visible)
            {
                spaceRow.Height = new GridLength(60);

                arrow.Visibility = Visibility.Visible;
                callout.Visibility = Visibility.Visible;
                textBlock.Visibility = Visibility.Visible;

                double _pwrRe = (double)pwrRe;

                textBlock.Text = _pwrRe.ToString();
            }
            if ((pwrRe == null || visibilityPwrRe == Visibility.Collapsed) && pwrIm != null && visibilityPwrIm == Visibility.Visible)
            {
                spaceRow.Height = new GridLength(60);

                arrow.Visibility = Visibility.Visible;
                callout.Visibility = Visibility.Visible;
                textBlock.Visibility = Visibility.Visible;

                double _pwrIm = (double)pwrIm;

                char sign = _pwrIm < 0 ? '-' : '\0';

                textBlock.Text = $"{sign}j{Math.Abs(_pwrIm)}";
            }
            if (pwrRe != null && pwrIm != null &&
                visibilityPwrRe == Visibility.Visible && visibilityPwrIm == Visibility.Visible)
            {
                spaceRow.Height = new GridLength(60);

                arrow.Visibility = Visibility.Visible;
                callout.Visibility = Visibility.Visible;
                textBlock.Visibility = Visibility.Visible;

                double _pwrRe = (double)pwrRe;

                double _pwrIm = (double)pwrIm;

                char sign = _pwrIm < 0 ? '-' : '+';

                textBlock.Text = $"{_pwrRe}{sign}j{Math.Abs(_pwrIm)}";
            }
        }

        public ElectricalCircuit()
        {
            InitializeComponent();
            this.start_node_ch_pwr.Visibility = Visibility.Collapsed;
            this.end_node_ch_pwr.Visibility = Visibility.Collapsed;
            this.resistor.Visibility = Visibility.Collapsed;
            this.inductance.Visibility = Visibility.Collapsed;
            this.transformer.Visibility = Visibility.Collapsed;
            this.arrow_st.Visibility = Visibility.Collapsed;
            this.arrow_end.Visibility = Visibility.Collapsed;
            this.callout_left.Visibility = Visibility.Collapsed;
            this.callout_right.Visibility = Visibility.Collapsed;
            this.ground_noud.Visibility = Visibility.Collapsed;

            this.text_r.Visibility = Visibility.Collapsed;
            this.text_x.Visibility = Visibility.Collapsed;
            this.text_cf.Visibility = Visibility.Collapsed;
            this.text_сh_pwr_st.Visibility = Visibility.Collapsed;
            this.text_сh_pwr_end.Visibility = Visibility.Collapsed;
            this.text_pwr_st.Visibility = Visibility.Collapsed;
            this.text_pwr_end.Visibility = Visibility.Collapsed;

            this.text_r.Text = "";
            this.text_x.Text = "";
            this.text_cf.Text = "";
            this.text_сh_pwr_st.Text = "";
            this.text_сh_pwr_end.Text = "";
            this.text_pwr_st.Text = "";
            this.text_pwr_end.Text = "";
		}
    }
}
