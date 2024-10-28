using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.ServiceLocator.Initializator
{
    public class Initializator
    {
        private readonly DependencyGraph _dependencyGraph = new ();
        
        public Initializator(IReadOnlyList<IInitializableService> services)
        {
            foreach (var service in services)
            {
                Register(service);
            }
        }
  
        public UniTask Do(CancellationToken token, IProgress<float> progress = null) => 
            Initialize(token, progress);

        private void Register(IInitializableService service)
        {
            var dependencies = service.GetDependencies()
                .Select(ServiceLocator.Get)
                .OfType<IInitializableService>()
                .ToArray();
            _dependencyGraph.Add(service, dependencies);
            
            Debug.Log($"[{nameof(Initializator)}] Register: {service.GetType().Name}, dependencies: {string.Join(", ", dependencies.Select(s => s.GetType().Name))}");
        }

        private async UniTask Initialize(CancellationToken cancellationToken, IProgress<float> progress = null)
        {
            var allDependencies = _dependencyGraph.GetAllDependencies().ToArray();
            var dependenciesDump = string.Join(", ", allDependencies.Select(d => d.Initializable.GetType().Name));
            Debug.Log($"[{nameof(Initializator)}] Initializing: {dependenciesDump}");

            progress?.Report(0);
            var completed = 0;
            foreach (var node in allDependencies)
            {
                if(node.Initializable.IsInitialized)
                    continue;
                
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