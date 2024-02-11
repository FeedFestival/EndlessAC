using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LightmapChanger : MonoBehaviour {
    [Header("Settings")]
    [SerializeField]
    private BakeryLightmapGroupSelector _bakeryLightmapGroupSelector;
    [SerializeField]
    private LightProbeGroup _lightProbeGroup;
    [SerializeField]
    private bool _activateDeactivateLights;
    public bool Bright;

    [Header("Toggleable Lights")]
    [SerializeField]
    private ToggleableLightsGroup[] _toggleableLightsGroups;

    [Header("Light")]
    [SerializeField]
    private Object _lightBakeryLightmapGroup;
    [SerializeField]
    private SavedLightmapSettingsSO _brightLightmapSettings;

    [Header("Dark")]
    [SerializeField]
    private Object _darkBakeryLightmapGroup;
    [SerializeField]
    private SavedLightmapSettingsSO _darkLightmapSettings;

    private LightmapData[] _darkLightmap, _brightLightmap;

    public void Setup(bool bright) {
        if (bright) {

            _bakeryLightmapGroupSelector.lmgroupAsset = _lightBakeryLightmapGroup;

        } else {

            _bakeryLightmapGroupSelector.lmgroupAsset = _darkBakeryLightmapGroup;

        }

        foreach (var tlGroup in _toggleableLightsGroups) {
            tlGroup.ChangeIntensity(bright, _activateDeactivateLights);
        }

        Bright = bright;
        gameObject.name = "ENVIRONMENT (" + (Bright ? "Bright" : "Dark") + ")";

        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    public void BakeLightmap() {

        Debug.Log("IS_BRIGHT: " + Bright);

        ftRenderLightmap.instance.RenderButton();
        ftRenderLightmap.OnFinishedFullRender += onFinishedFullRender;
    }

    public void SetupDifferentLightmaps(Texture2D[] darkLightmapDir, Texture2D[] darkLightmapColor, Texture2D[] brightLightmapDir, Texture2D[] brightLightmapColor) {
        _darkLightmap = SetupLightmap(darkLightmapDir, darkLightmapColor);
        _brightLightmap = SetupLightmap(brightLightmapDir, brightLightmapColor);
    }

    public void onLightSwitchOff() {
        LightmapSettings.lightmaps = _darkLightmap;
        LightmapSettings.lightProbes.bakedProbes = _darkLightmapSettings.bakedProbes;

        foreach (var tlGroup in _toggleableLightsGroups) {
            tlGroup.ChangeIntensity(false, true);
        }
    }

    public void onLightSwitchOn() {
        LightmapSettings.lightmaps = _brightLightmap;
        LightmapSettings.lightProbes.bakedProbes = _brightLightmapSettings.bakedProbes;

        foreach (var tlGroup in _toggleableLightsGroups) {
            tlGroup.ChangeIntensity(true, true);
        }
    }

    private void onFinishedFullRender(object sender, System.EventArgs e) {

        if (Bright) {
            _brightLightmapSettings.bakedProbes = LightmapSettings.lightProbes.bakedProbes;
        } else {
            _darkLightmapSettings.bakedProbes = LightmapSettings.lightProbes.bakedProbes;
        }

        Debug.Log("END - IS_BRIGHT: " + Bright + "Application.dataPath: " + Application.dataPath);
        copyLightmaps();

        ftRenderLightmap.OnFinishedFullRender -= onFinishedFullRender;
    }

    private void copyLightmaps() {
        var sourceFile = getSceneFolder();
        var destination = sourceFile + (Bright ? "/light/" : "/dark/");
        var envFileName = Bright ? _lightBakeryLightmapGroup.name : _darkBakeryLightmapGroup.name;
        Debug.Log("copyLightmaps() -> : IS_BRIGHT: " + Bright);
        CopyFile(sourceFile, destination, envFileName + "_dir.tga");
        CopyFile(sourceFile, destination, envFileName + "_final.hdr");
    }

    private void CopyFile(string fromPath, string toPath, string fileName) {
        // Combine the source and destination paths with the file name
        string sourceFilePath = Path.Combine(fromPath, fileName);
        string destinationFilePath = Path.Combine(toPath, fileName);

        try {
            // Check if the source file exists
            if (File.Exists(sourceFilePath)) {
                // Copy the file
                File.Copy(sourceFilePath, destinationFilePath, true);
                Debug.Log("File copied successfully.\ndestinationFilePath: " + destinationFilePath);
            } else {
                Debug.LogError("Source file does not exist.\nsourceFilePath: " + sourceFilePath);
            }
        } catch (IOException e) {
            Debug.LogError($"Error copying file: {e.Message}");
        }
    }

    private string getSceneFolder() {
        string path = string.Empty;
        string[] pathParts;
        var appPath = Application.dataPath;
        pathParts = appPath.Split('/');
        for (int i = 0; i < pathParts.Length - 1; i++) {
            path += "/" + pathParts[i];
        }
        path = path.Substring(1);

        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        pathParts = currentScenePath.Split('/');

        for (int i = 0; i < pathParts.Length - 1; i++) {
            path += "/" + pathParts[i];
        }
        //path = path.Substring(1);
        path += "/" + pathParts[pathParts.Length - 2];

        return path;
    }

    public static LightmapData[] SetupLightmap(Texture2D[] lightmapDir, Texture2D[] lightmapColor) {

        var lightMap = new List<LightmapData>();

        for (int i = 0; i < lightmapDir.Length; i++) {
            var lmData = new LightmapData();

            lmData.lightmapDir = lightmapDir[i];
            lmData.lightmapColor = lightmapColor[i];

            lightMap.Add(lmData);
        }

        return lightMap.ToArray();
    }
}
