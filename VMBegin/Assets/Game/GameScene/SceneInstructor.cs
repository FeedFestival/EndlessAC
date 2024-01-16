using UnityEngine;

namespace Game.Scene {
    public class SceneInstructor : MonoBehaviour, ISceneInstructor {

        public void StartInstructions() {
            Debug.Log("StartInstructions -> ");
        }
    }
}
