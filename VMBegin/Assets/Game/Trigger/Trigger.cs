using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.Trigger {
    public class Trigger : MonoBehaviour, ITrigger {
        public int ID { get; set; }

        private Action<int> _entityTouched;
        private Action<int> _entityLeft;
        private Action<IUnit> _instantInteract;

        public void Init(int entityId) {
            ID = entityId;
        }

        public void Init(int entityId, Action<int> entityTouched, Action<int> entityLeft, Action<IUnit> instantInteract = null) {
            ID = entityId;
            _instantInteract = instantInteract;
            _entityTouched = entityTouched;
            _entityLeft = entityLeft;
        }

        public void Enable() {
            GetComponent<BoxCollider>().enabled = true;
        }

        public void Disable() {
            GetComponent<BoxCollider>().enabled = false;
        }

        public void DestroyForever() {
            Destroy(gameObject);
        }

        public int EntityTouched(int entityId) {
            Debug.Log("entityId: " + entityId.ToString());
            _entityTouched?.Invoke(entityId);
            return ID;
        }

        public void EntityLeft(int entityId) {
            _entityLeft?.Invoke(entityId);
        }

        public void InstantInteract(IUnit unit) {
            Debug.Log("InstantInteract -> unit: " + unit);
            _instantInteract?.Invoke(unit);
        }
    }
}