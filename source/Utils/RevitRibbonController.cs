using UIFramework;

namespace BIMPlugins.Bars.Utils
{
    /// <summary>
    /// Enables or disables the Revit ribbon, for example to block user input while a long operation runs.
    /// </summary>
    public class RevitRibbonController
    {
        /// <summary>
        /// The Revit ribbon control (<c>UIFramework.RevitRibbonControl.RibbonControl</c>).
        /// </summary>
        public static RevitRibbonControl RibbonControl => RevitRibbonControl.RibbonControl;

        /// <summary>
        /// Enables the ribbon so the user can interact with it again.
        /// </summary>
        public static void Enable() => RibbonControl.IsEnabled = true;

        /// <summary>
        /// Disables the ribbon; its buttons stop responding to the user.
        /// </summary>
        public static void Disable() => RibbonControl.IsEnabled = false;
    }
}
