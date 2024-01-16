using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IInteractPayload {
        string unitName { get; set; }
        string interactableName { get; set; }
        public Transform startTransform { get; set; }
        public Transform endTransform { get; set; }
    }
}
