using UnityEngine;

namespace Modules.Extensions
{
    public static class RectExtensions
    {
        // Returns the smallest Rect that contains both rectA and rectB
        public static Rect Union(this Rect rectA, Rect rectB)
        {
            var xMin = Mathf.Min(rectA.xMin, rectB.xMin);
            var yMin = Mathf.Min(rectA.yMin, rectB.yMin);
            var xMax = Mathf.Max(rectA.xMax, rectB.xMax);
            var yMax = Mathf.Max(rectA.yMax, rectB.yMax);
        
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}