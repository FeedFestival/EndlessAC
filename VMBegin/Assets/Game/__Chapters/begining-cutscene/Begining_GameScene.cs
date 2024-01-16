using Game.Actors;
using Game.Scene;
using Game.Shared.DataModels;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.Chapters.Begining {
    public class Begining_GameScene : GameScene {

        [Header("Begining_GameScene")]
        [SerializeField]
        private PlayableDirector _beginingCTimeline;

        // This is called from Player when everything is ready
        public override void StartScene() {
            var storyData = _player.GetStoryData() as StoryData;
            Debug.Log("storyData.progression.currentChapterId: " + storyData.progression.currentChapterKey);

            if (storyData.progression.currentChapterKey != "begining") {
                Debug.LogError("Current story progression should not happen in this scene. What are you doing here?");
                return;
            }

            var chapter_0 = storyData.chapters["begining"];

            if (storyData.progression.currentQuestKey == "the_stop") {

                _beginingCTimeline.Play();
            } else {
                // ...
            }

            var spaceshipActor = getSpaceship_Actor();
            spaceshipActor.StartEngine();
            spaceshipActor.StartNuclear();
            spaceshipActor.StartRotateLivingArea();
        }

        public void StopMainEngine() {
            var spaceshipActor = getSpaceship_Actor();

            spaceshipActor.StopEngine();
        }

        public void StartMainEngine() {
            var spaceshipActor = getSpaceship_Actor();

            spaceshipActor.StartEngine();
        }

        public void StopLivingAreaRotation() {
            var spaceshipActor = getSpaceship_Actor();

            spaceshipActor.StopRotateLivingAreaAtTurnRotation();
        }

        public void StartRotationEngine() {
            var spaceshipActor = getSpaceship_Actor();

            spaceshipActor.StartRotationEngine();
        }

        public void StopRotationEngine() {
            var spaceshipActor = getSpaceship_Actor();

            spaceshipActor.StopRotationEngine();
        }

        public void BeginingCTimelineStopped() {
            Debug.Log("Timeline Stopped");
        }











        private Spaceship_Actor getSpaceship_Actor() {
            var spaceship = _unitManager.Units["Actor.SpaceShip [201]"];
            return spaceship.gameObject.GetComponent<Spaceship_Actor>();
        }
    }
}
