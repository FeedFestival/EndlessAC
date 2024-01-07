using Game.Shared.DataModels;
using Game.Shared.Interfaces;
using GameScrypt.JsonData;
using UnityEngine;

namespace Game.Story {
    public class GameStory : MonoBehaviour, IGameStory {

        private StoryJsonLink _storyJsonLink;

        private void Awake() {
            _storyJsonLink = new StoryJsonLink("story.json");
        }

        public IStoryData GetStoryData() {
            Debug.Log("GetStoryData");
            return _storyJsonLink.GetStoryData();
        }   
    }
}