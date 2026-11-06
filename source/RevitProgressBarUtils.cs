using System;
using System.Collections.Generic;

namespace BIMPlugins.Bars
{
    /// <summary>
    /// One-line shortcuts that create a <see cref="RevitProgressBar"/>, run a loop with it and dispose it.
    /// </summary>
    /// <remarks>
    /// Use these methods when you only need to show progress. The progress bar has no cancel button and
    /// uses the default timings; for a cancel button, custom ranges or manual updates, create a
    /// <see cref="RevitProgressBar"/> yourself.
    /// The methods must be called on the Revit UI thread.
    /// </remarks>
    /// <example>
    /// <code>
    /// RevitProgressBarUtils.Run("Revit Elements", elements, element =>
    /// {
    ///     Console.WriteLine(element.Name);
    /// });
    /// </code>
    /// </example>
    public static class RevitProgressBarUtils
    {
        /// <summary>
        /// Shows a progress bar with the operation name and executes <paramref name="action"/>
        /// <paramref name="count"/> times, advancing the bar after each call.
        /// The progress bar is removed when the loop finishes or throws.
        /// </summary>
        /// <param name="currentOperation">Text displayed next to the progress bar.</param>
        /// <param name="count">Number of iterations.</param>
        /// <param name="action">Action to execute; receives the zero-based iteration index.</param>
        /// <example>
        /// <code>
        /// RevitProgressBarUtils.Run("Revit Repeat", 100000, i =>
        /// {
        ///     Console.WriteLine(i);
        /// });
        /// </code>
        /// </example>
        public static void Run(string currentOperation, int count, Action<int> action)
        {
            using (var revitProgressBar = new RevitProgressBar())
            {
                revitProgressBar.Run(currentOperation, count, action);
            }
        }

        /// <summary>
        /// Shows a progress bar with the operation name and executes <paramref name="action"/>
        /// for every item of <paramref name="collection"/>, advancing the bar after each item.
        /// The progress bar is removed when the loop finishes or throws.
        /// </summary>
        /// <typeparam name="T">Type of the collection items.</typeparam>
        /// <param name="currentOperation">Text displayed next to the progress bar.</param>
        /// <param name="collection">
        /// Items to process. It is enumerated twice (to count the items and to process them),
        /// so pass a materialized collection such as a list or an array.
        /// </param>
        /// <param name="action">Action to execute for each item.</param>
        /// <example>
        /// <code>
        /// var walls = new FilteredElementCollector(document)
        ///     .OfClass(typeof(Wall))
        ///     .ToElements();
        ///
        /// RevitProgressBarUtils.Run("Walls", walls, wall =>
        /// {
        ///     Console.WriteLine(wall.Name);
        /// });
        /// </code>
        /// </example>
        public static void Run<T>(string currentOperation, IEnumerable<T> collection, Action<T> action)
        {
            using (var revitProgressBar = new RevitProgressBar())
            {
                revitProgressBar.Run(currentOperation, collection, action);
            }
        }
    }
}
