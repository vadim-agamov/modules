using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.Initializator
{
    public class Initializator
    {
        private readonly DependencyGraph _dependencyGraph = new();

        public Initializator(IReadOnlyList<IInitializable> services)
        {
            foreach (var service in services)
            {
                Register(service);
            }
        }

        public Initializator(IInitializable service)
        {
            Register(service);
        }

        public UniTask Do(CancellationToken token, IProgress<float> progress = null) =>
            Initialize(token, progress);

        private void Register(IInitializable service)
        {
            var dependencies = DependencyUtils.HasDependencies(service)
                ? DependencyUtils.GetDependencies(service)
                    .Select(ServiceLocator.ServiceLocator.Get)
                    .OfType<IInitializable>()
                    .ToArray()
                : Array.Empty<IInitializable>();

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
                if (node.Initializable.IsInitialized)
                    continue;

                Debug.Log($"[{nameof(Initializator)}] Begin: {node.Initializable.GetType().Name}");
                await node.Initializable.Initialize(cancellationToken);
                Debug.Log($"[{nameof(Initializator)}] End: {node.Initializable.GetType().Name}");
                progress?.Report(++completed / (float)allDependencies.Length);
            }
        }
    }
}