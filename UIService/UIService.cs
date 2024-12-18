using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.Initializator;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Modules.UIService
{
    public class UIService: IUIService
    {
        private Canvas _canvas;
        private Vector2 ReferenceResolution { get; }
        Canvas IUIService.Canvas => _canvas;
        
        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;

        public UIService(Vector2 referenceResolution)
        {
            ReferenceResolution = referenceResolution;
        }
        
        UniTask IInitializable.Initialize(CancellationToken cancellationToken)
        {
            SetupCanvas();
            _isInitialized = true;
            return UniTask.CompletedTask;
        }

        private void SetupCanvas()
        {
            var rootGameObject = new GameObject(
                "[RootCanvas]",
                typeof(Canvas),
                typeof(CustomCanvasScreenSizeScaler),
                typeof(GraphicRaycaster));

            var canvasScaler = rootGameObject.GetComponent<CustomCanvasScreenSizeScaler>();
            canvasScaler.referenceResolution = ReferenceResolution;
            canvasScaler.matchWidthOrHeight = 0;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            // var camera = new GameObject("[UICamera]").AddComponent<Camera>();
            // camera.transform.position = new Vector3(100, 0, 0);
            // camera.orthographic = true;
            // camera.clearFlags = CameraClearFlags.Nothing;
            // camera.orthographicSize = 10;

            _canvas = rootGameObject.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            // _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            // _canvas.worldCamera = camera;

            Object.DontDestroyOnLoad(rootGameObject);
            // Object.DontDestroyOnLoad(camera.gameObject);
        }

        async UniTask IUIService.Open<TModel>(TModel model, string key, CancellationToken cancellationToken)
        {
            var op = Addressables.InstantiateAsync(key, _canvas.transform);
            await op.ToUniTask(cancellationToken: cancellationToken);
            if (op.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Unable to instantiate asset from location {key}:\r\n{op.OperationException}");
                return;
            }

            var viewGameObject = op.Result;
            viewGameObject.name = key;
            viewGameObject.SetActive(false);
            var view = viewGameObject.GetComponent<UIView>();
            view.SetModel(model);
            model.ViewControl = view;
        }

        void IUIService.Close<TModel>(TModel model)
        {
            Addressables.ReleaseInstance(model.ViewControl.GameObject);
            model.ViewControl = null;
        }
    }
}