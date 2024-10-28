using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Modules.ServiceLocator.Initializator
{
    public static class InitializableExtensions
    { 
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        public static Type[] GetDependencies(this IInitializableService initializable)
        {
            return initializable.GetType()
                    .GetProperties(Flags)
                    .Where(p => p.GetCustomAttribute<InitializationDependencyAttribute>() != null)
                    .Select(p => p.PropertyType)
                    .ToArray();
        }

        public static void InjectDependencies(this IInitializableService injectable, IReadOnlyList<IInitializableService> dependencies)
        {
            var injectableType = injectable.GetType();
            foreach (var dependency in dependencies)
            {
                injectableType
                    .GetProperties(Flags)
                    .Where(p => p.GetCustomAttribute<InitializationDependencyAttribute>() != null)
                    .Single(p => p.PropertyType.IsInstanceOfType(dependency))
                    .SetValue(injectable, dependency);
            }
        }
        
        public static bool HasInitializationDependencyAttribute(this IService service)
        {
            return service.GetType()
                .GetProperties(Flags)
                .Any(p => p.GetCustomAttribute<InitializationDependencyAttribute>() != null);
        }
    }
}