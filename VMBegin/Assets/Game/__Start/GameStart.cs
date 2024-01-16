using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Start {
    public class GameStart : MonoBehaviour {

        [SerializeField]
        private MainMenuController _mainMenuController;

        private void Start() {

            _mainMenuController.Init();
        }
    }
}