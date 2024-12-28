using Game.Shared.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using _ = Game.Main;

namespace Game.Player {
    public class PlayerControl {

        InputManager _input;
        IUnit _unitRef;
        CameraController _cameraControllerRef;

        public PlayerControl(IUnit unitRef, CameraController cameraControllerRef, IWorldIndicatorManager worldIndicatorManager) {
            _unitRef = unitRef;
            _cameraControllerRef = cameraControllerRef;

            _unitRef.Init(_.Main._.BasePrefabs, worldIndicatorManager);
            _cameraControllerRef.Init(emitMovementTarget);

            _input = new InputManager();

            _input.Enable();
            _input.Player.Mouse.performed += onMouseClick;

            _input.Player.LookAt.performed += onLookAtPerformed;
            _input.Player.LookAt.canceled += stopLookAt;

            _input.Player.Movement.performed += onMovementPerformed;
            _input.Player.Movement.canceled += onMovementCanceled;
        }

        void onMouseClick(InputAction.CallbackContext context) {
            var isLeftClick = InputService.IsControlLeftClick(context.control.shortDisplayName);
            if (isLeftClick) {
                _cameraControllerRef.LeftClick();
            } else {
                _cameraControllerRef.RightClick();
            }
        }

        void onLookAtPerformed(InputAction.CallbackContext context) {
            var isAnalogue = InputService.IsControlRightAnalogue(context.control.shortDisplayName);
            if (!isAnalogue) { return; }

            var position = context.ReadValue<Vector2>();
            _cameraControllerRef.RightAnalogMoved(position);
        }

        void stopLookAt(InputAction.CallbackContext _) => _cameraControllerRef.stopLookAt();

        void emitMovementTarget(Vector3 pos) {
            _unitRef.MovementTargetChanged(pos);
        }
        void onMovementPerformed(InputAction.CallbackContext context) {
            var isAnalogue = InputService.IsControlLeftAnalogue(context.control.shortDisplayName);
            _unitRef.MovementInputChanged(context.ReadValue<Vector2>(), _cameraControllerRef.transform.eulerAngles.y, isAnalogue);
        }

        void onMovementCanceled(InputAction.CallbackContext context) {
            var isAnalogue = InputService.IsControlLeftAnalogue(context.control.shortDisplayName);
            _unitRef.MovementInputChanged(context.ReadValue<Vector2>(), _cameraControllerRef.transform.eulerAngles.y, isAnalogue);
        }


    }
}
