using Modules.DiContainer;
using UnityEngine;
using UnityEngine.Events;

namespace Modules.FlyItemsService
{
    public class FlyItemAnchor: MonoBehaviour
    {
        [SerializeField] 
        private string _id;

        [SerializeField] 
        private UnityEvent<string,int> _onEvent;

        public string Id => _id;

        public void Play(string id, int v) => _onEvent?.Invoke(id, v); 
        
        private void OnEnable()
        {
            Container.Resolve<IFlyItemsService>().RegisterAnchor(this);
        }
        
        private void OnDisable()
        {
            Container.Resolve<IFlyItemsService>().UnregisterAnchor(this);
        }
    }
}