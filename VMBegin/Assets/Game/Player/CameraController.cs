using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player {
    public class CameraController : MonoBehaviour {

        Vector2 _mousePos;
        [SerializeField]
        LayerMask _mask = -1;
        [SerializeField]
        RectTransform _crosshairRt;
        Action<Vector3> _emitMovementTarget;

        internal void Init(Action<Vector3> emitMovementTarget) {
            _emitMovementTarget = emitMovementTarget;            
        }

        private void Update() {
            //_mousePos = Mouse.current.position.ReadValue();
            //_crosshairRt.anchoredPosition = _mousePos;
        }

        internal void LeftClick() {
        }

        internal void RightClick() {

            checkFloor();
        }

        void checkFloor() {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_mousePos);
            if (Physics.Raycast(ray, out hit, 100, _mask)) {
                Debug.Log("hit.point: " + hit.point);
                _emitMovementTarget?.Invoke(hit.point);
            }
        }

        internal void RightAnalogMoved(Vector2 vector2) {
            _mousePos = vector2;
            _crosshairRt.anchoredPosition = _mousePos;
        }
    }
}