using System.Collections.Generic;

namespace Game.Main {
    public static class EntityDefinitionConsts {
        public static readonly Dictionary<string, Dictionary<string, int>> ENTITIES;
        public static readonly Dictionary<string, int> VARIATIONS;

        static EntityDefinitionConsts() {
            ENTITIES = new Dictionary<string, Dictionary<string, int>>() {
                {
                    "Interactable",
                    new Dictionary<string, int>() { 
						{ "Door", 1 },
						{ "Ladder", 2 },

                    }
                },
                {
                    "Actor",
                    new Dictionary<string, int>() { 
						{ "SpaceShip", 1 },
						{ "Alien", 7 },

                    }
                },
            };
            VARIATIONS = new Dictionary<string, int>() {
				{ "None", 0 },

            };

        }
    }
}