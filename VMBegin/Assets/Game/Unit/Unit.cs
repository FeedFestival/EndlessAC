using Game.Shared.Interfaces;
using UnityEngine;
using _ = Game.Main;

namespace Game.Unit {
    public class Unit : UnitBase, IUnit {

        public int ID { get; private set; }
        public string Name { get => gameObject.name; }

        [SerializeField]
        private SignalScannerManager _signalScannerManager;
        [SerializeField]
        private GameObject _actorRef;

        private Motor _motorRef;

        public IActor Actor { get; private set; }

        public void Init(IWorldIndicatorManager worldIndicatorManager = null) {

            var entityId = gameObject.GetComponent<IEntityId>();
            ID = entityId.CalculateId();

            var movementTargetTrigger = createMovementTarget(worldIndicatorManager);

            Actor = _actorRef.GetComponent<IActor>();
            Actor.Init();

            _motorRef = gameObject.GetComponent<Motor>();
            _motorRef?.Init(Actor, movementTargetTrigger);

            movementTargetTrigger?.Init(ID);

            _signalScannerManager?.Init(ID, onMovementTargetHit);
        }

        public void MovementInputChanged(Vector2 input, float cameraRotationY, bool analogMovement) {
            Debug.Log("input: " + input);

            _motorRef.MovementInputChanged(input, cameraRotationY, analogMovement);
            _signalScannerManager.DisableMovementScanner();
        }

        public void MovementTargetChanged(Vector3 targetPos) {
            _motorRef.MovementTargetChanged(targetPos);
            _signalScannerManager.EnableMovementScanner();
        }

        public void Teleport(Vector3 position) {
            _motorRef.Teleport(position, onNavMesh: true);
        }

        void onMovementTargetHit() {
            _motorRef.MoveTargetReached();
        }

        ITrigger createMovementTarget(IWorldIndicatorManager worldIndicatorManager = null) {
            var go = Instantiate(_.Main._.BasePrefabs.MovementTargetSignal);

            if (worldIndicatorManager != null) {
                go.transform.SetParent(worldIndicatorManager.transform);
            }

            return go.GetComponent<ITrigger>();
        }
    }
}
