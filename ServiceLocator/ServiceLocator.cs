using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator.Initializator;
using UnityEngine;

namespace Modules.ServiceLocator
{
    public static class ServiceLocator
    {
        private class ServiceItem
        {
            public IService Service;
        }

        private static readonly HashSet<ServiceItem> _services = new();

        public static TService Get<TService>() where TService : class, IService
        {
            foreach (var serviceItem in _services)
            {
                if (serviceItem.Service is TService serviceImplementation)
                {
                    return serviceImplementation;
                }
            }

            throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Get: Service of type {typeof(TService).Name} is not registered.");
        }

        public static IService Get(Type service)
        {
            foreach (var serviceItem in _services)
            {
                if (service.IsInstanceOfType(serviceItem.Service))
                {
                    return serviceItem.Service;
                }
            }

            throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Get: Service of type {service.Name} is not registered.");
        }

        public static IInitializableService[] GetInitializables() =>
            _services
                .Select(s => s.Service)
                .OfType<IInitializableService>()
                .ToArray();

        public static TService Register<TService>(TService service) where TService : class, IService
        {
            if (_services.Any(s => s.Service.GetType() == typeof(TService)))
            {
                throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Register: Service of type {typeof(TService)} already registered.");
            }

            if (service is not IInitializableService && service.HasInitializationDependencyAttribute())
            {
                throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Register: Service of type {typeof(TService)} must implement {nameof(IInitializableService)} interface.");
            }

            var serviceItem = new ServiceItem { Service = service };
            _services.Add(serviceItem);
            return service;
        }

        public static void UnRegister<TService>() where TService : class, IService
        {
            foreach (var service in _services)
            {
                if (service.Service is TService _)
                {
                    Debug.Log($"[{nameof(ServiceLocator)}] UnRegister {typeof(TService).Name}, {service}");
                    service.Service.Dispose();
                    _services.Remove(service);
                    return;
                }
            }

            throw new InvalidOperationException($"[{nameof(ServiceLocator)}] UnRegister: No service of type {typeof(TService).Name}");
        }
    }
}