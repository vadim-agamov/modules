using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.DiContainer;
using Modules.Initializator;
using Modules.LocalizationService;
using Modules.UIService;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.CheatService
{
    public class CheatService: MonoBehaviour, ICheatService
    {
        private bool _isShown;
        private readonly HashSet<ICheatsProvider> _cheatsProviders = new HashSet<ICheatsProvider>();
        private bool _isReady;

        private ICheatService This => this;
        public bool IsInitialized  { get; private set; }

        [Inject]
        private void Inject(ILocalizationService localizationService)
        {
        }

        UniTask IInitializable.Initialize(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = $"[{nameof(CheatService)}]";
            IsInitialized = true;
            return UniTask.CompletedTask;        
        }

        
        void ICheatService.Show()
        {
            _isShown = true;
            var canvas = Container.Resolve<IUIService>().Canvas;
            canvas.GetComponent<GraphicRaycaster>().enabled = false;
        }

        void ICheatService.Hide()
        {
            _isShown = false;
            var canvas = Container.Resolve<IUIService>().Canvas;
            canvas.GetComponent<GraphicRaycaster>().enabled = true;
        }

        void ICheatService.RegisterCheatProvider(ICheatsProvider cheatsProvider) => _cheatsProviders.Add(cheatsProvider);

        void ICheatService.UnRegisterCheatProvider(ICheatsProvider cheatsProvider) => _cheatsProviders.Remove(cheatsProvider);

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 40;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.button.fontSize = 40;
            GUI.skin.textArea.fontSize = 40;
            GUI.skin.textField.fontSize = 40;
            GUI.skin.box.fontSize = 40;
            GUI.skin.box.fontStyle = FontStyle.Bold;
            
            if (_isShown)
            {
                DrawCheats();
            }
            else
            {
                ShowCheats();
            }
        }

        private void Update()
        {
            if (_isShown)
            {
                var touchCount = Input.touchCount > 0 ? Input.touchCount : Input.GetMouseButton(0) ? 1 : 0;
                if (touchCount > 0 && GUIUtility.hotControl == 0)
                {
                    This.Hide();
                }
            }
        }

        private void DrawCheats()
        {
            const int width = 600;
            GUILayout.BeginArea(new Rect(Screen.width/2f - width/2f, 120, width, Screen.height - 240));
            GUILayout.BeginVertical();
            foreach (var cheatsProvider in _cheatsProviders)
            {
                GUILayout.BeginVertical(cheatsProvider.Id, "box");
                GUILayout.Space(40);
                cheatsProvider.OnGUI();
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void ShowCheats()
        {
            if (GUI.Button(new Rect(Screen.width - 120, Screen.height - 100, 120, 100), "CHTS"))
            {
                if (!_isShown)
                {
                    This.Show();
                }
                else
                {
                    This.Hide();
                }
            }
        }
    }
}