#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Main;

public class EntityHelperWindow : EditorWindow {

    //Dictionary<string, Dictionary<string, int>> _entities;
    //private Dictionary<string, int> _variationsDict;

    private readonly string[] _folders = new string[2] {
        "Assets/___ RESOURCE ___/",
        "Assets/_Game/",
        //"Assets/___ RESOURCE ___/",
    };

    [MenuItem("Game/Entity Helper")]
    static void OpenWindow() {

        var window = (EntityHelperWindow)GetWindow(typeof(EntityHelperWindow));
        //window.minSize = new Vector2(600, 300);

        window.Show();
    }

    private void OnGUI() {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width / 2, 100));

        var s = "Will check prefabs in the project that sit in these Folders: ";
        foreach (var f in _folders) {
            s += f + "; ";
        }

        GUILayout.Label(s);

        if (GUILayout.Button("Check only Entities")) {

            checkEntities();
        }

        if (GUILayout.Button("Check Prefabs Entities")) {

            checkEntities(lookForAllPrefabs: true);
        }

        GUILayout.EndArea();
    }

    private void checkEntities(bool lookForAllPrefabs = false) {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        var lookedUpEntities = new Dictionary<string, string>();

        foreach (var guid in guids) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (isInFolder(path) == false) {
                continue;
            }

            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var entityId = go.GetComponent<EntityId>();

            if (entityId == null) {
                if (lookForAllPrefabs == false) {
                    continue;
                }
                Debug.LogWarning("Prefab (" + go.name + ") doesn't have an Entity Id Component. Is this right? \n Found at path: " + path);
                continue;
            }

            string entityName = entityId.GetName();

            bool hasSameName = go.name == entityName;

            if (!hasSameName) {
                Debug.LogWarning("Prefab (" + go.name + ") doesn't have the same name as it's entity: " + entityName + " != " + go.name);
            }

            if (lookedUpEntities.ContainsKey(entityName) == false) {
                lookedUpEntities.Add(entityName, path);
            } else {
                Debug.LogError("Duplicate entity name. " + entityName + " at: " + path + ". \n Other entity is at: " + lookedUpEntities[entityName]);
                continue;
            }

            if (hasSameName) {
                var entities = EntityDefinitionConsts.ENTITIES;
                var variations = EntityDefinitionConsts.VARIATIONS;
                var variationNr = EntityDefinition.GetVariationNr(entityId.Variation, entityId.CustomVariation, ref variations);
                var id = EntityDefinition.GetId(
                    typeId: EntityDefinition.GetEntityTypeId(entityId.Type, ref entities),
                    nameId: entities[entityId.Type][entityId.Name],
                    variationNr
                );
                var nameId = entityId.GetName() + "[" + id + "]";
                var nameFromId = EntityDefinition.GetNameFromId(id, ref entities, ref variations);
                Debug.Log("All fine for: " + nameId + "\n" +
                    "Deconstructing: " + nameFromId
                );
            }
        }
    }

    private bool isInFolder(string path) {
        bool contains = false;
        foreach (var f in _folders) {
            contains = path.Contains(f);
            if (contains == false)
                continue;
            return true;
        }
        return contains;
    }
}
#endif