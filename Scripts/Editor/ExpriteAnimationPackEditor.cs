using UnityEngine;
using Exprite;
using UnityEditor;

[CustomEditor(typeof(ExpriteAnimationPack))]
public class ExpriteAnimationPackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ExpriteAnimationPack animationPack = (ExpriteAnimationPack)target;

        GUILayout.Label("Exprite Animation Pack Editor", EditorStyles.boldLabel);

        // Texture and Atlas Section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Assets", EditorStyles.boldLabel);
        animationPack.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", animationPack.texture, typeof(Texture2D), false);
        animationPack.atlas = (TextAsset)EditorGUILayout.ObjectField("Atlas", animationPack.atlas, typeof(TextAsset), false);

        // Global Offset Section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        animationPack.globalOffset = EditorGUILayout.Vector2Field("Global Offset", animationPack.globalOffset);

        // Animations Section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
        SerializedProperty animationsProperty = serializedObject.FindProperty("animations");
        animationsProperty.isExpanded = EditorGUILayout.Foldout(animationsProperty.isExpanded, "Animation List");
        if (animationsProperty.isExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(animationsProperty, true); // true to include children
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
