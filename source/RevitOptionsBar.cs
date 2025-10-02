using Autodesk.Windows;
using BIMPlugins.Bars.Extensions;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UIFramework;

namespace BIMPlugins.Bars
{
    public static class RevitOptionsBar
    {
        private static readonly Grid RootGrid;
        private static ContentPresenter _panelPresenter;
        private static readonly FrameworkElement InternalToolPanel;

        static RevitOptionsBar()
        {
            RootGrid = ComponentManager.Ribbon.FindParent<Grid>("rootGrid");
            if (RootGrid is null) throw new InvalidOperationException("Cannot find root grid in Revit UI");

            InternalToolPanel = RootGrid.FindChild<DialogBarControl>(string.Empty);
            if (InternalToolPanel is null) throw new InvalidOperationException("Cannot find internal tool panel in Revit UI");
        }

        public static void Show()
        {
            if (_panelPresenter is null) return;

            _panelPresenter.Visibility = Visibility.Visible;

            InternalToolPanel.Height = 0;
        }
        public static void Show(FrameworkElement content)
        {
            if (!(_panelPresenter is null))
            {
                _panelPresenter.Content = content;
                _panelPresenter.Visibility = Visibility.Visible;
                InternalToolPanel.Height = 0;
                return;
            }

            _panelPresenter = CreateOptionsBar();
            _panelPresenter.Content = content;

            InternalToolPanel.Height = 0;
        }

        public static void Hide(bool saveContent = false)
        {
            if (_panelPresenter is null) return;

            if (!saveContent)
            {
                _panelPresenter.Content = null;
            }
            _panelPresenter.Visibility = Visibility.Collapsed;

            InternalToolPanel.Height = 26;
        }
        public static async void Hide(TimeSpan delay)
        {
            await Task.Delay(delay);
            Hide();
        }

        private static ContentPresenter CreateOptionsBar()
        {
            const int panelRow = 2;

            RootGrid.RowDefinitions.Insert(2, new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Auto)
            });

            foreach (UIElement child in RootGrid.Children)
            {
                var row = Grid.GetRow(child);
                if (row > 1) Grid.SetRow(child, row + 1);
            }

            var panelPresenter = new ContentPresenter();
            Grid.SetRow(panelPresenter, panelRow);
            RootGrid.Children.Add(panelPresenter);

            return panelPresenter;
        }
    }
}