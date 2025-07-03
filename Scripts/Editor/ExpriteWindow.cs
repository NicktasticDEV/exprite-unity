using UnityEditor;
using UnityEngine;

public class ExpriteWindow : EditorWindow
{
    [MenuItem("Window/Exprite")]
    public static void ShowWindow()
    {
        GetWindow<ExpriteWindow>("Exprite");
    }

    private void OnGUI()
    {
        GUILayout.Label("Exprite Import", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Import"))
        {
        }
        if (GUILayout.Button("Export"))
        {
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        // Add more settings and options here
    }
    
}
