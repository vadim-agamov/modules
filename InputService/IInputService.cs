using Modules.Initializator;
using UnityEngine;

namespace Modules.InputService
{
    public interface IInputService: IInitializable
    {
        Vector2 Touch0 { get; }
        Vector2 Touch1 { get; }
        int TouchesCount { get; }
    }
}