using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "SavedLightmapSettings", menuName = "Game/Saved Lightmap Settings", order = 1)]
public class SavedLightmapSettingsSO : ScriptableObject {
    private SphericalHarmonicsL2[] _bakedProbes;
    private const string _filePath = "Assets/Resources";    // TODO: change to "Assets/Resources/Bakery/baked/{sceneName}"

    public void SaveBakedProbes(string fileName, SphericalHarmonicsL2[] bakedProbes) {
        _bakedProbes = bakedProbes;
        var filePath = _filePath + "/" + fileName + ".bytes";
        
        var serializableProbes = new SerializableSphericalHarmonicsL2[_bakedProbes.Length];

        for (int i = 0; i < _bakedProbes.Length; i++) {
            serializableProbes[i] = new SerializableSphericalHarmonicsL2(_bakedProbes[i]);
        }

        var formatter = new BinaryFormatter();
        using (var fileStream = File.Create(filePath)) {
            formatter.Serialize(fileStream, serializableProbes);
        }
        Debug.Log("Baked probes saved to: " + filePath);
    }

    public SphericalHarmonicsL2[] GetBakedProbes(string fileName) {
        if (_bakedProbes == null || _bakedProbes.Length == 0) {
            _bakedProbes = loadBakedProbes(fileName);
        }
        return _bakedProbes;
    }

    private SphericalHarmonicsL2[] loadBakedProbes(string resourceName) {
        SphericalHarmonicsL2[] loadedProbes = null;

        Debug.Log("-> loadBakedProbes()");

        var textAsset = Resources.Load<TextAsset>(resourceName);
        if (textAsset != null) {
            using (var memoryStream = new MemoryStream(textAsset.bytes)) {
                var formatter = new BinaryFormatter();
                var serializableProbes = (SerializableSphericalHarmonicsL2[])formatter.Deserialize(memoryStream);

                // Convert SerializableSphericalHarmonicsL2 back to SphericalHarmonicsL2
                loadedProbes = new SphericalHarmonicsL2[serializableProbes.Length];
                for (int i = 0; i < serializableProbes.Length; i++) {
                    loadedProbes[i] = serializableProbes[i].ToSphericalHarmonicsL2();
                }
            }
            Debug.Log("Baked probes loaded from Resources/" + resourceName);
        } else {
            Debug.LogError("Failed to load resource: " + resourceName);
        }

        return loadedProbes;
    }
}

[Serializable]
public class SerializableSphericalHarmonicsL2 {
    public float[] coefficients;

    public SerializableSphericalHarmonicsL2(SphericalHarmonicsL2 sh) {
        coefficients = new float[27];
        int index = 0;

        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 9; j++) {
                coefficients[index++] = sh[i, j];
            }
        }
    }

    public SphericalHarmonicsL2 ToSphericalHarmonicsL2() {
        var sh = new SphericalHarmonicsL2();
        int index = 0;

        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 9; j++) {
                sh[i, j] = coefficients[index++];
            }
        }

        return sh;
    }
}
