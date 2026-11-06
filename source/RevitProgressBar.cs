using BIMPlugins.Bars.Controls;
using BIMPlugins.Bars.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BIMPlugins.Bars
{
    /// <summary>
    /// Progress bar shown in the Revit status bar while a long-running operation executes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The progress bar replaces the native Revit status bar while it is visible and restores it
    /// when the instance is disposed. While the operation runs, the Revit ribbon is disabled and
    /// a wait cursor is shown.
    /// </para>
    /// <para>
    /// The UI is refreshed by pumping the message loop (<see cref="ApplicationUtils.DoEvents"/>), so the
    /// operation is expected to run on the Revit UI thread (for example inside an <c>IExternalCommand</c>).
    /// </para>
    /// <para>
    /// Always use the class in a <c>using</c> block so the status bar and the ribbon are restored
    /// even if the operation throws.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using (var progressBar = new RevitProgressBar(hasCancelButton: true))
    /// {
    ///     progressBar.Run("Processing elements", elements, element =>
    ///     {
    ///         // work with element
    ///     });
    ///
    ///     if (progressBar.IsCancelling())
    ///     {
    ///         // operation was cancelled by the user
    ///     }
    /// }
    /// </code>
    /// </example>
    public class RevitProgressBar : IDisposable
    {
        private readonly Stopwatch stopwatch;
        private readonly ProgressBarStackPanel progressBarStackPanel;

        /// <summary>
        /// Creates a new progress bar. The bar is not shown immediately: it appears only after
        /// <see cref="InitializeMilliseconds"/> have elapsed and the progress has been updated.
        /// </summary>
        /// <param name="hasCancelButton">
        /// <see langword="true"/> to show a cancel button. Use <see cref="IsCancelling"/> to check whether it was pressed.
        /// </param>
        public RevitProgressBar(bool hasCancelButton = false)
        {
            stopwatch = Stopwatch.StartNew();
            progressBarStackPanel = new ProgressBarStackPanel(hasCancelButton);
            this.ForceToRefresh = StatusBarController.IsVisible;
        }

        /// <summary>
        /// Sets the operation name and executes <paramref name="action"/> <paramref name="count"/> times,
        /// advancing the progress bar after each call.
        /// </summary>
        /// <param name="currentOperation">Text displayed next to the progress bar.</param>
        /// <param name="count">Number of iterations. The maximum value of the bar is set to this number.</param>
        /// <param name="action">Action to execute; receives the zero-based iteration index.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        /// <remarks>The loop stops early when the user presses the cancel button.</remarks>
        public RevitProgressBar Run(string currentOperation, int count, Action<int> action)
        {
            return SetCurrentOperation(currentOperation).Run(count, action);
        }

        /// <summary>
        /// Executes <paramref name="action"/> <paramref name="count"/> times,
        /// advancing the progress bar after each call.
        /// </summary>
        /// <param name="count">Number of iterations. The maximum value of the bar is set to this number.</param>
        /// <param name="action">Action to execute; receives the zero-based iteration index.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        /// <remarks>The loop stops early when the user presses the cancel button.</remarks>
        public RevitProgressBar Run(int count, Action<int> action)
        {
            return Run(Enumerable.Range(0, count), action);
        }

        /// <summary>
        /// Sets the operation name and executes <paramref name="action"/> for every item of
        /// <paramref name="collection"/>, advancing the progress bar after each item.
        /// </summary>
        /// <typeparam name="T">Type of the collection items.</typeparam>
        /// <param name="currentOperation">Text displayed next to the progress bar.</param>
        /// <param name="collection">Items to process. The maximum value of the bar is set to the number of items.</param>
        /// <param name="action">Action to execute for each item.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        /// <remarks>The loop stops early when the user presses the cancel button.</remarks>
        public RevitProgressBar Run<T>(string currentOperation, IEnumerable<T> collection, Action<T> action)
        {
            return SetCurrentOperation(currentOperation).Run(collection, action);
        }

        /// <summary>
        /// Executes <paramref name="action"/> for every item of <paramref name="collection"/>,
        /// advancing the progress bar after each item.
        /// </summary>
        /// <typeparam name="T">Type of the collection items.</typeparam>
        /// <param name="collection">
        /// Items to process. The maximum value of the bar is set to the number of items
        /// (the collection is enumerated once to count it, then once more to process it).
        /// </param>
        /// <param name="action">Action to execute for each item.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        /// <remarks>
        /// Resets the current and minimum values to 0 before starting.
        /// The loop stops early when the user presses the cancel button.
        /// </remarks>
        public RevitProgressBar Run<T>(IEnumerable<T> collection, Action<T> action)
        {
            progressBarStackPanel.Data.CurrentValue = 0;
            progressBarStackPanel.Data.MinimumValue = 0;
            progressBarStackPanel.Data.MaximumValue = collection.Count();
            foreach (var item in collection)
            {
                var current = progressBarStackPanel.Data.CurrentValue;
                action?.Invoke(item);
                progressBarStackPanel.Data.CurrentValue = current + 1;
                if (RefreshStopwatchBackground().IsCancelling())
                    break;
            }
            return this;
        }

        /// <summary>
        /// Sets the text displayed next to the progress bar.
        /// </summary>
        /// <param name="currentOperation">Name of the current operation.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        public RevitProgressBar SetCurrentOperation(string currentOperation)
        {
            progressBarStackPanel.Data.CurrentOperation = currentOperation;
            return this;
        }

        /// <summary>
        /// Sets the current progress value.
        /// </summary>
        /// <param name="currentValue">Value between the minimum and maximum values.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        /// <remarks>Does not refresh the UI; call <see cref="Increment"/> or one of the <c>Run</c> methods to repaint.</remarks>
        public RevitProgressBar SetCurrentValue(double currentValue)
        {
            progressBarStackPanel.Data.CurrentValue = currentValue;
            return this;
        }

        /// <summary>
        /// Sets the value that corresponds to an empty progress bar (0 %). Default is 0.
        /// </summary>
        /// <param name="minimumValue">Minimum value.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        public RevitProgressBar SetMinimumValue(double minimumValue)
        {
            progressBarStackPanel.Data.MinimumValue = minimumValue;
            return this;
        }

        /// <summary>
        /// Sets the value that corresponds to a full progress bar (100 %). Default is 100.
        /// </summary>
        /// <param name="maximumValue">Maximum value.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        public RevitProgressBar SetMaximumValue(double maximumValue)
        {
            progressBarStackPanel.Data.MaximumValue = maximumValue;
            return this;
        }

        /// <summary>
        /// Switches the progress bar to indeterminate mode (continuous animation without a percentage),
        /// useful when the total amount of work is unknown.
        /// </summary>
        /// <param name="isIndeterminate"><see langword="true"/> to enable indeterminate mode.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        public RevitProgressBar SetIsIndeterminate(bool isIndeterminate)
        {
            progressBarStackPanel.Data.IsIndeterminate = isIndeterminate;
            return this;
        }

        /// <summary>
        /// Shows or hides the cancel button.
        /// </summary>
        /// <param name="hasCancelButton"><see langword="true"/> to show the cancel button.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        public RevitProgressBar SetHasCancelButton(bool hasCancelButton)
        {
            progressBarStackPanel.Data.HasCancelButton = hasCancelButton;
            return this;
        }

        /// <summary>
        /// Increases the current value and refreshes the UI if <see cref="RefreshMilliseconds"/> have elapsed
        /// since the last refresh. Use it for manual loops instead of the <c>Run</c> methods.
        /// </summary>
        /// <param name="incrementCurrentValue">Amount added to the current value. Default is 1.</param>
        /// <returns>The same <see cref="RevitProgressBar"/> instance, for method chaining.</returns>
        /// <example>
        /// <code>
        /// using (var progressBar = new RevitProgressBar())
        /// {
        ///     progressBar.SetCurrentOperation("Processing").SetMaximumValue(elements.Count);
        ///     foreach (var element in elements)
        ///     {
        ///         progressBar.Increment();
        ///         if (progressBar.IsCancelling()) break;
        ///     }
        /// }
        /// </code>
        /// </example>
        public RevitProgressBar Increment(int incrementCurrentValue = 1)
        {
            progressBarStackPanel.Data.CurrentValue += incrementCurrentValue;
            RefreshStopwatchBackground();
            return this;
        }

        private bool cancelPressed { get; set; } = false;

        /// <summary>
        /// Returns whether cancellation was requested, either by the cancel button or by <see cref="Cancel"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the operation should stop.</returns>
        /// <remarks>
        /// The first call also binds the cancel button to <see cref="Cancel"/>, so the button
        /// only works after this method has been called at least once
        /// (the <c>Run</c> methods call it automatically).
        /// </remarks>
        public bool IsCancelling()
        {
            if (progressBarStackPanel.Data.CommandCancel is null)
            {
                progressBarStackPanel.Data.CommandCancel = new RelayCommand(Cancel);
            }
            return cancelPressed;
        }

        /// <summary>
        /// Requests cancellation. The <c>Run</c> methods stop after the current item;
        /// <see cref="IsCancelling"/> returns <see langword="true"/> from now on.
        /// </summary>
        public void Cancel()
        {
            cancelPressed = true;
        }

        private bool ForceToRefresh;

        /// <summary>
        /// Hides the progress bar, restores the native Revit status bar,
        /// re-enables the ribbon and resets the cursor.
        /// </summary>
        /// <remarks>
        /// The ribbon and cursor are restored only if the native status bar was visible
        /// when this instance was created.
        /// </remarks>
        public void Dispose()
        {
            StatusBarController.Hide();
            stopwatch.Stop();

            RefreshBackground(ForceToRefresh);
        }

        /// <summary>
        /// Minimum interval, in milliseconds, between UI refreshes. Lower values give smoother
        /// animation at the cost of performance. Default is 50.
        /// </summary>
        public int RefreshMilliseconds { get; set; } = 50;

        /// <summary>
        /// Delay, in milliseconds, before the progress bar first appears. Operations that finish
        /// faster than this are never shown. Default is 250; set to 0 to show it immediately.
        /// </summary>
        public int InitializeMilliseconds { get; set; } = 250;

        private RevitProgressBar RefreshStopwatchBackground()
        {
            if (InitializeMilliseconds > 0 && stopwatch.ElapsedMilliseconds < InitializeMilliseconds)
            {
                return this;
            }
            InitializeMilliseconds = 0;
            if (stopwatch.ElapsedMilliseconds > RefreshMilliseconds)
            {
                StatusBarController.Show(progressBarStackPanel);
                RefreshBackground();
                stopwatch.Restart();
            }
            return this;
        }

        private void RefreshBackground(bool disable = false)
        {
            //if (!disable)
            //    RevitRibbonController.Disable();

            ApplicationUtils.DoEvents();
            //ApplicationUtils.SetCursorWait();
            //progressBarStackPanel.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render);
            if (disable)
            {
                ApplicationUtils.SetCursorDefault();
                RevitRibbonController.Enable();
            }
            else
            {
                ApplicationUtils.SetCursorWait();
                RevitRibbonController.Disable();
            }
        }
    }
}
