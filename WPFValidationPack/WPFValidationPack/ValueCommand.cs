namespace WPFValidationPack
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    public class ValueCommand<T> : ICommand
    {
        readonly Action<T> execute;
        readonly Predicate<object> canExecute;
        readonly T value;

        public ValueCommand(Action<T> execute, T value)
            : this(execute, value, null)
        {
        }

        public ValueCommand(Action<T> execute, T value, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
            this.value = value;
        }


        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public T Value
        {
            get { return value; }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute(value);
        }
    }
}
