using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

namespace Modules.ServiceLocator
{
    public static class ServiceLocator
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

            throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Get: Service of type {typeof(TService).Name} is not registered.");
        }

        public static Object Get(Type service)
        {
            foreach (var s in _services)
            {
                if (service.IsInstanceOfType(s))
                {
                    return s;
                }
            }
        
            throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Get: Service of type {service.Name} is not registered.");
        }

        public static T[] AllServices<T>() => _services.OfType<T>().ToArray();

        public static TService Bind<TService>(TService service) where TService : class
        {
            if (_services.Any(s => s.GetType() == typeof(TService)))
            {
                throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Bind: Service of type {typeof(TService)} already registered.");
            }
            
            Debug.Log($"[{nameof(ServiceLocator)}] Bind {typeof(TService).Name}");
            DependencyUtils.InjectDependencies(service);
            _services.Add(service);
            return service;
        }
        

        public static void UnBind<TService>() where TService : class
        {
            foreach (var service in _services)
            {
                if (service is TService _)
                {
                    Debug.Log($"[{nameof(ServiceLocator)}] UnRegister {typeof(TService).Name}, {service}");
                    
                    if(service is IDisposable disposable)
                        disposable.Dispose();
                    
                    _services.Remove(service);
                    return;
                }
            }

            throw new InvalidOperationException($"[{nameof(ServiceLocator)}] UnRegister: No service of type {typeof(TService).Name}");
        }
    }
}