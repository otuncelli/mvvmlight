using System.Threading.Tasks;

#if PLATFORMNET45
namespace GalaSoft.MvvmLight.CommandWpf
#else
namespace GalaSoft.MvvmLight.Command
#endif
{
    /// <summary>Defines a asynchronous command.</summary>
    public interface IAsyncCommand<in T> : ICommandEx
    {
        /// <summary>
        /// Defines the method to be called asynchronously when the command is invoked. 
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data 
        /// to be passed, this object can be set to a null reference</param>
        Task ExecuteAsync(object parameter);
    }
}
