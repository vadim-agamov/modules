using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.DiContainer;
using Modules.Events;
using Modules.Initializator;
using Modules.PlatformService;
using UnityEngine;

namespace Modules.LocalizationService
{
    public class LocalizationChangedEvent
    {
    }

    public class LocalizationService: ILocalizationService
    {
        private HashSet<LocalizationProviderConfig> _providers = new ();
        private Dictionary<string, string> _keys = new ();
        private Language _language;

        public bool IsInitialized { get; private set; }

        private IPlatformService PlatformService { get; set; }
        
        [Inject]
        private void Inject(IPlatformService platformService)
        {
            PlatformService = platformService;
        }
        
        UniTask IInitializable.Initialize(CancellationToken cancellationToken)
        {
            _language = PlatformService.GetLocale();
            Debug.Log($"[{nameof(LocalizationService)}] Current language: {_language}");
            IsInitialized = true;
            return UniTask.CompletedTask;
        }
        
        void ILocalizationService.SetLanguage(Language language)
        {
            _language = language;
            _keys.Clear();
            foreach (var provider in _providers)
            {
                foreach (var key in provider.GetKeys(_language))
                {
                    _keys.Add(key.Key, key.Value);
                }
            }
            
            Event<LocalizationChangedEvent>.Publish();
        }

        void ILocalizationService.Register(LocalizationProviderConfig providerConfig)
        {
            _providers.Add(providerConfig);
            foreach (var key in providerConfig.GetKeys(_language))
            {
                _keys.Add(key.Key, key.Value);
            }
        }

        void ILocalizationService.Unregister(LocalizationProviderConfig providerConfig)
        {
            _providers.Remove(providerConfig);
            foreach (var key in providerConfig.GetKeys(_language))
            {
                _keys.Remove(key.Key);
            }
        }

        string ILocalizationService.Localize(string key, params object[] args)
        {
            if (_keys.TryGetValue(key, out var localization))
            {
                return args.Length > 0 ? string.Format(localization, args) : localization;
            }

            return key;
        }

        Language ILocalizationService.CurrentLanguage => _language;
    }
}