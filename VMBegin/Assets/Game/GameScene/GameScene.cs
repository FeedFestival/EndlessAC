using Game.Shared.Bus;
using Game.Shared.Interfaces;
using UnityEngine;
using _ = Game.Main;

namespace Game.Scene {
    public class GameScene : MonoBehaviour, IGameScene {
        [SerializeField]
        private GameObject _unitManagerRef;
        protected IUnitManager _unitManager;
        protected IPlayer _player;

        private void Awake() {

            if (_.Main._ == null) {
                var go = Resources.Load<GameObject>("main");
                Instantiate(go);
            }

            if (_unitManagerRef) {
                _unitManager = _unitManagerRef?.GetComponent<IUnitManager>();
            }
        }

        protected virtual void Start() {
            Debug.Log("GameScene  -> Start -> GameEvt.GAME_SCENE_LOADED");

            _unitManager?.Init(_.Main._.BasePrefabs);

            __.GameBus.Emit(GameEvt.GAME_SCENE_LOADED, this);
        }

        public void SetPlayer(IPlayer player) {
            _player = player;
        }

        public virtual void StartScene() {

        }
    }
}
