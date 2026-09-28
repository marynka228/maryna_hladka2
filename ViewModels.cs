using System.Windows.Input;

namespace maryna_hladka2.ViewModels
{
    // Власна реалізація ICommand. Якщо хочеш простіше - можна замінити на
    // Microsoft.Maui.Controls.Command / Command<T>, вони роблять те саме.
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public event EventHandler? CanExecuteChanged;

        // Викликати після зміни даних, від яких залежить CanExecute (наприклад, FullName)
        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
