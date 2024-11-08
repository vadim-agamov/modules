using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.Initializator;
using Modules.PlatformService;
using Modules.ServiceLocator;
using Newtonsoft.Json;
using UnityEngine;

namespace Modules.PlayerDataService
{
    public abstract class PlayerDataService<TData> : MonoBehaviour, IInitializable, IDisposable
        where TData : new()
    {
        private IPlatformService PlatformService { get; set; }
        protected TData Data;
        private bool _needSave;
        private bool _savingIsInProgress;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;
        
        [Inject]
        // protected since reflection can't see private fields in other assemblies
        protected void Inject(IPlatformService platformService)
        {
            PlatformService = platformService;
        }
        
        async UniTask IInitializable.Initialize(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = $"[{nameof(PlayerDataService)}]";

            var data = await PlatformService.LoadPlayerProgress();
            if (string.IsNullOrEmpty(data))
            {
                Data = new TData();
                Debug.Log($"[{nameof(PlayerDataService)}] Initialized with empty data");
            }
            else
            {
                try
                {
                    Data = JsonConvert.DeserializeObject<TData>(data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[{nameof(PlayerDataService)}] Unable to deserialize player data:\r\n{e}");
                }

                Data ??= new TData();
            }
            _isInitialized = true;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
        
        protected void SetDirty()
        {
            _needSave = true;
        }

        public void ResetData()
        {
            Data = new TData();
            SetDirty();
        }

        public void Commit() => SetDirty();

        private void Update()
        {
            if (_needSave && !_savingIsInProgress)
            {
                CommitAsync().Forget();
            }

            async UniTask CommitAsync()
            {
                try
                {
                    _savingIsInProgress = true;
                    var data = JsonConvert.SerializeObject(Data);
                    await PlatformService.SavePlayerProgress(data, _cancellationTokenSource.Token);
                }
                catch(Exception exception)
                {
                    Debug.LogError($"[{nameof(PlayerDataService)}] error while saving: {exception}");
                }

                _needSave = false;
                _savingIsInProgress = false;
            }
        }
    }
}