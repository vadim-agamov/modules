#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Modules.UIService
{
    [CustomEditor(typeof(CustomCanvasScreenSizeScaler))]
    public class CustomCanvasScreenSizeScalerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Screen is wider than reference, then match by height\n" +
                                       "Screen is thinner than reference, then match by weight\n",
                GUILayout.Height(30));
            
            var referenceResolutionProperty = serializedObject.FindProperty("m_ReferenceResolution");
            EditorGUILayout.PropertyField(referenceResolutionProperty);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
