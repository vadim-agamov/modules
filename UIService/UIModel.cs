using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.UIService
{
    public abstract class UIModel
    {
        internal IViewControl ViewControl { get; set; }
        public UniTask ShowView(CancellationToken token) => ViewControl.Show(token);
        public UniTask HideView(CancellationToken token) => ViewControl.Hide(token);
        public void UpdateModel() => ViewControl.UpdateModel();
    }
}