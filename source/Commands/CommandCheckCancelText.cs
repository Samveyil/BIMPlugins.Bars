using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMPlugins.Bars.Controls;
using System.Globalization;
using System.Threading;

namespace BIMPlugins.Bars.Commands
{
    /// <summary>
    /// Test command for the cancel button localization.
    /// Shows a progress bar with a cancel button for about 5 seconds, then reports the Revit language,
    /// the UI culture and the caption the button used.
    /// </summary>
    /// <remarks>
    /// Expected result: "Отмена" in Russian Revit, "Cancel" in any other language
    /// (for example English Revit started with <c>/language ENU</c> on Russian Windows).
    /// </remarks>
    [Transaction(TransactionMode.Manual)]
    public class CommandCheckCancelText : IExternalCommand
    {
        /// <inheritdoc/>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            var revitLanguage = commandData.Application.Application.Language;
            var uiCulture = CultureInfo.CurrentUICulture;
            var cancelText = ProgressBarStackPanel.ProgressBarData.GetDefaultCancelText();

            bool cancelled;
            using (var revitProgressBar = new RevitProgressBar(hasCancelButton: true))
            {
                revitProgressBar.InitializeMilliseconds = 0;
                revitProgressBar.Run($"Revit: {revitLanguage}, UI culture: {uiCulture.Name}", 100, i =>
                {
                    Thread.Sleep(50);
                });
                cancelled = revitProgressBar.IsCancelling();
            }

            var dialog = new TaskDialog("BIMPlugins.Bars")
            {
                MainInstruction = cancelled ? "Progress bar was cancelled" : "Progress bar finished",
                MainContent =
                    $"Revit language: {revitLanguage}\n" +
                    $"UI culture: {uiCulture.Name} ({uiCulture.DisplayName})\n" +
                    $"Cancel button caption: {cancelText}\n\n" +
                    "Expected: \"Отмена\" for Russian Revit, \"Cancel\" for any other language."
            };
            dialog.Show();

            return Result.Succeeded;
        }
    }
}
