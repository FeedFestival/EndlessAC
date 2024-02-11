using Game.Shared.Bus;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Scene {
    public class GameScene : MonoBehaviour, IGameScene {
        [SerializeField]
        private GameObject _unitManagerRef;
        protected IUnitManager _unitManager;
        protected IPlayer _player;

        private void Awake() {
            _unitManager = _unitManagerRef.GetComponent<IUnitManager>();
        }

        protected virtual void Start() {
            Debug.Log("GameScene  -> Start -> GameEvt.GAME_SCENE_LOADED");

            _unitManager.Init();

            __.GameBus.Emit(GameEvt.GAME_SCENE_LOADED, this);
        }

        public void SetPlayer(IPlayer player) {
            Debug.Log("SetPlayer");
            _player = player;
        }

        public virtual void StartScene() {

        }
    }
}
