using Cysharp.Threading.Tasks;
using UnityEngine;
using Screen = UnityEngine.Screen;

namespace Modules.UiComponents
{
    public static class CameraExtensions
    { 
        public static Rect GetWorldRect(this RectTransform rectTransform)
        {
            var worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            // Calculate width and height in world space
            var width = Vector3.Distance(worldCorners[0], worldCorners[3]); // Bottom-left to Bottom-right
            var height = Vector3.Distance(worldCorners[0], worldCorners[1]); // Bottom-left to Top-left

            return new Rect(worldCorners[0].x, worldCorners[0].y, width, height);
        }
        
        public static void FitOrthographic(this Camera camera, Rect worldRect, float padding)
        {
            // portrait
            var orthographicSizePortrait = worldRect.height / 2;

            // landscape
            var ratio = Screen.height / (float)Screen.width;
            var orthographicSizeLandscape = ratio * worldRect.width / 2;

            // The orthographicSize is half the size of the vertical viewing volume.
            camera.orthographicSize = padding + Mathf.Max(orthographicSizePortrait, orthographicSizeLandscape);
            camera.transform.position = new Vector3(worldRect.center.x, worldRect.center.y, camera.transform.position.z);
        }
        
        public static async UniTask FitOrthographic(this Camera camera, Rect worldRect, RectTransform uiRectTransform)
        {
            // Calculate the orthographic size based on the screen size
            
            // Screen Space - Overlay --> world coordinates match screen coordinates
            var targetScreenRect = uiRectTransform.GetWorldRect();
            var factorHeight = Screen.height / targetScreenRect.height;
            var factorWight = Screen.width / targetScreenRect.width;
            
            // portrait
            var orthographicSizePortrait = factorHeight * worldRect.height / 2;
            
            // landscape
            var ratio = Screen.height / (float)Screen.width;
            var orthographicSizeLandscape = factorWight * ratio * worldRect.width / 2;

            // The orthographicSize is half the size of the vertical viewing volume.
            camera.orthographicSize = Mathf.Max(orthographicSizePortrait, orthographicSizeLandscape);
            
            await UniTask.Yield();
            
            // Calculate the camera position based on the screen size
            var screenOffset = new Vector2(Screen.width / 2f, Screen.height / 2f) - targetScreenRect.center;
            var screenToWorld = new Vector2(0,  (2 * camera.orthographicSize) / Screen.height); 
            var cameraPosition = worldRect.center + screenOffset * screenToWorld;
            
            camera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, camera.transform.position.z);
        }
    }
}