using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.ServiceLocator.Initializator
{
    public interface IInitializableService : IService
    {
        UniTask Initialize(CancellationToken cancellationToken);
        bool IsInitialized { get; }
    }
}