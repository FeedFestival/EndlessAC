using Game.Shared.Bus;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Player {
    public class Player : MonoBehaviour, IPlayer {

        [SerializeField]
        public CameraController _cameraControllerRef;
        PlayerControl _playerControl;

        [Header("Unit")]
        [SerializeField]
        private GameObject _unitGo;
        private IUnit _unitRef;
        [SerializeField]
        private WorldIndicatorManager _worldIndicatorManager;

        [Header("System")]
        [SerializeField]
        private GameObject _gameStoryRef;
        private IGameStory _gameStory;

        void Awake() {

            _unitRef = _unitGo.GetComponent<IUnit>();
            _unitGo = null;

            _playerControl = new PlayerControl(_unitRef, _cameraControllerRef, _worldIndicatorManager);

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

        }

        // CONTROL


        // STORY

        public IStoryData GetStoryData() => _gameStory.GetStoryData();


        void OnDisable() {
            //_input.Disable();
            //_input.Player.Movement.performed -= onMovementPerformed;
        }
    }
}