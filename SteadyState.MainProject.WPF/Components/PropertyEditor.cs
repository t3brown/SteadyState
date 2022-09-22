
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using HandyControl.Data;

namespace SteadyState.MainProject.WPF.Components
{
	public class PropertyEditor : Control
	{

		static PropertyEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyEditor),
				new FrameworkPropertyMetadata(typeof(PropertyEditor)));
		}


		#region свойства зависимости

		#region только для чтения

		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly),
			typeof(bool),
			typeof(PropertyEditor),
			new FrameworkPropertyMetadata(false,
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public bool IsReadOnly
		{
			get => (bool)GetValue(IsReadOnlyProperty);
			set => SetValue(IsReadOnlyProperty, value);
		}

		#endregion

		#region значение свойства

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value),
			typeof(object),
			typeof(PropertyEditor),
			new FrameworkPropertyMetadata(default,
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		#endregion

		#region название свойства

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title),
			typeof(string),
			typeof(PropertyEditor),
			new FrameworkPropertyMetadata(default,
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		#endregion

		#region коллекция для перечисления

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource),
			typeof(IEnumerable<EnumItem>), 
			typeof(PropertyEditor),
			new PropertyMetadata(default(IEnumerable<EnumItem>)));

		public IEnumerable<EnumItem> ItemsSource
		{
			get { return (IEnumerable<EnumItem>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		#endregion

		#endregion

		#region комманды

		#region двойной клик на TextBox

		public static readonly DependencyProperty MouseDoubleClickCommandProperty = DependencyProperty.Register(nameof(MouseDoubleClickCommand),
			typeof(ICommand),
			typeof(PropertyEditor),
			new FrameworkPropertyMetadata(default));

		public ICommand MouseDoubleClickCommand
		{
			get => (ICommand)GetValue(MouseDoubleClickCommandProperty);
			set => SetValue(MouseDoubleClickCommandProperty, value);
		}

		public static readonly DependencyProperty MouseDoubleClickCommandParameterProperty = DependencyProperty.Register(nameof(MouseDoubleClickCommandParameter),
			typeof(object),
			typeof(PropertyEditor),
			new FrameworkPropertyMetadata(default));

		public object MouseDoubleClickCommandParameter
		{
			get => GetValue(MouseDoubleClickCommandParameterProperty);
			set => SetValue(MouseDoubleClickCommandParameterProperty, value);
		}

		#endregion

		#endregion
	}
}
