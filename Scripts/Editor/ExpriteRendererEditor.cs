using UnityEngine;
using UnityEditor;
using Exprite;

[CustomEditor(typeof(ExpriteRenderer))]
public class ExpriteRendererEditor : Editor
{

    private int globalSelectedAnimationIndex = 0;
    

    public override void OnInspectorGUI()
    {
        ExpriteRenderer expriteRenderer = (ExpriteRenderer)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        // Playback controls
        GUILayout.Label("Playback Controls", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Play"))
        {
            expriteRenderer.Play(expriteRenderer.AnimationPack.animations[globalSelectedAnimationIndex].name);
        }

        if (GUILayout.Button("Pause"))
        {
        }

        if (GUILayout.Button("Stop"))
        {
        }

        GUILayout.EndHorizontal();

        // Dropdown list for selecting an animation
        if (expriteRenderer.AnimationPack != null)
        {
            string[] animationNames = new string[expriteRenderer.AnimationPack.animations.Length];
            for (int i = 0; i < expriteRenderer.AnimationPack.animations.Length; i++)
            {
                animationNames[i] = expriteRenderer.AnimationPack.animations[i].name;
            }

            // Dropdown for selecting an animation
            globalSelectedAnimationIndex = EditorGUILayout.Popup("Animation", globalSelectedAnimationIndex, animationNames);
        }
    }


}
