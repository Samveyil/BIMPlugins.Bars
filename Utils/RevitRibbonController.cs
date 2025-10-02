using UIFramework;

namespace BIMPlugins.ProgressBar.Utils
{
    /// <summary>
    /// RevitRibbonController
    /// </summary>
    public class RevitRibbonController
    {
        /// <summary>
        /// RibbonControl
        /// </summary>
        public static RevitRibbonControl RibbonControl => RevitRibbonControl.RibbonControl;

        /// <summary>
        /// Enable
        /// </summary>
        public static void Enable() => RibbonControl.IsEnabled = true;
        /// <summary>
        /// Disable
        /// </summary>
        public static void Disable() => RibbonControl.IsEnabled = false;
    }
}