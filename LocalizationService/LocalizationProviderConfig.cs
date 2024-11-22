using System.Collections.Generic;
using System.Linq;
using Modules.PlatformService;
using UnityEngine;

namespace Modules.LocalizationService
{
    [CreateAssetMenu(menuName = "Create LocalizationProviderConfig", fileName = "LocalizationProviderConfig", order = 0)]
    public class LocalizationProviderConfig : ScriptableObject
    {
        [SerializeField] 
        private List<LocalizationConfig> _configs = new List<LocalizationConfig>();
        
        public IReadOnlyList<LocalizationKey> GetKeys(Language language)
        {
            var config = _configs.First(config => config.Language == language);
            return config.Keys;
        }
        
#if UNITY_EDITOR
        public void Add(LocalizationConfig config)
        {
            _configs.Add(config);
        }
#endif
    }
}