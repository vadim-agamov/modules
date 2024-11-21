using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

namespace Modules.DiContainer
{
    public static class Container
    {
        private static readonly HashSet<Object> _instances = new();

        public static T Resolve<T>() where T : class
        {
            foreach (var instance in _instances)
            {
                if (instance is T serviceImplementation)
                {
                    return serviceImplementation;
                }
            }

            throw new InvalidOperationException($"[{nameof(Container)}] Get: instance of type {typeof(T).Name} is not registered.");
        }

        public static Object Resolve(Type instance)
        {
            foreach (var s in _instances)
            {
                if (instance.IsInstanceOfType(s))
                {
                    return s;
                }
            }
        
            throw new InvalidOperationException($"[{nameof(Container)}] Get: instance of type {instance.Name} is not registered.");
        }

        public static T[] AllInstances<T>() => _instances.OfType<T>().ToArray();

        public static T Bind<T>(T instance) where T : class
        {
            if (_instances.Any(i => i is T))
            {
                throw new InvalidOperationException($"[{nameof(Container)}] Bind: instance of type {typeof(T)} already registered.");
            }
            
            Debug.Log($"[{nameof(Container)}] Bind {typeof(T).Name}");
            _instances.Add(instance);
            return instance;
        }
        
        public static T Inject<T>(T instance) where T : class
        {
            DependencyUtils.InjectDependencies(instance);
            return instance;
        }
        
        public static T BindAndInject<T>(T instance) where T : class
        {
            Bind(instance);
            Inject(instance);
            return instance;
        }

        public static void UnBind<T>() where T : class
        {
            foreach (var instance in _instances)
            {
                if (instance is T _)
                {
                    Debug.Log($"[{nameof(Container)}] UnRegister {typeof(T).Name}, {instance}");

                    if (instance is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }

                    _instances.Remove(instance);
                    return;
                }
            }

            throw new InvalidOperationException($"[{nameof(Container)}] UnRegister: No instance of type {typeof(T).Name}");
        }
    }
}