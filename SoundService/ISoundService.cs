using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.ServiceLocator.Initializator;

namespace Modules.SoundService
{
    public interface ISoundService: IInitializableService
    {
        void PlayLoop(string soundId);
        UniTask Play(string soundId);
        UniTask Stop(string soundId);
        void Mute();
        void UnMute();
        bool IsMuted { get;}
    }
}