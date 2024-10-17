using Cysharp.Threading.Tasks;

namespace Modules.Actions
{
    public interface IAction
    {
        UniTask<bool> Do();
    }
}
