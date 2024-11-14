using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Modules.Initializator;
using Modules.ServiceLocator;
using Modules.UIService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Modules.FlyItemsService
{
    public interface IFlyItemsService : IInitializable
    {
        UniTask Fly(string name, string from, string to, int count);
        UniTask Fly(string name, Vector3 from, string to, int count);
        void RegisterAnchor(FlyItemAnchor anchor);
        void UnregisterAnchor(FlyItemAnchor anchor);
    }
    
    public class FlyItemsService: IFlyItemsService
    {
        private ObjectPool<Image> _pool;
        private Canvas _canvas;
        private FlyItemsConfig _config;
        private readonly List<FlyItemAnchor> _anchors = new();
        public bool IsInitialized { get; private set; }

        private IUIService UiService { get; set; }
        
        
        [Inject]
        private void Inject(IUIService uiService)
        {
            UiService = uiService;
        }

        async UniTask IInitializable.Initialize(CancellationToken cancellationToken)
        {
            _canvas = UiService.Canvas;
            _pool = new ObjectPool<Image>(OnCreateItem, OnGetItem, OnReleaseItem);
            _config = await Addressables.LoadAssetAsync<FlyItemsConfig>("FlyItemsConfig");
            IsInitialized = true;
        }

        private void OnReleaseItem(Image item)
        {
            item.transform.position = Vector3.zero;
            item.gameObject.SetActive(false); 
        }

        private void OnGetItem(Image item)
        {
            item.gameObject.SetActive(true); 
        }

        private Image OnCreateItem()
        {
            var go = new GameObject();
            go.transform.SetParent(_canvas.transform);
            go.transform.localScale = Vector3.one;
            go.transform.transform.position = Vector3.zero;
            go.name = "FlyItem";
            var image = go.AddComponent<Image>();
            image.preserveAspect = true;
            return image;
        }

        public UniTask Fly(string name, Vector3 from, string toId, int count)
        {
            var to = _anchors.First(x => x.Id == toId);
            return Fly(name, count, from, null, to.transform.position, to.Play);
        }

        public UniTask Fly(string name, string fromId, string toId, int count)
        {
            var from = _anchors.First(x => x.Id == fromId);
            var to = _anchors.First(x => x.Id == toId);
            var names = Enumerable.Repeat(name, count).ToList();
            return Fly(name, count,from.transform.position, from.Play, to.transform.position, to.Play);
        }
        
        void IFlyItemsService.RegisterAnchor(FlyItemAnchor anchor) => _anchors.Add(anchor);

        void IFlyItemsService.UnregisterAnchor(FlyItemAnchor anchor) => _anchors.Remove(anchor);

        private static IReadOnlyList<int> DivideIntoNParts(int x, int n)
        {
            if(x < n)
            {
                return Enumerable.Repeat(1, x).ToList();
            }
            
            // Calculate the base value for each part
            var baseValue = x / n;
            // Calculate the remainder to be added to one of the parts
            var remainder = x % n;

            // Create the list with 'baseValue' repeated 'N' times
            var parts = Enumerable.Repeat(baseValue, n).ToList();

            // Add the remainder to the first element (or any one element as needed)
            parts[0] += remainder;
            return parts;
        }
        private async UniTask Fly(string id, int totalAmount, Vector3 from, Action<string,int> fromAction, Vector3 to, Action<string,int> toAction)
        {
            from = new Vector3(from.x, from.y, _canvas.transform.position.z); 

            var distance = Vector3.Distance(from, to);
            
            var midPoint = Vector3.Lerp(from, to, 0.5f);
            var xDelta = (from-to).x * 0.2f;
            var yDelta = distance * 0.2f;
                
            var sequence = DOTween.Sequence();
  
            
            var amounts = DivideIntoNParts(totalAmount, 10);
            for (var i = 0; i < amounts.Count; i++)
            {
                var amount = amounts[i];
                var item = _pool.Get();
                item.sprite = _config.GetIcon(id);
                item.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                item.transform.position = from;

                fromAction?.Invoke(id, -1);

                midPoint = new Vector3(midPoint.x + Random.Range(-xDelta, xDelta), midPoint.y, midPoint.z);
                
                sequence
                    .Insert(i * 0.01f, item.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f))
                    .Insert(i * 0.01f + 0.2f, item.transform.DOScale(Vector3.one, 0.2f))
                    .Insert(i * 0.05f + 0.4f,item.transform.DOPath(new[] {midPoint, to}, distance /1000, PathType.CatmullRom)
                        .SetEase(Ease.InCubic)
                        .OnComplete(() =>
                        {
                            _pool.Release(item);
                            toAction?.Invoke(id, amount);
                        }));
            }

            await sequence.Play();
        }
    }
}