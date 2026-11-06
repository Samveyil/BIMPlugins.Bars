using System;
using System.Windows.Input;

namespace BIMPlugins.Bars.Utils
{
    /// <summary>
    /// Minimal <see cref="ICommand"/> that always can execute and invokes a parameterless action.
    /// Used to bind the cancel button of the progress bar.
    /// </summary>
    internal class RelayCommand : ICommand
    {
        private Action action;
#pragma warning disable CS0067
        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        /// <summary>
        /// Creates a command that invokes <paramref name="action"/>.
        /// </summary>
        /// <param name="action">Action to run when the command executes.</param>
        public RelayCommand(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// Always returns <see langword="true"/>.
        /// </summary>
        /// <param name="parameter">Ignored.</param>
        /// <returns><see langword="true"/>.</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Invokes the action passed to the constructor.
        /// </summary>
        /// <param name="parameter">Ignored.</param>
        public void Execute(object parameter)
        {
            action?.Invoke();
        }
    }
}
