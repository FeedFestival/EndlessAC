using GameScrypt.GSDictionary;
using GameScrypt.JsonData;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Main {
    [System.Serializable]
    public struct EntityDeff {
        public string EntityType;
        public UDictionary<string, int> EntityName;
    }

    public static class EntityDefinition {

        public const int MAX_CUSTOM_VARIATION = 33;

        public static int GetId(int typeId, int nameId, string variationNr) {
            string typeNr = typeId.ToString();
            string nameNr = nameId.ToString("D2");

            var sId = typeNr + "" + nameNr + "" + variationNr;
            return Convert.ToInt32(sId);
        }

        public static string GetNameFromId(int id, Dictionary<string, Dictionary<string, int>> entities, Dictionary<string, int> variations) {
            return GetNameFromId(id, ref entities, ref variations);
        }

        public static string GetNameFromId(int id, ref Dictionary<string, Dictionary<string, int>> entities, ref Dictionary<string, int> variations) {
            string sId = id.ToString();
            var type = GetTypeFromId(sId, ref entities);
            var name = GetEntityName(type, sId, ref entities);

            var fullName = type + "." + name;

            // ex:   10101 [1.01.01]
            if (id > 9999) {
                var variation = GetVariationFromId(sId, ref variations, out int varNr);
                if (variation == string.Empty || variation == "None") {
                    return fullName;
                }

                if (variation == "Custom") {
                    fullName += "." + varNr.ToString("D2");
                } else {
                    fullName += "." + variation;
                }
            }

            return fullName;
        }

        public static string GetTypeFromId(string sId, ref Dictionary<string, Dictionary<string, int>> entities) {
            // ex:   101 [1.01]
            int i = 1;
            int typeId = int.Parse(sId.Substring(0, 1));
            foreach (var entityType in entities) {
                if (i == typeId) {
                    return entityType.Key;
                }
                i++;
            }
            return "";
        }

        public static string GetEntityName(string entityType, string sId, ref Dictionary<string, Dictionary<string, int>> entities) {
            int nameId = int.Parse(sId.Substring(1, 2));

            foreach (var kvp in entities[entityType]) {
                if (kvp.Value == nameId) {
                    return kvp.Key;
                }
            }
            return "";
        }

        public static string GetVariationFromId(string sId, ref Dictionary<string, int> variations, out int varNr) {
            string variation = "";
            varNr = int.Parse(sId.Substring(3, 2));
            if (varNr >= MAX_CUSTOM_VARIATION) {
                variation = "Custom";
            } else {
                foreach (var varKvp in variations) {
                    if (varKvp.Value == varNr) {
                        variation = varKvp.Key;
                        break;
                    }
                }
            }
            return variation;
        }

        public static string GetVariationNr(string variation, int customVariation, ref Dictionary<string, int> _variationsDict) {
            if (variation == "0" || variation == "None") {
                return "";
            }
            if (variation == "Custom") {
                return customVariation.ToString("D2");
            }

            return (Convert.ToInt32(_variationsDict[variation])).ToString("D2");
        }

        internal static string[] GetEntityTypes(Dictionary<string, Dictionary<string, int>> entities) {
            var list = new List<string>();
            foreach (var entityType in entities) {
                list.Add(entityType.Key);
            }
            return list.ToArray();
        }

        internal static string[] GetEntityNames(string type, Dictionary<string, Dictionary<string, int>> entities) {
            var list = new List<string>();
            try {
                foreach (var entityType in entities[type]) {
                    list.Add(entityType.Key);
                }
            } catch (Exception e) {
                Debug.LogError("type: " + type + ", err: " + e.Message);
            }
            return list.ToArray();
        }

        internal static string[] GetEntityVariations(Dictionary<string, int> variationDict) {
            var list = new List<string>();
            foreach (var entityType in variationDict) {
                list.Add(entityType.Key);
            }
            return list.ToArray();
        }

        public static Dictionary<string, Dictionary<string, int>> CreateEntitiesForSave(List<EntityDeff> entityTypes) {
            var entities = new Dictionary<string, Dictionary<string, int>>();

            foreach (var entityType in entityTypes) {
                var entitiesName = new Dictionary<string, int>();

                foreach (var entityName in entityType.EntityName) {
                    entitiesName.Add(entityName.Key, entityName.Value);
                }
                entities.Add(entityType.EntityType, entitiesName);
            }

            return entities;
        }

        internal static int GetEntityTypeId(string type, ref Dictionary<string, Dictionary<string, int>> entities) {
            int i = 1;
            foreach (var kvp in entities) {
                if (kvp.Key == type) {
                    return i;
                }
                i++;
            }
            return -1;
        }

        internal static Dictionary<string, Dictionary<string, int>> GetEntitiesFromJson(JSONNode jsonNode, ref int entitiesCount) {
            entitiesCount = 0;
            var entities = new Dictionary<string, Dictionary<string, int>>();

            foreach (var entityType in jsonNode) {
                var entitiesName = new Dictionary<string, int>();
                if (entityType.Key == "Variations") { continue; }
                foreach (var entityName in entityType.Value) {
                    entitiesCount++;
                    entitiesName.Add(entityName.Key, entityName.Value);
                }
                entitiesCount++;
                entities.Add(entityType.Key, entitiesName);
            }
            return entities;
        }

        internal static Dictionary<string, int> GetVariationsFromJson(JSONNode jsonNode) {
            var variations = new Dictionary<string, int>();
            foreach (var variation in jsonNode["Variations"]) {
                variations.Add(variation.Key, variation.Value);
            }
            return variations;
        }

#if UNITY_EDITOR
        internal static JSONNode LoadEntitiesJson() {
            string[] guids = AssetDatabase.FindAssets("entitiesDefinition");
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            var json = GSJsonService.ReadJsonFromFilePath(path);
            return GSJsonData.GetParsedJson(json);
        }
#endif
    }
}
