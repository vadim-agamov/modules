using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.Fsm
{
    public interface IState
    {
        UniTask Enter(CancellationToken cancellationToken = default);
        UniTask Exit(CancellationToken cancellationToken = default);
    }
}