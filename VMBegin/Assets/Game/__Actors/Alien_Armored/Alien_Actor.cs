using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Actors {
    public class Alien_Actor : MonoBehaviour, IActor {

        public Animator Animator { get => _animatorRef; }

        public GameObject go => gameObject;

        [SerializeField]
        private Animator _animatorRef;

        public void Init() {


            SetActive(false);
        }

        public void SetActive(bool active) => gameObject.SetActive(active);
    }
}