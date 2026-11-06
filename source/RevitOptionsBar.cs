using Autodesk.Windows;
using BIMPlugins.Bars.Extensions;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UIFramework;

namespace BIMPlugins.Bars
{
    /// <summary>
    /// Shows custom WPF content in the Revit options bar (the panel directly below the ribbon),
    /// in place of the native options bar.
    /// </summary>
    /// <remarks>
    /// <para>
    /// On first use a new row is inserted into the Revit main window grid right below the ribbon,
    /// and a <see cref="ContentPresenter"/> hosting the custom content is placed there. The native
    /// options bar is collapsed while the custom content is shown and restored by <see cref="Hide(bool)"/>.
    /// </para>
    /// <para>
    /// All members must be called on the Revit UI thread.
    /// </para>
    /// <para>
    /// If the Revit main window grid or the native options bar cannot be found, the first access to
    /// this class throws a <see cref="TypeInitializationException"/> wrapping an <see cref="T:System.InvalidOperationException"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var panel = new StackPanel { Orientation = Orientation.Horizontal };
    /// panel.Children.Add(new TextBlock { Text = "Offset:" });
    /// panel.Children.Add(new TextBox { Width = 80 });
    ///
    /// RevitOptionsBar.Show(panel);
    /// // ... user interaction ...
    /// RevitOptionsBar.Hide();
    /// </code>
    /// </example>
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

        /// <summary>
        /// Shows the options bar again with the content it had before it was hidden
        /// with <c>Hide(saveContent: true)</c>.
        /// </summary>
        /// <remarks>Does nothing if <see cref="Show(FrameworkElement)"/> has never been called.</remarks>
        public static void Show()
        {
            if (_panelPresenter is null) return;

            _panelPresenter.Visibility = Visibility.Visible;

            InternalToolPanel.Height = 0;
        }

        /// <summary>
        /// Shows <paramref name="content"/> in the options bar, replacing any previous content,
        /// and collapses the native Revit options bar.
        /// </summary>
        /// <param name="content">WPF element to display.</param>
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

        /// <summary>
        /// Hides the custom content and restores the native Revit options bar.
        /// </summary>
        /// <param name="saveContent">
        /// <see langword="true"/> to keep the current content so it can be shown again with <see cref="Show()"/>;
        /// <see langword="false"/> (default) to discard it.
        /// </param>
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

        /// <summary>
        /// Hides the custom content (discarding it) after the specified delay.
        /// </summary>
        /// <param name="delay">Time to wait before hiding.</param>
        /// <remarks>
        /// This is an <c>async void</c> fire-and-forget method: it returns immediately and cannot be awaited.
        /// </remarks>
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
