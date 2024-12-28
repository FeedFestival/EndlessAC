using UnityEngine;
using Game.Shared.Interfaces;

namespace Game.Main {
    [CreateAssetMenu(fileName = "BasePrefabs", menuName = "ScriptableObjects/BasePrefabs", order = 1)]
    public class BasePrefabsSO : ScriptableObject, IBasePrefabs {

        [SerializeField]
        private GameObject _movementTargetSignal;
        public GameObject MovementTargetSignal => _movementTargetSignal;
    }
}
