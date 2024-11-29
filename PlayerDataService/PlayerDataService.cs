using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.DiContainer;
using Modules.Initializator;
using Modules.PlatformService;
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
        private const int AutoSaveInterval = 5;
        private float _timeSinceLastSave;
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
                finally
                {
                    Data ??= new TData();
                }
            }
            _isInitialized = true;
        }

        public void Dispose() => _cancellationTokenSource.Cancel();

        protected void SetDirty() => _needSave = true;

        public void ResetData()
        {
            Data = new TData();
            SetDirty();
        }

        public void Commit() => SetDirty();

        public void ForceCommit()
        {
            SetDirty();
            _timeSinceLastSave = AutoSaveInterval;
        }

        private void Update()
        {
            _timeSinceLastSave += Time.deltaTime;
            
            if (_needSave && !_savingIsInProgress && _timeSinceLastSave >= AutoSaveInterval)
            {
                CommitAsync().Forget();
            }

            async UniTask CommitAsync()
            {
                try
                {
                    Debug.Log($"[{nameof(PlayerDataService)}] saving...");
                    _savingIsInProgress = true;
                    var data = JsonConvert.SerializeObject(Data, Formatting.Indented);
                    await PlatformService.SavePlayerProgress(data, _cancellationTokenSource.Token);
                    Debug.Log($"[{nameof(PlayerDataService)}] saved");
                }
                catch(Exception exception)
                {
                    Debug.LogError($"[{nameof(PlayerDataService)}] error while saving: {exception}");
                }
                finally
                {
                    _needSave = false;
                    _savingIsInProgress = false;
                    _timeSinceLastSave = 0;
                }
            }
        }
    }
}