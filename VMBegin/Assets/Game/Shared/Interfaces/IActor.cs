using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IActor {
        Animator Animator { get; }

        GameObject go { get;}

        void Init();

        void SetActive(bool active);
    }
}
