using GameScrypt.GSDictionary;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using GameScrypt.JsonData;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Main {

    [CreateAssetMenu(fileName = "EntityDefinition", menuName = "ScriptableObjects/EntityDefinition", order = 1)]
    public class EntityDefinitionSO : ScriptableObject {

        public List<EntityDeff> EntityTypes;
        public UDictionary<string, int> EntityVariation;

        public int EntitiesCount { get => _entitiesCount; }
        private int _entitiesCount = 0;
        private Dictionary<string, Dictionary<string, int>> _entities;
        private Dictionary<string, int> _variations;

        public void CreateEntitiesFromJson(JSONNode jsonNode) {
            _entities = EntityDefinition.GetEntitiesFromJson(jsonNode, ref _entitiesCount);
            _variations = EntityDefinition.GetVariationsFromJson(jsonNode);
        }

        public void LoadFromJsonForEditing() {
            EntityTypes = new List<EntityDeff>();

            foreach (var entityType in _entities) {
                var entitiesName = new UDictionary<string, int>();
                foreach (var entityName in entityType.Value) {
                    entitiesName.Add(entityName.Key, entityName.Value);
                }

                var editEntityType = new EntityDeff() {
                    EntityType = entityType.Key,
                    EntityName = entitiesName
                };
                EntityTypes.Add(editEntityType);
            }

            EntityVariation = new UDictionary<string, int>();
            foreach (var varKvp in _variations) {
                EntityVariation.Add(varKvp.Key, varKvp.Value);
            }
        }

        public void ResetInspectorValues() {
            EntityTypes = null;
            EntityVariation = null;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(EntityDefinitionSO))]
    public class EntityDefinitionSOInspector : Editor {

        private bool _editMode = false;

        public override void OnInspectorGUI() {

            var script = (EntityDefinitionSO)target;

            if (_editMode) {
                base.OnInspectorGUI();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Cancel")) {
                    script.ResetInspectorValues();
                    _editMode = false;
                }
                if (GUILayout.Button("Save")) {
                    save(script);
                    script.ResetInspectorValues();
                    _editMode = false;
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Label("Entities: " + script.EntitiesCount);
            GUILayout.Space(10);
            GUILayout.Label("The above is a representation of the entitiesDefinition.json.");
            GUILayout.Label("It is not used in the actual game.");
            GUILayout.Label("The ones that are used are Entities Dictionary.");

            if (GUILayout.Button("Load latest .json")) {
                var jsonNode = EntityDefinition.LoadEntitiesJson();
                script.CreateEntitiesFromJson(jsonNode);
                script.LoadFromJsonForEditing();
                _editMode = true;
            }

            if (GUILayout.Button("Create Entities from Json")) {
                var jsonNode = EntityDefinition.LoadEntitiesJson();
                script.CreateEntitiesFromJson(jsonNode);
                script.ResetInspectorValues();
            }
        }

        private void save(EntityDefinitionSO script) {
            var entitiesToSave = EntityDefinition.CreateEntitiesForSave(script.EntityTypes);
            var path = getPathOfAsset("entitiesDefinition");
            var json = createJsonString(entitiesToSave, script.EntityVariation);
            File.WriteAllText(path, json);

            path = getPathOfAsset("EntityDefinitionConsts");
            var csFile = createCsFile(entitiesToSave, script.EntityVariation);
            File.WriteAllText(path, csFile);
        }

        private string createJsonString(Dictionary<string, Dictionary<string, int>> entities, UDictionary<string, int> variations) {
            var json = "{\n";

            foreach (var entityType in entities) {
                json += "\t\"" + entityType.Key + "\": {\n";

                var toSaveValues = new Dictionary<int, string>();
                foreach (var entityName in entityType.Value) {
                    json += "\t\t\"" + entityName.Key + "\": " + entityName.Value + ",\n";

                    toSaveValues.Add(entityName.Value, entityName.Key);
                }
                json += "\t},\n";
            }

            json += "\t\"Variations\": {\n";

            var toSaveVarValues = new Dictionary<int, string>();
            foreach (var variation in variations) {
                json += "\t\t\"" + variation.Key + "\": " + variation.Value + ",\n";

                toSaveVarValues.Add(variation.Value, variation.Key);
            }
            json += "\t},\n";

            json += "}";

            return json;
        }

        private string createCsFile(Dictionary<string, Dictionary<string, int>> entities, UDictionary<string, int> variations) {
            var cs = @"using System.Collections.Generic;

namespace Game.Main {
    public static class EntityDefinitionConsts {
        public static readonly Dictionary<string, Dictionary<string, int>> ENTITIES;
        public static readonly Dictionary<string, int> VARIATIONS;

        static EntityDefinitionConsts() {
            ENTITIES = new Dictionary<string, Dictionary<string, int>>() {";

            foreach (var entityType in entities) {
                cs += @"
                {
                    " + "\"" + entityType.Key + "\"" + @",
                    new Dictionary<string, int>() { 
";
                

                var toSaveValues = new Dictionary<int, string>();
                foreach (var entityName in entityType.Value) {
                    cs += "\t\t\t\t\t\t{ \"" + entityName.Key + "\", " + entityName.Value + " },\n";

                    toSaveValues.Add(entityName.Value, entityName.Key);
                }
                cs += @"
                    }";
                cs += @"
                },";
            }

            cs += @"
            };
            VARIATIONS = new Dictionary<string, int>() {
";
            var toSaveVarValues = new Dictionary<int, string>();
            foreach (var variation in variations) {
                cs += "\t\t\t\t{ \"" + variation.Key + "\", " + variation.Value + " },\n";

                toSaveVarValues.Add(variation.Value, variation.Key);
            }

            cs += @"
            };
";

            cs += @"
        }
    }
}";
            return cs;
        }

        private string getPathOfAsset(string fileName) {
            string[] guids = AssetDatabase.FindAssets(fileName);
            return AssetDatabase.GUIDToAssetPath(guids[0]);
        }
    }
#endif
}
