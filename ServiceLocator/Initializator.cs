using System;
using System.Threading;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.ServiceLocator
{
    public interface IInitializableService : IService
    {
        UniTask Initialize(CancellationToken cancellationToken);
    }

    internal class Initializator
    {
        private readonly DependencyGraph _dependencyGraph = new ();
        
        // public Initializator Register<T>() where T : class, IService, IInitializable
        // {
        //     var service = (IInitializable)ServiceLocator.Get<T>();
        //     var dependencies = service.GetDependencies()
        //         .Select(ServiceLocator.Get)
        //         .OfType<IInitializable>()
        //         .ToArray();
        //     _dependencyGraph.Add(service, dependencies);
        //     
        //     Debug.Log($"[{nameof(Initializator)}] Register: {typeof(T).Name}, dependencies: {string.Join(", ", dependencies.Select(s => s.GetType().Name))}");
        //     return this; 
        // }

        public UniTask InitializeAllInitializables(CancellationToken cancellationToken, IProgress<float> progress = null)
        {
            foreach (var initializable in ServiceLocator.GetInitializables())
            {
                Register(initializable);
            }

            return Initialize(cancellationToken, progress);
        }

        private void Register(IInitializableService service)
        {
            var dependencies = service.GetDependencies()
                .Select(ServiceLocator.Get)
                .OfType<IInitializableService>()
                .ToArray();
            _dependencyGraph.Add(service, dependencies);
            
            Debug.Log($"[{nameof(Initializator)}] Register: {service.GetType().Name}, dependencies: {string.Join(", ", dependencies.Select(s => s.GetType().Name))}");
        }

        public async UniTask Initialize(CancellationToken cancellationToken, IProgress<float> progress = null)
        {
            var allDependencies = _dependencyGraph.GetAllDependencies().ToArray();
            var dependenciesDump = string.Join(", ", allDependencies.Select(d => d.Initializable.GetType().Name));
            Debug.Log($"[{nameof(Initializator)}] Initializing: {dependenciesDump}");

            progress?.Report(0);
            var completed = 0;
            foreach (var node in allDependencies)
            {
                var dependencies = node.Dependencies.Select(d => d.Initializable).ToList();
                Debug.Log($"[{nameof(Initializator)}] Begin: {node.Initializable.GetType().Name}");
                node.Initializable.InjectDependencies(dependencies);
                await node.Initializable.Initialize(cancellationToken);
                Debug.Log($"[{nameof(Initializator)}] End: {node.Initializable.GetType().Name}");
                progress?.Report(++completed / (float)allDependencies.Length);
            }
        }
    }
}