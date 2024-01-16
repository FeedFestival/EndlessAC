using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Start {
    public class MainMenuController : MonoBehaviour {

        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Button _optionsButton;

        [SerializeField]
        private Button _quitButton;

        internal void Init() {
            _startButton.onClick.AddListener(() => {
                SceneManager.LoadScene(1);
            });

            _optionsButton.onClick.AddListener(() => {
                Debug.Log("Go To Options -> ");
            });

            _quitButton.onClick.AddListener(() => {
                Application.Quit();
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#endif
            });
        }
    }
}