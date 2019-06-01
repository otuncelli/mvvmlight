using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;

#if !NET_FXCORE
using GalaSoft.MvvmLight.Helpers;
#endif

#if PLATFORMNET45
namespace GalaSoft.MvvmLight.CommandWpf
#else
namespace GalaSoft.MvvmLight.Command
#endif
{
    /// <summary>
    /// A command whose sole purpose is to asynchronously relay its functionality to other
    /// objects by invoking delegates. The default return value for the CanExecute
    /// method is 'true'.  This class does not allow you to accept command parameters in the
    /// Execute and CanExecute callback methods.
    /// </summary>
    /// <remarks>If you are using this class in WPF4.5 or above, you need to use the 
    /// GalaSoft.MvvmLight.CommandWpf namespace (instead of GalaSoft.MvvmLight.Command).
    /// This will enable (or restore) the CommandManager class which handles
    /// automatic enabling/disabling of controls based on the CanExecute delegate.</remarks>
    public class AsyncCommand : IAsyncCommand
    {
        private readonly WeakFunc<Task> _execute;

        private readonly WeakFunc<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the AsyncCommand class that 
        /// can always execute.
        /// </summary>
        /// <param name="execute">The asynchronous execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closure. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public AsyncCommand(Func<Task> execute, bool keepTargetAlive = false)
            : this(execute, null, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AsyncCommand class.
        /// </summary>
        /// <param name="execute">The asynchronous execution logic. IMPORTANT: If the action causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="canExecute">The execution status logic.  IMPORTANT: If the func causes a closure,
        /// you must set keepTargetAlive to true to avoid side effects. </param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is causing a closures. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
        public AsyncCommand(Func<Task> execute, Func<bool> canExecute, bool keepTargetAlive = false)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            _execute = new WeakFunc<Task>(execute, keepTargetAlive);

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#elif NETFX_CORE
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#elif XAMARIN
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#else
        private EventHandler _requerySuggestedLocal;
        
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    // add event handler to local handler backing field in a thread safe manner
                    EventHandler handler2;
                    EventHandler canExecuteChanged = _requerySuggestedLocal;

                    do
                    {
                        handler2 = canExecuteChanged;
                        EventHandler handler3 = (EventHandler)Delegate.Combine(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
                            ref _requerySuggestedLocal, 
                            handler3, 
                            handler2);
                    } 
                    while (canExecuteChanged != handler2); 
                    
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (_canExecute != null)
                {
                    // removes an event handler from local backing field in a thread safe manner
                    EventHandler handler2;
                    EventHandler canExecuteChanged = this._requerySuggestedLocal;

                    do
                    {
                        handler2 = canExecuteChanged;
                        EventHandler handler3 = (EventHandler)Delegate.Remove(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
                            ref this._requerySuggestedLocal, 
                            handler3, 
                            handler2);
                    } 
                    while (canExecuteChanged != handler2); 
                    
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
#endif

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "The this keyword is used in the Silverlight version")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public void RaiseCanExecuteChanged()
        {
#if SILVERLIGHT
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#elif NETFX_CORE
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#elif XAMARIN
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#else
            CommandManager.InvalidateRequerySuggested();
#endif
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// Defines the method to be called asynchronously when the command is invoked. 
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync();
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        public bool CanExecute()
        {
            return _canExecute == null
                   || (_canExecute.IsStatic || _canExecute.IsAlive)
                   && _canExecute.Execute();
        }

        /// <summary>
        /// Defines the method to be called asynchronously when the command is invoked. 
        /// </summary>
        public virtual Task ExecuteAsync()
        {
            if (CanExecute()
                && _execute != null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                return _execute.Execute();
            }

            return Task.FromResult(0);
        }
    }
}
