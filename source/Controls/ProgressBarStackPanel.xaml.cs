using BIMPlugins.Bars.Controls.Themes;
using System.Windows.Input;

namespace BIMPlugins.Bars.Controls
{
    /// <summary>
    /// WPF control that renders the progress bar, the current operation text and an optional cancel button.
    /// It is hosted in the Revit status bar by <see cref="RevitProgressBar"/>.
    /// </summary>
    /// <remarks>
    /// The control binds to <see cref="Data"/>, which is used as its <c>DataContext</c>.
    /// Light/dark theme resources are applied automatically according to the current Revit theme.
    /// Styles are based on
    /// <see href="https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/progressbar-styles-and-templates?view=netframeworkdesktop-4.8">ProgressBar styles and templates</see>.
    /// </remarks>
    public partial class ProgressBarStackPanel
    {
        /// <summary>
        /// View model the control is bound to. Changing its properties updates the UI.
        /// </summary>
        public ProgressBarData Data { get; } = new ProgressBarData();

        /// <summary>
        /// Creates the control without a cancel button and with a 0–100 range.
        /// </summary>
        public ProgressBarStackPanel()
        {
            Initialize();
        }

        /// <summary>
        /// Creates the control with a 0–100 range.
        /// </summary>
        /// <param name="hasCancelButton"><see langword="true"/> to show the cancel button.</param>
        public ProgressBarStackPanel(bool hasCancelButton)
        {
            Data.HasCancelButton = hasCancelButton;
            Initialize();
        }

        /// <summary>
        /// Creates the control with the specified range. The current value starts at <paramref name="minimumValue"/>.
        /// </summary>
        /// <param name="hasCancelButton"><see langword="true"/> to show the cancel button.</param>
        /// <param name="minimumValue">Value that corresponds to an empty bar.</param>
        /// <param name="maximumValue">Value that corresponds to a full bar.</param>
        public ProgressBarStackPanel(bool hasCancelButton, double minimumValue, double maximumValue)
        {
            Data.HasCancelButton = hasCancelButton;
            Data.CurrentValue = minimumValue;
            Data.MinimumValue = minimumValue;
            Data.MaximumValue = maximumValue;
            Initialize();
        }

        /// <summary>
        /// Sets the text displayed next to the progress bar.
        /// </summary>
        /// <param name="currentOperation">Name of the current operation.</param>
        /// <returns>The same control, for method chaining.</returns>
        public ProgressBarStackPanel SetCurrentOperation(string currentOperation)
        {
            Data.CurrentOperation = currentOperation;
            return this;
        }

        private void Initialize()
        {
            DataContext = Data;
            InitializeComponent();
            this.AddResourceThemes();
        }

        /// <summary>
        /// Bindable state of <see cref="ProgressBarStackPanel"/>.
        /// <c>INotifyPropertyChanged</c> is implemented at compile time by PropertyChanged.Fody.
        /// </summary>
        [PropertyChanged.AddINotifyPropertyChangedInterface]
        public class ProgressBarData
        {
            /// <summary>
            /// <see langword="true"/> to show a continuous animation instead of a percentage. Default is <see langword="false"/>.
            /// </summary>
            public bool IsIndeterminate { get; set; } = false;

            /// <summary>
            /// Text displayed next to the progress bar. Default is <c>"Loading"</c>.
            /// </summary>
            public string CurrentOperation { get; set; } = "Loading";

            /// <summary>
            /// Current progress value, between <see cref="MinimumValue"/> and <see cref="MaximumValue"/>. Default is 0.
            /// </summary>
            public double CurrentValue { get; set; } = 0;

            /// <summary>
            /// Value that corresponds to an empty bar. Default is 0.
            /// </summary>
            public double MinimumValue { get; set; } = 0;

            /// <summary>
            /// Value that corresponds to a full bar. Default is 100.
            /// </summary>
            public double MaximumValue { get; set; } = 100;

            /// <summary>
            /// Progress as a percentage (0–100) calculated from <see cref="CurrentValue"/>,
            /// <see cref="MinimumValue"/> and <see cref="MaximumValue"/>. Returns 100 when the minimum and maximum
            /// values are equal (compared as integers).
            /// </summary>
            public double DisplayValue => ((int)MaximumValue == (int)MinimumValue) ? 100 : 100.0 * (CurrentValue - MinimumValue) / (MaximumValue - MinimumValue);

            /// <summary>
            /// Command executed when the cancel button is pressed.
            /// </summary>
            public ICommand CommandCancel { get; set; }

            /// <summary>
            /// <see langword="true"/> to show the cancel button. Default is <see langword="false"/>.
            /// </summary>
            public bool HasCancelButton { get; set; } = false;
        }
    }
}
