using System;
using UniRx;
using UnityEngine;

public class GameScene_BeginingTutorial_Test : MonoBehaviour {

    private Subject<bool> _test__s = new Subject<bool>();

    private GameScene_BeginingTutorial _gameScene_BeginingTutorialRef;

    internal void Init(GameScene_BeginingTutorial gameScene_BeginingTutorialRef) {

        _gameScene_BeginingTutorialRef = gameScene_BeginingTutorialRef;

        _test__s
            .Delay(TimeSpan.FromSeconds(1))
            .Do(_ => {
                _gameScene_BeginingTutorialRef.ToggleLightswtich();
            })
            .Delay(TimeSpan.FromSeconds(1))
            .Do(_ => {
                _gameScene_BeginingTutorialRef.ToggleLightswtich();
            })
            .Delay(TimeSpan.FromSeconds(1))
            .Do(_ => {
                _gameScene_BeginingTutorialRef.ToggleLightswtich();
            })
            .Delay(TimeSpan.FromSeconds(1))
            .Do(_ => {
                Debug.Log("Game Done !!");
            })
            .Subscribe();

        _test__s.OnNext(true);
    }
}
