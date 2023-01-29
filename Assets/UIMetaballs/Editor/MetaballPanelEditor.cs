using UIMetaballs.Runtime;
using UnityEditor;
using UnityEngine;

namespace UIMetaballs.Editor
{
    [CustomEditor(typeof(MetaballPanel))]
    public class MetaballPanelEditor : UnityEditor.Editor
    {
        SerializedObject so;
        
        SerializedProperty pResolution;
        SerializedProperty pAntiAliasing;
        SerializedProperty pColorBlending;
        SerializedProperty pCorrectColoring;
        SerializedProperty pBGColor;
        SerializedProperty pOutlineColor;
        SerializedProperty pUseCanvasSize;
        SerializedProperty pResolutionDivider;
        SerializedProperty pResolutionMultiplier;
        SerializedProperty pUseOutline;

        void OnEnable()
        {
            so = serializedObject;
            
            pResolution = so.FindProperty("_resolution");
            pAntiAliasing = so.FindProperty("_antiAliasing"); 
            pColorBlending = so.FindProperty("_colorBlending");
            pCorrectColoring = so.FindProperty("_correctColoring");
            pBGColor = so.FindProperty("_backgroundColor");
            pOutlineColor = so.FindProperty("_outlineColor");
            pUseCanvasSize = so.FindProperty("_useCanvasSize");
            pResolutionDivider = so.FindProperty("_resolutionDivider");
            pResolutionMultiplier = so.FindProperty("_resolutionMultiplier");

            pUseOutline = so.FindProperty("_useOutline");
        }

        public override void OnInspectorGUI()
        {
            MetaballPanel component = (MetaballPanel)target;

            so.Update();

            #region Metaball
            
            var centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 20};
            EditorGUILayout.LabelField("Metaball Panel", centeredStyle);
            EditorGUILayout.Space();

            var centerStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 14};
            EditorGUILayout.LabelField("Resolution Options", centerStyle);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(pUseCanvasSize);
                EditorGUILayout.PropertyField(pResolutionDivider);
                EditorGUILayout.PropertyField(pResolutionMultiplier);
                //_useCanvasSize = EditorGUILayout.Toggle("Use canvas size", _useCanvasSize);

                EditorGUI.BeginDisabledGroup(pUseCanvasSize.boolValue);
                EditorGUILayout.PropertyField(pResolution);
                EditorGUI.EndDisabledGroup();

                if (pUseCanvasSize.boolValue)
                {
                    pResolution.vector2IntValue = new Vector2Int((int)(component.rectTransform.rect.width*component._resolutionMultiplier/component._resolutionDivider), (int)(component.rectTransform.rect.height*component._resolutionMultiplier/component._resolutionDivider));
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Panel Options", centerStyle);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(pAntiAliasing);
                EditorGUILayout.PropertyField(pColorBlending);
                EditorGUILayout.PropertyField(pCorrectColoring);
                EditorGUILayout.PropertyField(pBGColor);

                EditorGUILayout.PropertyField(pUseOutline);
                if(pUseOutline.boolValue)
                    EditorGUILayout.PropertyField(pOutlineColor);
            }

            #endregion
            
            so.ApplyModifiedProperties();
        }
        
        [MenuItem("GameObject/UI/Metaball Panel", false, 0)]
        static void CreateMetaballPanel()
        {
            var mp = new GameObject("Metaball Panel", typeof(MetaballPanel));
            if(Selection.gameObjects[0] != null)
                mp.transform.SetParent(Selection.gameObjects[0].transform);

            var rect = mp.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;

            rect.localScale = Vector3.one;

            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}