using Cysharp.Threading.Tasks;
using Modules.Initializator;
using UnityEngine;

namespace Modules.FlyItemsService
{
    public interface IFlyItemsService : IInitializable
    {
        UniTask Fly(string name, string from, string to, int count, FlyType type);
        UniTask Fly(string name, Vector3 from, string to, int count, FlyType type);
        void RegisterAnchor(FlyItemAnchor anchor);
        void UnregisterAnchor(FlyItemAnchor anchor);
    }

    public enum FlyType
    {
        FlyToTarget,
        FlyUp
    }
}