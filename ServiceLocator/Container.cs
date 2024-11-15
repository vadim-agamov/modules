using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

namespace Modules.ServiceLocator
{
    public static class Container
    {
        private static readonly HashSet<Object> _services = new();

        public static TService Resolve<TService>() where TService : class
        {
            foreach (var service in _services)
            {
                if (service is TService serviceImplementation)
                {
                    return serviceImplementation;
                }
            }

            throw new InvalidOperationException($"[{nameof(Container)}] Get: Service of type {typeof(TService).Name} is not registered.");
        }

        public static Object Resolve(Type service)
        {
            foreach (var s in _services)
            {
                if (service.IsInstanceOfType(s))
                {
                    return s;
                }
            }
        
            throw new InvalidOperationException($"[{nameof(Container)}] Get: Service of type {service.Name} is not registered.");
        }

        public static T[] AllServices<T>() => _services.OfType<T>().ToArray();

        public static T Bind<T,TImpl>(TImpl service) 
            where T : class
            where TImpl : T
        {
            if (_services.Any(s => s.GetType() == typeof(T)))
            {
                throw new InvalidOperationException($"[{nameof(Container)}] Bind: Service of type {typeof(T)} already registered.");
            }
            
            Debug.Log($"[{nameof(Container)}] Bind {typeof(T).Name} to {typeof(TImpl).Name}");
            _services.Add(service);
            return service;
        }
        
        public static T Bind<T>(T service) where T : class
        {
            if (_services.Any(s => s.GetType() == typeof(T)))
            {
                throw new InvalidOperationException($"[{nameof(Container)}] Bind: Service of type {typeof(T)} already registered.");
            }
            
            Debug.Log($"[{nameof(Container)}] Bind {typeof(T).Name}");
            _services.Add(service);
            return service;
        }
        
        public static T Inject<T>(T service) where T : class
        {
            DependencyUtils.InjectDependencies(service);
            return service;
        }

        public static TImpl BindAndInject<T,TImpl>(TImpl service) 
            where T : class
            where TImpl : class, T, new()
        {
            Bind<T,TImpl>(service);
            Inject(service);
            return service;
        }
        
        public static T BindAndInject<T>(T service) where T : class
        {
            Bind(service);
            Inject(service);
            return service;
        }

        public static void UnBind<TService>() where TService : class
        {
            foreach (var service in _services)
            {
                if (service is TService _)
                {
                    Debug.Log($"[{nameof(Container)}] UnRegister {typeof(TService).Name}, {service}");
                    
                    if(service is IDisposable disposable)
                        disposable.Dispose();
                    
                    _services.Remove(service);
                    return;
                }
            }

            throw new InvalidOperationException($"[{nameof(Container)}] UnRegister: No service of type {typeof(TService).Name}");
        }
    }
}