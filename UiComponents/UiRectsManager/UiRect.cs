using System;
using System.Linq;
using UnityEngine;

namespace Modules.UiComponents.UiRectsManager
{
    [RequireComponent(typeof(RectTransform))]
    public class UiRect : MonoBehaviour
    {
        [SerializeField]
        private string _elementId;
        
        private Rect _previousRect;
        private Canvas _canvas;

        public event Action OnRectTransformDimensionsChanged;

        public RectTransform RectTransform { get; set; }

        public Rect GetScreenRect(Camera cam)
        {
            var corners = new Vector3[4];
            RectTransform.GetWorldCorners(corners);
            var screenCorners = corners.Select(cam.WorldToScreenPoint).ToArray();
            return new Rect(screenCorners.Min(c => c.x), screenCorners.Min(c => c.y), screenCorners.Max(c => c.x), screenCorners.Max(c => c.y));
        }

        private void Awake() => RectTransform = GetComponent<RectTransform>();

        private void OnEnable()
        {
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null)
            {
                throw new Exception("UiRect must be a child of a Canvas");
            }

            UiRectsManager.Register(this, _elementId);
            
            _previousRect = RectTransform.rect;
     
        }

        private void OnDisable() => UiRectsManager.Unregister(_elementId);

        private void Update()
        {
            if(_previousRect != RectTransform.rect)
            {
                _previousRect = RectTransform.rect;
                OnRectTransformDimensionsChanged?.Invoke();
            }
        }
    }
}
