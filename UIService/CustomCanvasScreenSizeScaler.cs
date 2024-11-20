using UnityEngine.Device;
using UnityEngine.UI;

namespace Modules.UIService
{
    public class CustomCanvasScreenSizeScaler : CanvasScaler
    {
        protected override void Handle()
        {
            var screenRatio = Screen.width / (float) Screen.height;
            var referenceRatio = referenceResolution.x / referenceResolution.y;
            
            // wider than reference -> match by height
            if (screenRatio > referenceRatio)
            {
                matchWidthOrHeight = 1;
            }
            
            // thinner than reference -> match by weight
            else if (screenRatio < referenceRatio)
            {
                matchWidthOrHeight = 0;
            }
            
            base.Handle();
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            uiScaleMode = ScaleMode.ScaleWithScreenSize;
        }
#endif
    }
}
