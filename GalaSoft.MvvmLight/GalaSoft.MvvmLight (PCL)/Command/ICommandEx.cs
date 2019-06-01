using System.Windows.Input;

#if PLATFORMNET45
namespace GalaSoft.MvvmLight.CommandWpf
#else
namespace GalaSoft.MvvmLight.Command
#endif
{
    /// <summary>
    /// The ICommand interface provides ability to raise CanExecuteChanged event on request.
    /// </summary>
    public interface ICommandEx : ICommand
    {
        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged" /> event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
