using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SteadyState.MainProject.WPF.Infrastructure;
using SteadyState.MainProject.WPF.Models;

namespace SteadyState.MainProject.WPF.Converters
{
	public class DoubleConverter: Freezable, IValueConverter
    {
        #region Units: Units - свойство зависимости единиц измерения

        private static readonly DependencyProperty UnitsDataProperty = DependencyProperty.Register(nameof(Units), typeof(Units), typeof(DoubleConverter));
        public Units Units
        {
            get => (Units)GetValue(UnitsDataProperty);
            set => SetValue(UnitsDataProperty, value);
        }

        #endregion

        #region DisplayPrecision: DisplayPrecision - свойство зависимости точности отображения

        private static readonly DependencyProperty DisplayPrecisionDataProperty = DependencyProperty.Register(nameof(DisplayPrecision), typeof(DisplayPrecision), typeof(DoubleConverter));
        public DisplayPrecision DisplayPrecision
        {
            get => (DisplayPrecision)GetValue(DisplayPrecisionDataProperty);
            set => SetValue(DisplayPrecisionDataProperty, value);
        }

        #endregion

        #region IsRelative: bool - свойство зависимости проверки на расчет в относительных единицах

        private static readonly DependencyProperty IsRelativeProperty = DependencyProperty.Register(nameof(IsRelative), typeof(bool), typeof(DoubleConverter));
        public bool IsRelative
        {
            get => (bool)GetValue(IsRelativeProperty);
            set => SetValue(IsRelativeProperty, value);
        }

        #endregion

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && value != null && DisplayPrecision != null)

                return Math.Round((double)value / ConvertUnit(Units.GetType().GetProperty(parameter.ToString()).GetValue(Units).ToString()),
                    (byte)DisplayPrecision.GetType().GetProperty(parameter.ToString()).GetValue(DisplayPrecision),
                    MidpointRounding.AwayFromZero);
            else return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null)
            {
                if (value.ToString() == "")
                    return null;
                else
                {
                    double cf = parameter != null ? ConvertUnit(Units.GetType().GetProperty(parameter.ToString()).GetValue(Units).ToString()) : 1;
                    double d;
                    if (value.ToString().Contains('.'))
                    {
                        if (double.TryParse(value.ToString().Replace('.', ','), out d))
                            return d * cf;
                        return 0;
                    }
                    else if (double.TryParse((string)value, out d))
                        return d * cf;
                    return 0;
                }
            }
            else return null;
        }

        private double ConvertUnit(string value)
        {
            var coef = value switch
            {
                "В" => 0.001,
                "кА" => 1000,
                "кВт" => 0.001,
                "квар" => 0.001,
                "рад" => 180 / Math.PI,
                _ => 1
            };
            return coef;
        }

        protected override Freezable CreateInstanceCore()
        {
	        return new BindingProxy();
        }
    }
}
