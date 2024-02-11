using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ToggleableLightsGroup : MonoBehaviour
{
    [SerializeField]
    private BakeryLightMesh[] _bakeryLightMeshes;

    [SerializeField]
    private float _brightIntensity;

    [SerializeField]
    private float _darkIntensity = 0;

    internal void ChangeIntensity(bool bright, bool justActivation) {

        var intensity = bright ? _brightIntensity : _darkIntensity;

        foreach (var bakeryLightMesh in _bakeryLightMeshes) {

            if (justActivation) {
                bakeryLightMesh.gameObject.SetActive(bright);
            }  else {
                bakeryLightMesh.intensity = intensity;
                var lightMeshInspector = Editor.CreateEditor(bakeryLightMesh) as ftLightMeshInspector;
                if (lightMeshInspector != null) {
                    lightMeshInspector.SyncMaterialAndLight();
                }
            }
        }
    }
}
