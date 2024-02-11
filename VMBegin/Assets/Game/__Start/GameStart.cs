using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Start {
    public class GameStart : MonoBehaviour {

        public Action OnDependenciesLoaded;

        [SerializeField]
        private MainMenuController _mainMenuController;

        private string[] _scenesToLoad = new string[2] { "Main", "Player" };
        private int _loadedIndex = -1;
        private Action<string> _onSceneLoaded;
        private IEnumerator _loadScene;
        private IEnumerator _starGameScene;

        private void Start() {

            OnDependenciesLoaded += onDependenciesLoaded;
            _onSceneLoaded += onSceneLoaded;

            loadNext();
        }

        private void onDependenciesLoaded() {

            _onSceneLoaded -= onSceneLoaded;
            OnDependenciesLoaded -= onDependenciesLoaded;

            StopCoroutine(_loadScene);
            _mainMenuController.Init();
            Debug.Log("_mainMenuController.Init(); -> ");
            _mainMenuController.StartGame += () => {
                Debug.Log("StartCoroutine(_starGameScene); -> ");
                _starGameScene = startGameScene();
                StartCoroutine(_starGameScene);
            };
        }

        private void loadNext() {

            if (_loadedIndex == _scenesToLoad.Length - 1) {
                OnDependenciesLoaded?.Invoke();
                return;
            }

            _loadedIndex++;
            _loadScene = loadScene(_scenesToLoad[_loadedIndex]);
            StartCoroutine(_loadScene);
        }

        private void onSceneLoaded(string sceneName) {
            Debug.Log((_loadedIndex + 1) + ". " + sceneName + " has been loaded.");

            loadNext();
        }

        private IEnumerator loadScene(string sceneName) {
            var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncLoadLevel.isDone) {
                yield return null;
            }

            _onSceneLoaded?.Invoke(sceneName);
        }

        private IEnumerator startGameScene() {

            var loadSceneParameters = new LoadSceneParameters();
            loadSceneParameters.loadSceneMode = LoadSceneMode.Additive;
            loadSceneParameters.localPhysicsMode = LocalPhysicsMode.Physics3D;
            var scene = SceneManager.LoadScene(1, loadSceneParameters);

            while (!scene.isLoaded) {
                yield return null;
            }

            SceneManager.SetActiveScene(scene);

            SceneManager.UnloadSceneAsync(0);

            StopCoroutine(_starGameScene);
        }
    }
}