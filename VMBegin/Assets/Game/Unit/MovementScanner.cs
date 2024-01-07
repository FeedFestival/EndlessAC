using Game.Shared.Interfaces;
using System;
using UniRx;
using UnityEngine;

namespace Game.Unit {
    public class MovementScanner : MonoBehaviour {

        [SerializeField]
        LayerMask _mask = 8;
        private int _entityId;
        private IDisposable _checkWithRay;
        private Action _emitOnHit;

        // shoot ray and emit when you hit your own move target
        internal void Init(int entityId, Action emitOnHit) {
            _entityId = entityId;
            _emitOnHit = emitOnHit;
        }

        void OnEnable() {
            Debug.Log("OnEnable: ");
            _checkWithRay = Observable
                .Interval(TimeSpan.FromMilliseconds(150))
                .Do(_ => Debug.Log("Checking..."))
                .Subscribe(_ => GetHitWithRay(
                    transform.position,
                    transform.TransformDirection(Vector3.down),
                    distance: 1,
                    _mask,
                    onHit,
                    onMiss)
                );
        }

        void OnDisable() {
            _checkWithRay?.Dispose();
        }

        void onHit(RaycastHit hit) {
            var trigger = hit.collider.GetComponent<ITrigger>();
            Debug.Log("hit: " + hit.transform.name);
            Debug.Log("trigger.ID: " + trigger.ID);
            Debug.Log("_entityId: " + _entityId);
            if (trigger.ID == _entityId) {
                Debug.Log("Reached destination");
                _emitOnHit?.Invoke();

                gameObject.SetActive(false);
            }
        }

        void onMiss() {
            //    _trigger?.EntityLeft(_entityId);
            //    _trigger = null;
            //    _interactableId = null;
        }

        private static void GetHitWithRay(Vector3 position, Vector3 dir, int distance, LayerMask mask, Action<RaycastHit> onHit, Action onMiss) {
            RaycastHit hit;
            if (Physics.Raycast(position, dir, out hit, distance, mask)) {
                onHit?.Invoke(hit);
            } else {
                onMiss?.Invoke();
            }
        }

        void OnDestroy() {
            _checkWithRay?.Dispose();
            //_trigger = null;
        }
    }
}