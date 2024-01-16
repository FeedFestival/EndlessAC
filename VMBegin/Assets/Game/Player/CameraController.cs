using DG.Tweening;
using GameScrypt.GSUtils;
using System;
using UnityEngine;

namespace Game.Player {
    public class CameraController : MonoBehaviour {

        Vector2 _mousePos;
        [SerializeField]
        LayerMask _mask = -1;
        [SerializeField]
        RectTransform _crosshairRt;
        Action<Vector3> _emitMovementTarget;
        Vector2Int _centerScreen;

        internal void Init(Action<Vector3> emitMovementTarget) {
            _centerScreen = new Vector2Int(Screen.width / 2, Screen.height / 2);

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

        internal void RightAnalogMoved(Vector2 position) {
            _mousePos = position;

            position = new Vector2(
                x: GSPercent.Find((position.x + 1) * 100, _centerScreen.x),
                y: GSPercent.Find((position.y + 1) * 100, _centerScreen.y)
            );

            if (position.x < 0) {
                position.x = 0;
            }
            if (position.x > Screen.width) {
                position.x = Screen.width;
            }
            if (position.y < 0) {
                position.y = 0;
            }
            if (position.y > Screen.height) {
                position.y = Screen.height;
            }
            Debug.Log("_mousePos: " + _mousePos);

            _crosshairRt.anchoredPosition = _mousePos;
        }

        internal void stopLookAt() {

            if (_mousePos.x > 0.1f && _mousePos.y > 0.1f) {
                DOVirtual
                    .Vector3(_mousePos, new Vector3(_centerScreen.x, _centerScreen.y), 0.33f, (value) => {
                        RightAnalogMoved(value);
                    })
                    .SetEase(Ease.InOutCirc);
            }
        }

        void checkFloor() {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_mousePos);
            if (Physics.Raycast(ray, out hit, 100, _mask)) {
                Debug.Log("hit.point: " + hit.point);
                _emitMovementTarget?.Invoke(hit.point);
            }
        }
    }
}