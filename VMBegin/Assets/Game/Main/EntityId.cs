using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Main {
    public class EntityId : MonoBehaviour, IEntityId {
        public string Type;
        public string Name;
        public string Variation;
        public int CustomVariation = 0;
        public int ID { get; set; }

        public string GetName() {
            string variationName = "";
            if (Variation == "Custom") {
                variationName = "." + CustomVariation.ToString("D2");
            } else if (Variation != "0" && Variation != "None") {
                variationName = "." + Variation;
            }
            return Type + "." + Name + variationName;
        }

        public string GetNameId() {
            return GetName() + " [" + ID + "]";
        }

        public int CalculateId(bool setName = true) {
            var entities = EntityDefinitionConsts.ENTITIES;
            var variations = EntityDefinitionConsts.VARIATIONS;
            ID = EntityDefinition.GetId(
                typeId: EntityDefinition.GetEntityTypeId(Type, ref entities),
                nameId: EntityDefinitionConsts.ENTITIES[Type][Name],
                variationNr: EntityDefinition.GetVariationNr(Variation, CustomVariation, ref variations)
            );

            if (setName) {
                var nameId = GetNameId();
                gameObject.name = nameId;
            }

            return ID;
        }
    }
}
