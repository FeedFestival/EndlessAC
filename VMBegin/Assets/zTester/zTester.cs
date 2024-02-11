using Game.Start;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System;
using Game.Chapters.Begining;

public class zTester : MonoBehaviour {

    private IEnumerator _loadStartGameScene;

    private Subject<bool> _mainMenuTest__s = new Subject<bool>();

    private void Awake() {
        _loadStartGameScene = loadStarGameScene();
        StartCoroutine(_loadStartGameScene);

        SceneManager.activeSceneChanged += activeSceneChanged;

        _mainMenuTest__s
            .Delay(TimeSpan.FromSeconds(1))
            .Do(_ => {
                Debug.Log("mainMenuControllerRef -> ");
                var mainMenuControllerRef = (MainMenuController)FindObjectOfType(typeof(MainMenuController));
                mainMenuControllerRef.StartGame?.Invoke();
            })
            .Delay(TimeSpan.FromSeconds(1))
            .Subscribe();
    }

    private void activeSceneChanged(Scene previous, Scene current) {

        var combine = previous.name + "->" + current.name;
        Debug.Log("activeSceneChanged: " + combine);

        GameObject go;

        switch (combine) {
            case "zTester->start":

                var gameStartRef = (GameStart)FindObjectOfType(typeof(GameStart));
                gameStartRef.OnDependenciesLoaded += () => _mainMenuTest__s.OnNext(true);

                break;

            case "start->begining-cutscene":

                go = new GameObject("begining-cutscene Tester");
                var begining_GameScene_Test = go.AddComponent<Begining_GameScene_Test>();
                var begining_GameSceneRef = (Begining_GameScene)FindObjectOfType(typeof(Begining_GameScene));
                begining_GameScene_Test.Init(begining_GameSceneRef);

                break;

            case "begining-cutscene->begining-tutorial":

                go = new GameObject("begining-tutorial Tester");
                var gameScene_BeginingTutorial_Test = go.AddComponent<GameScene_BeginingTutorial_Test>();
                var gameScene_BeginingTutorialRef = (GameScene_BeginingTutorial)FindObjectOfType(typeof(GameScene_BeginingTutorial));
                gameScene_BeginingTutorial_Test.Init(gameScene_BeginingTutorialRef);

                break;

            default:
                break;
        }
    }

    private void onGameStarted(Scene scene) {
        SceneManager.SetActiveScene(scene);
        StopCoroutine(_loadStartGameScene);
    }

    private IEnumerator loadStarGameScene() {

        var loadSceneParameters = new LoadSceneParameters();
        loadSceneParameters.loadSceneMode = LoadSceneMode.Additive;
        loadSceneParameters.localPhysicsMode = LocalPhysicsMode.Physics3D;
        var scene = SceneManager.LoadScene(0, loadSceneParameters);

        while (!scene.isLoaded) {
            yield return null;
        }

        onGameStarted(scene);
    }
}
