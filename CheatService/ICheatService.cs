using Modules.ServiceLocator;
using Modules.ServiceLocator.Initializator;

namespace Modules.CheatService
{
    public interface ICheatService : IInitializableService
    {
        void Show();
        void Hide();
        void RegisterCheatProvider(ICheatsProvider cheatsProvider);
        void UnRegisterCheatProvider(ICheatsProvider cheatsProvider);
    }
}