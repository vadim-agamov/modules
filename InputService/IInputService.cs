using Modules.ServiceLocator;
using Modules.ServiceLocator.Initializator;
using UnityEngine;

namespace Modules.InputService
{
    public interface IInputService: IInitializableService
    {
        Vector2 Touch0 { get; }
        Vector2 Touch1 { get; }
        int TouchesCount { get; }
    }
}