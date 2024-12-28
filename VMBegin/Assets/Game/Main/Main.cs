using Game.Shared.Bus;
using UnityEngine;

namespace Game.Main {
    public class Main : MonoBehaviour {
        public delegate void GameQuit();
        public GameQuit OnGameQuit;

        public BasePrefabsSO BasePrefabs;

        public static Main _ { get; private set; }
        private void Awake() {
            // If there is an instance, and it's not me, delete myself.
            if (Main._ != null && Main._ != this) {
                Destroy(this);
                return;
            }

            _ = this;

            DontDestroyOnLoad(this);
        }

        private void OnApplicationQuit() {
            Debug.Log("ApplicationQuit - Reseting Static Properties");
            __.ClearAll();
        }
    }
}
