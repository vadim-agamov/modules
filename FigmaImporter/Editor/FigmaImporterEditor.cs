using UnityEditor;
using UnityEngine;

namespace Modules.FigmaImporter.Editor
{
    [CustomEditor(typeof(FigmaNodeImporter))]
    public class FigmaImporterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Import"))
            {
                ((FigmaNodeImporter)target).Import();
            }
        }
    }
}