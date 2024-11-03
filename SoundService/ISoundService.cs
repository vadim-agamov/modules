using System;
using Cysharp.Threading.Tasks;
using Modules.Initializator;

namespace Modules.SoundService
{
    public interface ISoundService: IInitializable, IDisposable
    {
        void PlayLoop(string soundId);
        UniTask Play(string soundId);
        UniTask Stop(string soundId);
        void Mute();
        void UnMute();
        bool IsMuted { get;}
    }
}