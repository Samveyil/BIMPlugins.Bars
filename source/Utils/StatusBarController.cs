using BIMPlugins.Bars.Extensions;
using System;
using System.Windows;
using System.Windows.Controls;
using UIFramework;

namespace BIMPlugins.Bars.Utils
{
    /// <summary>
    /// Replaces the native Revit status bar (bottom of the main window) with custom WPF content and restores it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The custom content is placed in a <see cref="ContentPresenter"/> in the same grid row as the native
    /// status bar, which is hidden while the custom content is shown.
    /// </para>
    /// <para>
    /// All members must be called on the Revit UI thread. If the Revit main window grid or the native
    /// status bar cannot be found, the first access to this class throws a <see cref="TypeInitializationException"/>
    /// wrapping an <see cref="T:System.InvalidOperationException"/>.
    /// </para>
    /// </remarks>
    public class StatusBarController
    {
        private static Grid RootGrid;
        private static DialogBarControl InternalControl;
        private static ContentPresenter _controlPresenter;
        static StatusBarController()
        {
            RootGrid = MainWindow.getMainWnd().FindChild<Grid>("rootGrid");
            if (RootGrid is null) throw new InvalidOperationException("Cannot find root grid in Revit UI");

            InternalControl = RootGrid.FindChild<DialogBarControl>("statusBar");
            if (InternalControl is null) throw new InvalidOperationException("Cannot find internal control in Revit UI");
        }

        /// <summary>
        /// <see langword="true"/> if the native Revit status bar is currently visible.
        /// </summary>
        public static bool IsVisible => InternalControl.Visibility == Visibility.Visible;

        /// <summary>
        /// Hides the native status bar and shows <paramref name="content"/> in its place,
        /// replacing any previously shown content.
        /// </summary>
        /// <param name="content">WPF element to display.</param>
        public static void Show(FrameworkElement content)
        {
            InternalControl.Visibility = Visibility.Hidden;

            if (_controlPresenter is null)
            {
                _controlPresenter = new ContentPresenter();
                RootGrid.Children.Add(_controlPresenter);
            }

            _controlPresenter.Content = content;
            Grid.SetRow(_controlPresenter, Grid.GetRow(InternalControl));
        }

        /// <summary>
        /// Removes the custom content and makes the native status bar visible again.
        /// </summary>
        public static void Hide()
        {
            InternalControl.Visibility = Visibility.Visible;
            if (_controlPresenter is null) return;

            RootGrid.Children.Remove(_controlPresenter);
            _controlPresenter = null;
        }
    }
}
