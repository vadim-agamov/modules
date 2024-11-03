using Modules.Initializator;

namespace Modules.CheatService
{
    public interface ICheatService : IInitializable
    {
        void Show();
        void Hide();
        void RegisterCheatProvider(ICheatsProvider cheatsProvider);
        void UnRegisterCheatProvider(ICheatsProvider cheatsProvider);
    }
}