using Game.Shared.Bus;
using Game.Shared.Enums;
using GameScrypt.GSDictionary;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Main {
    public class Main : MonoBehaviour {
        public delegate void GameQuit();
        public GameQuit OnGameQuit;

        //[System.Serializable]
        //public struct DependencyPrefab
        //{
        //    public Dependency dependency;
        //    public GameObject gameObject;
        //}

        //public List<DependencyPrefab> DependencyPrefabs;
        //private Dictionary<Dependency, GameObject> _dependencyPrefabs;

        public UDictionary<Dependency, GameObject> _dependencyPrefabs;

        public static Main _ { get; private set; }
        private void Awake() {
            // If there is an instance, and it's not me, delete myself.
            if (Main._ != null && Main._ != this) {
                Destroy(this);
                return;
            }

            _ = this;

            DontDestroyOnLoad(this);

            //_dependencyPrefabs = new Dictionary<Dependency, GameObject>();
            //foreach (var dp in DependencyPrefabs)
            //{
            //    _dependencyPrefabs.Add(dp.dependency, dp.gameObject);
            //}
            //DependencyPrefabs = null;
        }

        public string GetNameFromId(int entityId) {
            return EntityDefinition.GetNameFromId(entityId, EntityDefinitionConsts.ENTITIES, EntityDefinitionConsts.VARIATIONS);
        }

        internal string[] GetEntityTypes() {
            return EntityDefinition.GetEntityTypes(EntityDefinitionConsts.ENTITIES);
        }

        public void CreateDependency(Dependency dependency) {
            Instantiate(_dependencyPrefabs[dependency]);
        }

        private void OnApplicationQuit() {
            Debug.Log("ApplicationQuit - Reseting Static Properties");
            __.ClearAll();
            OnGameQuit?.Invoke();
        }
    }
}
