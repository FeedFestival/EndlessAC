using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameScene_BeginingTutorial : MonoBehaviour {
    [SerializeField]
    private Texture2D[] _darkLightmapDir, _darkLightmapColor;
    [SerializeField]
    private Texture2D[] _brightLightmapDir, _brightLightmapColor;
    [SerializeField]
    private LightmapChanger _lightmapChanger;

    [SerializeField]
    private LightmapData _lightmapData;

    private bool _isLightswitchOn;

    private void Start() {

        _lightmapChanger.SetupDifferentLightmaps(_darkLightmapDir, _darkLightmapColor, _brightLightmapDir, _brightLightmapColor);

        _isLightswitchOn = false;

        ToggleLightswtich();
    }

    public void ToggleLightswtich() {

        if (_isLightswitchOn) {
            _lightmapChanger.onLightSwitchOn();
        } else {
            _lightmapChanger.onLightSwitchOff();
        }

        _isLightswitchOn = !_isLightswitchOn;
    }
}
