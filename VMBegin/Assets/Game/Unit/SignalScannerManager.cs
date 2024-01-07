using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {
    public class SignalScannerManager : MonoBehaviour {
        //[SerializeField]
        //private EntitySignal _entitySignal;
        [SerializeField]
        private MovementScanner _movementScanner;
        //[SerializeField]
        //private WallCheckSignal _wallCheckSignal;

        internal void Init(int entityId, Action onMovementTargetHit) {

            //_unitSignal?.Init(entityId);
            _movementScanner?.Init(entityId, onMovementTargetHit);
            //_wallCheckSignal?.Init(entityId);
        }

        internal void DisableMovementScanner() {
            _movementScanner.gameObject.SetActive(false);
        }

        internal void EnableMovementScanner() {
            _movementScanner.gameObject.SetActive(true);
        }

        //public void EmitInteract(IUnit unit) => _unitSignal?.EmitInteract(unit);

        //public void SubscribeToOnTriggerTouch(Action<IInteractInfo> action)
        //    => _unitSignal.OnTriggerTouch += action;

        //public void SubscribeToOnTriggerLeft(Action action)
        //    => _unitSignal.OnTriggerLeft += action;

        //public void SubscribeToOnWallCheckShow(Action<int> action)
        //    => _wallCheckSignal.OnWallCheckShow += action;

        //public void SubscribeToOnWallCheckHide(Action<int[]> action)
        //    => _wallCheckSignal.OnWallCheckHide += action;
    }
}