#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Main {
    [CustomEditor(typeof(EntityId))]
    public class EntityIdInspector : Editor {
        string[] _typeChoices;
        string[] _nameChoices;
        string[] _variationChoices;
        int _typeChoiceIndex = 0;
        int _nameChoiceIndex = 0;
        int _variationChoiceIndex = 0;
        bool _editEntity;

        public override void OnInspectorGUI() {
            var script = (EntityId)target;

            if (_editEntity) {

                GUILayout.BeginHorizontal();
                GUILayout.Label("Entity Type");
                GUILayout.Label(script.Type + " Name");
                GUILayout.Label("Variation");
                GUILayout.EndHorizontal();

                // ...

                GUILayout.BeginHorizontal();

                if (_typeChoices == null) {
                    loadChoices();
                }

                _typeChoiceIndex = _typeChoices.ToList().FindIndex(tc => tc == script.Type);
                if (_typeChoiceIndex == -1) {
                    _typeChoiceIndex = 0;
                    script.Type = _typeChoices[_typeChoiceIndex];
                }

                var typeChanged = false;
                var newTypeChoiceIndex = EditorGUILayout.Popup(_typeChoiceIndex, _typeChoices);
                if (newTypeChoiceIndex != _typeChoiceIndex) {
                    _typeChoiceIndex = newTypeChoiceIndex;
                    script.Type = _typeChoices[_typeChoiceIndex];
                    typeChanged = true;
                    EditorUtility.SetDirty(target);
                }

                if (_nameChoices == null || _nameChoices.Length == 0 || typeChanged) {
                    _nameChoices = EntityDefinition.GetEntityNames(script.Type, EntityDefinitionConsts.ENTITIES);
                }

                _nameChoiceIndex = _nameChoices.ToList().FindIndex(nc => nc == script.Name);
                if (_nameChoiceIndex == -1) {
                    _nameChoiceIndex = 0;
                    script.Name = _nameChoices[_nameChoiceIndex];
                }

                var newNameChoiceIndex = EditorGUILayout.Popup(_nameChoiceIndex, _nameChoices);
                if (newNameChoiceIndex != _nameChoiceIndex) {
                    _nameChoiceIndex = newNameChoiceIndex;
                    script.Name = _nameChoices[_nameChoiceIndex];
                    EditorUtility.SetDirty(target);
                }

                _variationChoiceIndex = _variationChoices.ToList().FindIndex(nc => nc == script.Variation);
                if (_variationChoiceIndex == -1) {
                    _variationChoiceIndex = 0;
                    script.Variation = _variationChoices[_variationChoiceIndex];
                }

                var newVariationChoiceIndex = EditorGUILayout.Popup(_variationChoiceIndex, _variationChoices);
                if (newVariationChoiceIndex != _variationChoiceIndex) {
                    _variationChoiceIndex = newVariationChoiceIndex;
                    script.Variation = _variationChoices[_variationChoiceIndex];
                    EditorUtility.SetDirty(target);
                }

                GUILayout.EndHorizontal();

                if (script.Variation == "Custom") {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Custom Variation");
                    script.CustomVariation = EditorGUILayout.IntSlider(script.CustomVariation, EntityDefinition.MAX_CUSTOM_VARIATION, 99);
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                calculateID(script, EntityDefinitionConsts.ENTITIES);
                var entities = EntityDefinitionConsts.ENTITIES;
                var variations = EntityDefinitionConsts.VARIATIONS;
                var deconstructedName = EntityDefinition.GetNameFromId(script.ID, ref entities, ref variations);
                GUILayout.Label("ID: " + script.ID + " = " + deconstructedName);
                GUILayout.EndHorizontal();
            } else {
                GUILayout.BeginHorizontal();
                if (script.ID == 0) {
                    calculateID(script, EntityDefinitionConsts.ENTITIES);
                }
                GUILayout.Label("ID: " + script.ID + " = " + script.GetName());
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_editEntity ? "Close Editing" : "Edit")) {
                _editEntity = !_editEntity;

                if (_editEntity == true) {
                    loadChoices();
                }
            }
            if (GUILayout.Button("Reload Entities")) {
                loadChoices();
            }
            GUILayout.EndHorizontal();
        }

        private void calculateID(EntityId script, Dictionary<string, Dictionary<string, int>> entities) {

            if (script.Type != null && script.Type.Length > 0
                && script.Name != null && script.Name.Length > 0
                && script.Variation != null && script.Variation.Length > 0) {

                var variations = EntityDefinitionConsts.VARIATIONS;
                var variationNr = EntityDefinition.GetVariationNr(script.Variation, script.CustomVariation, ref variations);
                script.ID = EntityDefinition.GetId(
                    typeId: EntityDefinition.GetEntityTypeId(script.Type, ref entities),
                    nameId: EntityDefinitionConsts.ENTITIES[script.Type][script.Name],
                    variationNr
                );
            } else {
                loadChoices();

                script.Type = _typeChoices[0];
                var nameChoices = EntityDefinition.GetEntityNames(script.Type, EntityDefinitionConsts.ENTITIES);
                script.Name = nameChoices[0];
                script.Variation = _variationChoices[0];
            }
        }

        private void loadChoices() {
            _typeChoices = EntityDefinition.GetEntityTypes(EntityDefinitionConsts.ENTITIES);
            _variationChoices = EntityDefinition.GetEntityVariations(EntityDefinitionConsts.VARIATIONS);
        }
    }
}
#endif