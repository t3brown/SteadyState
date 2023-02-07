using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools.Extension;
using SteadyState.MainProject.WPF.Views.ViewsSettings;

namespace SteadyState.MainProject.WPF.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            AllSettins();
        }

        private void AllSettins()
        {
            SettingContent.Children.Add(new Label()
            {
                Content = "Единицы измерения",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
                Background = new SolidColorBrush(Color.FromArgb(255, 50, 109, 242)),
            });
            SettingContent.Children.Add(new Label()
            {
                Content = "Узлы",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(UnitVertexSetting);
            SettingContent.Children.Add(new Divider()
            {
                Margin = new System.Windows.Thickness(0),
                Height = 1,
            });
            SettingContent.Children.Add(new Label()
            {
                Content = "Ветви",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(UnitEdgeSetting);

            SettingContent.Children.Add(new Divider()
            {
                Margin = new System.Windows.Thickness(0),
                Height = 1,
            });

            SettingContent.Children.Add(new Label()
            {
                Content = "Точность отображения",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
                Background = new SolidColorBrush(Color.FromArgb(255, 50, 109, 242)),
                Foreground = Brushes.White,
            });
            SettingContent.Children.Add(new Label()
            {
                Content = "Узлы",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(AccuracyVertexSetting);
            SettingContent.Children.Add(new Divider()
            {
                Margin = new System.Windows.Thickness(0),
                Height = 1,
            });
            SettingContent.Children.Add(new Label()
            {
                Content = "Ветви",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(AccuracyEdgeSetting);

            SettingContent.Children.Add(new Divider()
            {
                Margin = new System.Windows.Thickness(0),
                Height = 1,
            });

            SettingContent.Children.Add(new Label()
            {
                Content = "Видимость",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
                Background = new SolidColorBrush(Color.FromArgb(255, 50, 109, 242)),
                Foreground = Brushes.White,
            }); ;
            SettingContent.Children.Add(new Label()
            {
                Content = "Узлы",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(VisibilityVertexSetting);
            SettingContent.Children.Add(new Divider()
            {
                Margin = new System.Windows.Thickness(0),
                Height = 1,
            });
            SettingContent.Children.Add(new Label()
            {
                Content = "Ветви",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(VisibilityEdgeSetting);

            SettingContent.Children.Add(new Label()
            {
                Content = "Прочее",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
                Background = new SolidColorBrush(Color.FromArgb(255, 50, 109, 242)),
                Foreground = Brushes.White,
            });
            SettingContent.Children.Add(AnySetting);
        }

        #region Единицы измерения

        UnitVertexSetting UnitVertexSetting = new UnitVertexSetting();
        UnitEdgeSetting UnitEdgeSetting = new UnitEdgeSetting();

        private void UnitSetting_Click(object sender, MouseButtonEventArgs e)
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(new Label()
            {
                Content = "Узлы",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(UnitVertexSetting);
            SettingContent.Children.Add(new Divider()
            {
                Margin = new System.Windows.Thickness(0),
                Height = 1,
            });
            SettingContent.Children.Add(new Label()
            {
                Content = "Ветви",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(UnitEdgeSetting);
        }

        private void UnitVertexSetting_Click(object sender, MouseButtonEventArgs e)
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(UnitVertexSetting);
        }

        private void UnitEdgeSetting_Click(object sender, MouseButtonEventArgs e)
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(UnitEdgeSetting);
        }

        #endregion

        #region Точность отображения

        AccuracyVertexSetting AccuracyVertexSetting = new AccuracyVertexSetting();
        AccuracyEdgeSetting AccuracyEdgeSetting = new AccuracyEdgeSetting();

        private void AccuracySetting_Click(object sender, MouseButtonEventArgs e)
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(new Label()
            {
                Content = "Узлы",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(AccuracyVertexSetting);
            SettingContent.Children.Add(new Divider()
            {
                Margin = new System.Windows.Thickness(0),
                Height = 1,
            });
            SettingContent.Children.Add(new Label()
            {
                Content = "Ветви",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(AccuracyEdgeSetting);
        }

        private void AccuracyVertexSetting_Click(object sender, MouseButtonEventArgs e)
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(AccuracyVertexSetting);
        }

        private void AccuracyEdgeSetting_Click(object sender, MouseButtonEventArgs e)
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(AccuracyEdgeSetting);
        }

        #endregion

        #region Видимость

        VisibilityVertexSetting VisibilityVertexSetting = new VisibilityVertexSetting();
        VisibilityEdgeSetting VisibilityEdgeSetting = new VisibilityEdgeSetting();

        private void VisibilityVertexSetting_Click(object sender, MouseButtonEventArgs e)
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(VisibilityVertexSetting);
        }

        private void VisibilityEdgeSetting_Click(object sender, MouseButtonEventArgs e) 
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(VisibilityEdgeSetting);
        }

        private void VisibilitySetting_Click(object sender, MouseButtonEventArgs e)
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(new Label()
            {
                Content = "Узлы",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(VisibilityVertexSetting);
            SettingContent.Children.Add(new Divider()
            {
                Margin = new System.Windows.Thickness(0),
                Height = 1,
            });
            SettingContent.Children.Add(new Label()
            {
                Content = "Ветви",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                BorderThickness = new System.Windows.Thickness(0),
            });
            SettingContent.Children.Add(VisibilityEdgeSetting);
        }

        #endregion




        AnySetting AnySetting = new AnySetting();

        private void AnySetting_Click(object sender, MouseButtonEventArgs e)
        {
            SettingContent.Children.Clear();
            SettingContent.Children.Add(AnySetting);
        }

    }
}
