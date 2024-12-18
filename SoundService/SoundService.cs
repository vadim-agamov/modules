using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Modules.Events;
using Modules.PlatformService;
using Modules.PlayerDataService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Modules.Initializator;


namespace Modules.SoundService
{
    public class SoundService: MonoBehaviour, ISoundService
    {
        private SoundsConfig _config;
        private ObjectPool<AudioSource> _objectPool;
        private List<AudioSource> _activeSources;

        private bool IsMuted { get; set; }

        private readonly CancellationTokenSource _cancellationToken = new ();

        public bool IsInitialized { get; private set; }

        private void Silence(bool silence)
        {
            AudioListener.pause = silence;
            AudioListener.volume = silence ? 0 : 1;
        }
        
        async UniTask IInitializable.Initialize(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = $"[{nameof(SoundService)}]";
            
            _config = await Addressables.LoadAssetAsync<SoundsConfig>("SoundsConfig").ToUniTask(cancellationToken: cancellationToken);

            var clips = _config.GetAllAudioClips().ToList();
            
            clips.ForEach(clip =>
            {
                clip.LoadAudioData();
            });
            
            await UniTask.WaitUntil(() => clips.All(clip =>
            {
                Debug.Log($"[{nameof(SoundService)}] {clip.name}: {clip.loadState}");
                return clip.loadState == AudioDataLoadState.Loaded;
            }), cancellationToken: cancellationToken);
            
            gameObject.name = $"[{nameof(SoundService)}]";
            gameObject.AddComponent<AudioListener>();
            _objectPool = new ObjectPool<AudioSource>(OnCreateAudioSource, OnGetAudioSource, OnReleaseAudioSource, OnDestroyAudioSource);
            _activeSources = new List<AudioSource>();

            Event<AppFocusState>.Subscribe(OnAppFocusStateChanged);
            
            IsInitialized = true;
        }
        
        private void OnAppFocusStateChanged(AppFocusState evt)
        {
            Silence(!evt.IsFocus);
        }

        public void Dispose()
        {
            Event<AppFocusState>.Unsubscribe(OnAppFocusStateChanged);
            
            Addressables.Release(_config);
            _cancellationToken.Cancel();
            _objectPool.Dispose();
            Destroy(this);
        }

        void ISoundService.PlayLoop(string soundId)
        {
            Debug.Log($"[{nameof(SoundService)}] PlayLoop {soundId}");
            var source = _objectPool.Get();
            if (source.clip == null || source.clip.name != soundId)
            {
                source.clip = _config.GetSound(soundId);
                source.clip.name = soundId;
            }

            source.loop = true;
            source.mute = IsMuted;
            source.Play();
        }

        async UniTask ISoundService.Play(string soundId)
        {
            // Debug.Log($"[{nameof(SoundService)}] Play {soundId}");
            var source = _objectPool.Get();
            if (source.clip == null || source.clip.name != soundId)
            {
                source.clip = _config.GetSound(soundId);
                source.clip.name = soundId;
            }

            source.loop = false;
            source.mute = IsMuted;
            source.Play();

            await UniTask
                .Delay(TimeSpan.FromSeconds(source.clip.length), cancellationToken: _cancellationToken.Token)
                .SuppressCancellationThrow();
            _objectPool.Release(source);
        }

        async UniTask ISoundService.Stop(string soundId)
        {
            foreach (var audioSource in _activeSources)
            {
                if (audioSource.clip.name == soundId)
                {
                    await audioSource.DOFade(0, 0.2f)
                        .ToUniTask(cancellationToken: _cancellationToken.Token)
                        .SuppressCancellationThrow();
                    audioSource.Stop();
                    _objectPool.Release(audioSource);
                    break;
                }
            }
        }

        void ISoundService.Mute()
        {
            IsMuted = true;
            foreach (var activeSource in _activeSources)
            {
                activeSource.mute = true;
            }
        }

        void ISoundService.UnMute()
        {
            IsMuted = false;
            foreach (var activeSource in _activeSources)
            {
                activeSource.mute = false;
            }
        }
        bool ISoundService.IsMuted => IsMuted;

        #region Pool

        private AudioSource OnCreateAudioSource()
        {
            var go = new GameObject("AudioSource");
            go.transform.SetParent(gameObject.transform);
            return go.AddComponent<AudioSource>();
        }

        private void OnDestroyAudioSource(AudioSource item)
        {
            item.Stop();
            Destroy(item.gameObject);
        }

        private void OnReleaseAudioSource(AudioSource item)
        {
            _activeSources.Remove(item);
            item.volume = 1f;
        }

        private void OnGetAudioSource(AudioSource item)
        {
            _activeSources.Add(item);
        }

        #endregion
    }
}