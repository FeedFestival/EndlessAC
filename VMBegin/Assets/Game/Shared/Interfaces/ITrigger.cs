using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface ITrigger {
        int ID { get; }
        Transform transform { get; }
        void Init(int entityId);
        void Init(int entityId, Action<int> entityTouched, Action<int> entityLeft, Action<IUnit> instantInteract = null);
        void Enable();
        void Disable();
        void DestroyForever();
        int EntityTouched(int entityId);
        void EntityLeft(int entityId);
    }
}
