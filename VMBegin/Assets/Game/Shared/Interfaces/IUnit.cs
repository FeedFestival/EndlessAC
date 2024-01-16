using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnit {
        int ID { get; }
        string Name { get; }
        IActor Actor { get; }
        GameObject gameObject { get; }

        void Init(IWorldIndicatorManager worldIndicatorManager = null);
        void MovementInputChanged(Vector2 input, float cameraRotationY, bool analogMovement);
        void MovementTargetChanged(Vector3 pos);
        void Teleport(Vector3 position);
    }
}