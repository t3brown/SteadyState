using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SteadyState.MainProject.WPF.Controls
{
    public class ScrollTabControl : TabControl
    {
        //private const int ScrollStep = 15;

        private RepeatButton tabLeftButton;
        private RepeatButton tabRightButton;
        private Button tabAddItemButton;
        private ScrollViewer tabScrollViewer;
        private Panel tabPanelTop;

        static ScrollTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollTabControl), new FrameworkPropertyMetadata(typeof(ScrollTabControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.tabLeftButton = GetTemplateChild("TabLeftButtonTop") as RepeatButton;
            this.tabRightButton = GetTemplateChild("TabRightButtonTop") as RepeatButton;
            this.tabScrollViewer = GetTemplateChild("TabScrollViewerTop") as ScrollViewer;
            this.tabPanelTop = GetTemplateChild("HeaderPanel") as Panel;
            this.tabAddItemButton = GetTemplateChild("TabAddItemButton") as Button;

            if (this.tabLeftButton != null)
                this.tabLeftButton.Click += tabLeftButton_Click;

            if (this.tabRightButton != null)
                this.tabRightButton.Click += tabRightButton_Click;

            if (this.tabAddItemButton != null)
            {
                this.tabAddItemButton.Click += tabAddItemButton_Click;
                this.tabAddItemButton.Visibility = this.isAddItemEnabled ? Visibility.Visible : Visibility.Collapsed;
            }

            this.tabScrollViewer.Loaded += (s, e) => this.UpdateScrollButtonsAvailability();
            this.tabScrollViewer.ScrollChanged += (s, e) => this.UpdateScrollButtonsAvailability();

            this.SelectionChanged += (s, e) => this.ScrollToSelectedItem();

        }


        #region Add item functionality

        private bool isAddItemEnabled;

        public bool IsAddItemEnabled
        {
            get { return isAddItemEnabled; }
            set
            {
                isAddItemEnabled = value;

                if (this.tabAddItemButton != null)
                    this.tabAddItemButton.Visibility = isAddItemEnabled ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public ICommand AddItemCommand
        {
            get { return (ICommand)GetValue(AddItemCommandProperty); }
            set { SetValue(AddItemCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddItemCommandProperty =
            DependencyProperty.Register("AddItemCommand", typeof(ICommand), typeof(ScrollTabControl), new PropertyMetadata(null));


        private void tabAddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.AddItemCommand != null && this.AddItemCommand.CanExecute(null))
                this.AddItemCommand.Execute(null);
        }
        #endregion

        #region Scrollable tabs
        /// <summary>
        /// Gets or sets the Tab Top Left Button style.
        /// </summary>
        /// <value>The left button style style.</value>
        [Description("Gets or sets the tab top left button style")]
        [Category("ScrollButton")]
        public Style TabLeftButtonTopStyle
        {
            get { return (Style)GetValue(TabLeftButtonTopStyleProperty); }
            set { SetValue(TabLeftButtonTopStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Tab Top Right Button style.
        /// </summary>
        /// <value>The left button style style.</value>
        [Description("Gets or sets the tab top right button style")]
        [Category("ScrollButton")]
        public Style TabRightButtonTopStyle
        {
            get { return (Style)GetValue(TabRightButtonTopStyleProperty); }
            set { SetValue(TabRightButtonTopStyleProperty, value); }
        }

        /// <summary>
        /// Tab Top Left Button style
        /// </summary>
        public static readonly DependencyProperty TabLeftButtonTopStyleProperty = DependencyProperty.Register(
            "TabLeftButtonTopStyle",
            typeof(Style),
            typeof(ScrollTabControl),
            new PropertyMetadata(null));

        /// <summary>
        /// Tab Top Left Button style
        /// </summary>
        public static readonly DependencyProperty TabRightButtonTopStyleProperty = DependencyProperty.Register(
            "TabRightButtonTopStyle",
            typeof(Style),
            typeof(ScrollTabControl),
            new PropertyMetadata(null));

        private void tabRightButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != this.tabScrollViewer && null != this.tabPanelTop)
            {
                // added margin left for sure that the item will be scrolled
                var rightItemOffset = Math.Min(this.tabScrollViewer.HorizontalOffset + this.tabScrollViewer.ViewportWidth + this.tabPanelTop.Margin.Left, this.tabScrollViewer.ExtentWidth);

                var rightItem = this.GetItemByOffset(rightItemOffset);
                this.ScrollToItem(rightItem);
            }
        }
        private void tabLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != this.tabScrollViewer)
            {
                var leftItemOffset = Math.Max(this.tabScrollViewer.HorizontalOffset - this.tabPanelTop.Margin.Left, 0);

                var leftItem = this.GetItemByOffset(leftItemOffset);
                this.ScrollToItem(leftItem);
            }

        }

        /// <summary>
        /// Change visibility and avalability of buttons if it is necessary
        /// </summary>
        /// <param name="horizontalOffset">the real offset instead of outdated one from the scroll viewer</param>
        private void UpdateScrollButtonsAvailability(double? horizontalOffset = null)
        {
            if (this.tabScrollViewer == null) return;

            var hOffset = horizontalOffset ?? this.tabScrollViewer.HorizontalOffset;
            hOffset = Math.Max(hOffset, 0);

            var scrWidth = this.tabScrollViewer.ScrollableWidth;
            scrWidth = Math.Max(scrWidth, 0);

            if (this.tabLeftButton != null)
            {
                this.tabLeftButton.Visibility = scrWidth == 0 ? Visibility.Collapsed : Visibility.Visible;

                this.tabLeftButton.IsEnabled = hOffset > 0;
            }
            if (this.tabRightButton != null)
            {
                this.tabRightButton.Visibility = scrWidth == 0 ? Visibility.Collapsed : Visibility.Visible;

                this.tabRightButton.IsEnabled = hOffset < scrWidth;
            }
        }

        /// <summary>
        /// Scrolls to a selected tab
        /// </summary>
        private void ScrollToSelectedItem()
        {
            var model = base.SelectedItem;
            var si = this.ItemContainerGenerator.ContainerFromItem(model) as TabItem;
            if (si == null || this.tabScrollViewer == null)
                return;
            if (si.ActualWidth == 0 && !si.IsLoaded)
            {
                si.Loaded += (s, e) => ScrollToSelectedItem();
                return;
            }

            this.ScrollToItem(si);
        }

        /// <summary>
        /// Scrolls to the specified tab
        /// </summary>
        private void ScrollToItem(TabItem si)
        {
            var tabItems = this.Items.Cast<object>()
                .Select(item => this.ItemContainerGenerator.ContainerFromItem(item) as TabItem);

            var leftItems = tabItems
                .Where(ti => ti != null)
                .TakeWhile(ti => ti != si).ToList();

            var leftItemsWidth = leftItems.Sum(ti => ti.ActualWidth);

            //If the selected item is situated somewhere at the right area
            if (leftItemsWidth + si.ActualWidth > this.tabScrollViewer.HorizontalOffset + this.tabScrollViewer.ViewportWidth)
            {
                var currentHorizontalOffset = (leftItemsWidth + si.ActualWidth) - this.tabScrollViewer.ViewportWidth;
                // the selected item has extra width, so I add it to the offset
                var hMargin = !leftItems.Any(ti => ti.IsSelected) && !si.IsSelected ? this.tabPanelTop.Margin.Left + this.tabPanelTop.Margin.Right : 0;
                currentHorizontalOffset += hMargin;

                this.tabScrollViewer.ScrollToHorizontalOffset(currentHorizontalOffset);
            }
            //if the selected item somewhere at the left
            else if (leftItemsWidth < this.tabScrollViewer.HorizontalOffset)
            {
                var currentHorizontalOffset = leftItemsWidth;
                // the selected item has extra width, so I remove it from the offset
                var hMargin = leftItems.Any(ti => ti.IsSelected) ? this.tabPanelTop.Margin.Left + this.tabPanelTop.Margin.Right : 0;
                currentHorizontalOffset -= hMargin;

                this.tabScrollViewer.ScrollToHorizontalOffset(currentHorizontalOffset);
            }
        }

        /// <summary>
        /// Returns the tab item by using some kind of a hit-test
        /// </summary>
        /// <param name="offset">the absolute coordinate in pixels starting from the left</param>
        private TabItem GetItemByOffset(double offset)
        {
            var tabItems = this.Items.Cast<object>()
                .Select(item => this.ItemContainerGenerator.ContainerFromItem(item) as TabItem)
                .ToList();

            double currentItemsWidth = 0;
            // get tabs one by one and calculate their aggregated width until the offset value is reached
            foreach (var ti in tabItems)
            {
                if (currentItemsWidth + ti.ActualWidth >= offset)
                    return ti;

                currentItemsWidth += ti.ActualWidth;
            }

            return tabItems.LastOrDefault();
        }

        #endregion
    }
}
