namespace BIMPlugins.Bars
{
    /// <summary>
    /// Thin wrappers around Windows Forms application and cursor APIs used to keep the Revit UI
    /// responsive during long operations on the UI thread.
    /// </summary>
    public static class ApplicationUtils
    {
        /// <summary>
        /// Processes all pending Windows messages so the UI can repaint
        /// (<see cref="System.Windows.Forms.Application.DoEvents"/>).
        /// </summary>
        /// <remarks>
        /// Pumping messages allows re-entrancy: user input may be processed while your code runs.
        /// <see cref="RevitProgressBar"/> disables the ribbon to limit this.
        /// </remarks>
        public static void DoEvents()
        {
            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        /// Sets the current cursor to the wait (hourglass) cursor.
        /// </summary>
        public static void SetCursorWait()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        /// <summary>
        /// Restores the default arrow cursor.
        /// </summary>
        public static void SetCursorDefault()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }
    }
}
