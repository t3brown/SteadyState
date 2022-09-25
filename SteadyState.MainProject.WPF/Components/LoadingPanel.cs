using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SteadyState.MainProject.WPF.Components
{
	public class LoadingPanel : Control
	{
		static LoadingPanel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingPanel), new FrameworkPropertyMetadata(typeof(LoadingPanel)));
		}
	}
}
