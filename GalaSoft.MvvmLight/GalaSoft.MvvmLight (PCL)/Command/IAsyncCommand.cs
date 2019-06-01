using System.Threading.Tasks;

#if PLATFORMNET45
namespace GalaSoft.MvvmLight.CommandWpf
#else
namespace GalaSoft.MvvmLight.Command
#endif
{
    /// <summary>Defines a asynchronous command.</summary>
    public interface IAsyncCommand : ICommandEx
    {
        /// <summary>
        /// Defines the method to be called when the command is invoked. 
        /// </summary>
        Task ExecuteAsync();

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        bool CanExecute();
    }
}
