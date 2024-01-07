using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Shared.Bus
{
    public struct InteractPayload : IInteractPayload
    {
        public string unitName { get; set; }
        public string interactableName { get; set; }
        public Transform startTransform { get; set; }
        public Transform endTransform { get; set; }
    }
}
