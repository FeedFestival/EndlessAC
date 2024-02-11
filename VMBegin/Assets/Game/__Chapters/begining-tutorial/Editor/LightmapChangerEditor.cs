using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightmapChanger))]
public class LightmapChangerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var lightmapChanger = (LightmapChanger)target;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Setup Bright")) {
            lightmapChanger.Setup(bright: true);
        }

        if (GUILayout.Button("Setup Dark")) {
            lightmapChanger.Setup(bright: false);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Bake " + (lightmapChanger.Bright == true ? "Bright" : "Dark") + " Lightmap Lightmap")) {
            lightmapChanger.BakeLightmap();
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}