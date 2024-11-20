using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Modules.Utils
{
    public interface IPooledGameObject
    {
        string Id { get; }
        GameObject GameObject { get; }
        void Reset();
    }

    public class GameObjectsPool
    {
        private readonly Dictionary<string, IObjectPool<IPooledGameObject>> _pools = new ();
        private readonly Transform _releasedItemsHolder;
        
        public GameObjectsPool(Transform releasedItemsHolder)
        {
            _releasedItemsHolder = releasedItemsHolder;
        }

        public T Get<T>(T prefab) where T : MonoBehaviour, IPooledGameObject
        {
            if (!_pools.TryGetValue(prefab.Id, out var pool))
            {
                pool = new ObjectPool<IPooledGameObject>(
                    createFunc: () => CreateView(prefab),
                    actionOnGet: OnGet,
                    actionOnRelease: OnRelease);
                _pools[prefab.Id] = pool;
            }

            return (T) pool.Get();
        }
        
        public void Release<T>(T view) where T : IPooledGameObject
        {
            if (!_pools.TryGetValue(view.Id, out var pool))
            {
                throw new InvalidOperationException($"Pool for {view.GameObject.name}|{view.Id} not found");
            }
            
            pool.Release(view);
        }

        private static T CreateView<T>(T prefab) where T : MonoBehaviour, IPooledGameObject
        {
            var view = Object.Instantiate(prefab);
            return view;
        }

        private static void OnGet<T>(T view) where T : IPooledGameObject
        {
            view.GameObject.SetActive(true);
        }

        private void OnRelease<T>(T view) where T : IPooledGameObject
        {
            view.Reset();
            view.GameObject.transform.SetParent(_releasedItemsHolder);
            view.GameObject.SetActive(false);
        }
    }
}
