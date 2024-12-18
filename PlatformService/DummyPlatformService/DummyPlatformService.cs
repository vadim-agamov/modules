using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.Initializator;
using UnityEngine;

namespace Modules.PlatformService.DummyPlatformService
{
    public class DummyPlatformService : MonoBehaviour, IPlatformService
    {
        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;
        
        UniTask IInitializable.Initialize(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            _isInitialized = true;
            return UniTask.CompletedTask;
        }
        

        Language IPlatformService.GetLocale() => Language.English;

        string IPlatformService.GetUserId() => SystemInfo.deviceUniqueIdentifier;

        UniTask<string> IPlatformService.LoadPlayerProgress() => UniTask.FromResult(string.Empty);

        UniTask IPlatformService.SavePlayerProgress(string playerProgress, CancellationToken token) => UniTask.CompletedTask;

        void IPlatformService.PreloadRewardedVideo()
        {
        }

        async UniTask<bool> IPlatformService.ShowRewardedVideo(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        void IPlatformService.PreloadInterstitial()
        {
        }

        async UniTask<bool> IPlatformService.ShowInterstitial(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        void IPlatformService.PreloadRewardedInterstitial()
        {
        }

        async UniTask<bool> IPlatformService.ShowRewardedInterstitial(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        void IPlatformService.LogEvent(string eventName, Dictionary<string, object> parameters)
        {
        }

        void IPlatformService.GameReady()
        {
        }
    }
}