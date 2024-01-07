using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Actors {
    public class Alien_Actor : MonoBehaviour, IActor {

        public Animator Animator { get; private set; }

        void Awake() {
            Animator = GetComponent<Animator>();


        }
    }
}