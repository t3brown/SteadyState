﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SteadyState.MainProject.WPF.Annotations;

namespace SteadyState.MainProject.WPF.Properties
{
    public static class ScrollViewerHelper
    {
        public static readonly DependencyProperty ShiftWheelScrollsHorizontallyProperty
            = DependencyProperty.RegisterAttached("ShiftWheelScrollsHorizontally",
                typeof(bool),
                typeof(ScrollViewerHelper),
                new PropertyMetadata(false, UseHorizontalScrollingChangedCallback));

        private static void UseHorizontalScrollingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;

            if (element == null)
                throw new Exception("Attached property must be used with UIElement.");


            if ((bool)e.NewValue)
                element.PreviewMouseWheel += OnPreviewMouseWheel;
            else
                element.PreviewMouseWheel -= OnPreviewMouseWheel;


        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            ScrollViewer scrollViewer = sender is ScrollViewer viewer ? viewer : ((UIElement)sender).FindDescendant<ScrollViewer>();

            if (scrollViewer == null)
                return;

            if (Keyboard.Modifiers != ModifierKeys.Shift)
                return;

            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + -args.Delta);

            args.Handled = true;
        }

        public static void SetShiftWheelScrollsHorizontally(UIElement element, bool value) => element.SetValue(ShiftWheelScrollsHorizontallyProperty, value);
        public static bool GetShiftWheelScrollsHorizontally(UIElement element) => (bool)element.GetValue(ShiftWheelScrollsHorizontallyProperty);

        [CanBeNull]
        private static T FindDescendant<T>([CanBeNull] this DependencyObject d) where T : DependencyObject
        {
            if (d == null)
                return null;

            var childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);

                var result = child as T ?? FindDescendant<T>(child);

                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
