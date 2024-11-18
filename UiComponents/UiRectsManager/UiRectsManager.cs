using System.Collections.Generic;

namespace Modules.UiComponents.UiRectsManager
{
    public static class UiRectsManager
    {
        private static readonly Dictionary<string, UiRect> _rects = new();

        public static void Register(UiRect worldRect, string elementId)
        {
            if (!_rects.TryAdd(elementId, worldRect))
            {
                throw new System.Exception($"Element with id {elementId} is already registered");
            }
        }
        
        public static void Unregister(string elementId)
        {
            if (!_rects.Remove(elementId))
            {
                throw new System.Exception($"Element with id {elementId} is not registered");
            }
        }
        
        public static bool TryGetWorldRect(string elementId, out UiRect worldRect)
        {
            return _rects.TryGetValue(elementId, out worldRect);
        }
    }
}
