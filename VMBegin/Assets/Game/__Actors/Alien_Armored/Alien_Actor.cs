using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Actors {
    public class Alien_Actor : MonoBehaviour, IActor {

        public Animator Animator { get => _animatorRef; }

        [SerializeField]
        private Animator _animatorRef;

        void Awake() {

        }
    }
}