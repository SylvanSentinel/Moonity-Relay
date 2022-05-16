using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoonNetworkManager))]
public class MoonInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(Resources.Load<Texture>("borpa"), GUILayout.Height(50), GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        base.OnInspectorGUI();

    }


}
