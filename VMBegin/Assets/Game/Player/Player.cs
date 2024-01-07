using Game.Shared.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using GameScrypt.GSUtils;
using Game.Shared.Bus;
using System.Collections;
using DG.Tweening;

namespace Game.Player {
    public class Player : MonoBehaviour, IPlayer {

        [SerializeField]
        CameraController _cameraControllerRef;
        InputManager _input;

        [Header("Unit")]
        [SerializeField]
        GameObject _unitGo;
        IUnit _unitRef;
        [SerializeField]
        GameObject _movementTargetTriggerGo;
        ITrigger _movementTargetTrigger;

        [Header("System")]
        [SerializeField]
        private GameObject _gameStoryRef;
        private IGameStory _gameStory;
        private Vector2 _rightAnalogueScreenPosition;
        Vector2Int _centerScreen;

        void Awake() {
            _input = new InputManager();

            _cameraControllerRef.Init(emitMovementTarget);

            _unitRef = _unitGo.GetComponent<IUnit>();
            _unitGo = null;

            _movementTargetTrigger = _movementTargetTriggerGo.GetComponent<ITrigger>();
            _movementTargetTriggerGo = null;

            _centerScreen = new Vector2Int(Screen.width / 2, Screen.height / 2);

            if (_gameStoryRef) {
                _gameStory = _gameStoryRef.GetComponent<IGameStory>();
                _gameStoryRef = null;
            } else {
                Debug.LogWarning("There is no GameStory on Player");
            }

            __.GameBus.On(GameEvt.GAME_SCENE_LOADED, (object obj) => {
                var gameScene = obj as IGameScene;
                Debug.Log("__.GameBus.On -> GameEvt.GAME_SCENE_LOADED");
                gameScene.SetPlayer(this);
                gameScene.StartScene();
            });
        }

        void Start() {
            _unitRef.Init(_movementTargetTrigger);
        }

        void OnEnable() {
            _input.Enable();
            _input.Player.Mouse.performed += onMouseClick;

            _input.Player.LookAt.performed += onLookAtPerformed;
            _input.Player.LookAt.canceled += stopLookAt;

            _input.Player.Movement.performed += onMovementPerformed;
            _input.Player.Movement.canceled += onMovementCanceled;
        }


        // CONTROL

        void onMouseClick(InputAction.CallbackContext context) {
            Debug.Log("context.control.shortDisplayName: " + context.control.shortDisplayName);
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
            _rightAnalogueScreenPosition = position;
            Debug.Log("_rightAnalogueScreenPosition: " + _rightAnalogueScreenPosition);

            _cameraControllerRef.RightAnalogMoved(_rightAnalogueScreenPosition);
        }

        void stopLookAt(InputAction.CallbackContext context) {

            if (_rightAnalogueScreenPosition.x > 0.1f && _rightAnalogueScreenPosition.y > 0.1f) {
                DOVirtual
                    .Vector3(_rightAnalogueScreenPosition, new Vector3(_centerScreen.x, _centerScreen.y), 0.33f, (value) => {
                        _cameraControllerRef.RightAnalogMoved(value);
                    })
                    .SetEase(Ease.InOutCirc);
            }
        }

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

        // STORY

        public IStoryData GetStoryData() => _gameStory.GetStoryData();


        void OnDisable() {
            _input.Disable();
            _input.Player.Movement.performed -= onMovementPerformed;
        }
    }
}