using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.Initializator
{
    public interface IInitializable
    {
        UniTask Initialize(CancellationToken cancellationToken);
        bool IsInitialized { get; }
    }
}