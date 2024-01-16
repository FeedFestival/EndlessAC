using UnityEngine;

namespace Game.Main {
    [CreateAssetMenu(fileName = "BasePrefabs", menuName = "ScriptableObjects/BasePrefabs", order = 1)]
    public class BasePrefabsSO : ScriptableObject {

        public GameObject MovementTargetSignal;
    }
}
