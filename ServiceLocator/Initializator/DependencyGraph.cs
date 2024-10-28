using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.ServiceLocator.Initializator
{
    internal class Node
    {
        public IInitializableService Initializable;
        public List<Node> Dependencies;
        
        public void Deconstruct(out IInitializableService initializable, out List<Node> dependencies)
        {
            initializable = Initializable;
            dependencies = Dependencies;
        }
    }
    
    internal class DependencyGraph
    {
        private readonly Dictionary<IInitializableService, Node> _nodes = new ();
        private readonly HashSet<Node> _visited = new();
        
        public void Add(IInitializableService initializable, params IInitializableService[] dependencies)
        {
            var node = GetOrCreateNode(initializable);
            node.Dependencies = dependencies.Select(GetOrCreateNode).ToList();
        }

        public IEnumerable<Node> GetAllDependencies()
        {
            var rootNode = new Node { Dependencies = _nodes.Values.ToList() };
            CheckCircularDependencies(rootNode, new Stack<Node>());
            
            _visited.Clear();
            var allDependencies = new LinkedList<Node>();
            GetDependenciesRecursive(rootNode, allDependencies);
            allDependencies.RemoveLast(); // remove root node
            return allDependencies;
        }

        private Node GetOrCreateNode(IInitializableService initializable)
        {
            if (!_nodes.ContainsKey(initializable))
            {
                _nodes.Add(initializable, new Node
                {
                    Initializable = initializable,
                    Dependencies = new List<Node>()
                });
            }

            return _nodes[initializable];
        }
        
        private void GetDependenciesRecursive(Node node, LinkedList<Node> allDependencies)
        {
            _visited.Add(node);
            
            foreach (var dependencyNode in node.Dependencies)
            {
                if (_visited.Contains(dependencyNode))
                {
                    continue;
                }
                
                GetDependenciesRecursive(dependencyNode, allDependencies);
            }
            
            allDependencies.AddLast(node);
        }
        
        private void CheckCircularDependencies(Node node, Stack<Node> allDependencies)
        {
            if(allDependencies.Any(d => d == node))
            {
                allDependencies.Push(node);
                var dependencies = allDependencies.Reverse().Skip(1);
                var dump = string.Join("->", dependencies.Select(d => d.Initializable.GetType().Name));
                throw new Exception($"Circular dependency detected {dump}");
            }
            
            allDependencies.Push(node);
            foreach (var dependencyNode in node.Dependencies)
            {
                CheckCircularDependencies(dependencyNode, allDependencies);
            }
            allDependencies.Pop();
        }
    }
}