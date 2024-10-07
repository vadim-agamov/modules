using UnityEngine;

namespace Modules.UiComponents
{
    public static class CameraExtensions
    {
        public static void FitOrthographic(this Camera camera, Rect rect, float padding)
        {
            // portrait
            var orthographicSizePortrait = rect.height / 2;

            // landscape
            var ratio = Screen.height / (float)Screen.width;
            var orthographicSizeLandscape = ratio * rect.width / 2;

            // The orthographicSize is half the size of the vertical viewing volume.
            camera.orthographicSize = padding + Mathf.Max(orthographicSizePortrait, orthographicSizeLandscape);
            
            camera.transform.position = new Vector3(rect.center.x, rect.center.y, camera.transform.position.z);
        }
    }
}