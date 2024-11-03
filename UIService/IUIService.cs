using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.Initializator;
using UnityEngine;

namespace Modules.UIService
{
    public interface IUIService: IInitializable
    {
        Canvas Canvas { get; }
        UniTask Open<TModel>(TModel model, string key, CancellationToken cancellationToken) where TModel : UIModel;
        void Close<TModel>(TModel model) where TModel : UIModel;
    }
}