using Game.Scene;
using UnityEngine;

public class GameScene_BeginingTutorial : GameScene {
    [SerializeField]
    private Texture2D[] _darkLightmapDir, _darkLightmapColor;
    [SerializeField]
    private Texture2D[] _brightLightmapDir, _brightLightmapColor;
    [SerializeField]
    private LightmapChanger _lightmapChanger;

    [SerializeField]
    private LightmapData _lightmapData;

    private bool _isLightswitchOn;

    // This is called from Player when everything is ready
    public override void StartScene() {

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
