using UIMetaballs.Runtime;
using UnityEditor;
using UnityEngine;

namespace UIMetaballs.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIMetaball))]
    public class UIMetaballEditor : UnityEditor.Editor
    {
        SerializedObject so;
        
        SerializedProperty propBlending;
        SerializedProperty propColor;
        SerializedProperty propRound;
        SerializedProperty propRoundness;
        SerializedProperty propOutlineWidth;
        //SerializedProperty propLayer;

        void OnEnable()
        {
            so = serializedObject;
            
            propBlending  = so.FindProperty("_blending");
            propColor = so.FindProperty("_color");
            propRound = so.FindProperty("round");
            propRoundness = so.FindProperty("_roundness");
            propOutlineWidth = so.FindProperty("_outlineWidth");
        }
        
        public override void OnInspectorGUI()
        {
            UIMetaball component = (UIMetaball)target;
            
            so.Update();
            
            var centeredStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 20};
            EditorGUILayout.LabelField("Metaball Options", centeredStyle);
            EditorGUILayout.Space();

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(propBlending);
                
                EditorGUILayout.PropertyField(propOutlineWidth);
                //EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.PropertyField(propRound);

                
                EditorGUILayout.EndHorizontal();
                if (!propRound.boolValue)
                {
                    EditorGUILayout.PropertyField(propRoundness);
                }

                //EditorGUILayout.Space();

                EditorGUILayout.PropertyField(propColor);
                //EditorGUILayout.Space();
            }

            so.ApplyModifiedProperties();
        }
        
        [MenuItem("GameObject/UI/UI Metaball", false, 0)]
        static void CreateUIMetaball()
        {
            var mp = new GameObject("UI Metaball", typeof(RectTransform), typeof(UIMetaball));
            mp.layer = LayerMask.NameToLayer("UI");
            mp.transform.SetParent(Selection.gameObjects[0].transform);

            var rect = mp.GetComponent<RectTransform>();

            rect.anchorMin = Vector2.one / 2;
            rect.anchorMax = Vector2.one / 2;
            rect.pivot = Vector2.one / 2;

            rect.anchoredPosition = Vector2.zero;

            rect.localScale = Vector3.one;

            rect.sizeDelta = new Vector2(100, 50);
            
            var metaball = mp.GetComponent<UIMetaball>();
            metaball.Color = Color.white;
            metaball.Blending = 0.1f;
            
            mp.GetComponentInParent<MetaballPanel>().RebuildData();
        }
    }
}
