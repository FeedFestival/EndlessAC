using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "SavedLightmapSettings", menuName = "Game/Saved Lightmap Settings", order = 1)]
public class SavedLightmapSettingsSO : ScriptableObject {
    public SphericalHarmonicsL2[] bakedProbes;
}
