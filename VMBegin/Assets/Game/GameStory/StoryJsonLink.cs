using Game.Shared.DataModels;
using GameScrypt.JsonData;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Story {
    public class StoryJsonLink : GSJsonData {
        public static readonly StoryData DEFAULT_STORY = new StoryData() {
            decisions = new StoryDecisions() {
                chapters = new Dictionary<string, Dictionary<string, string>>() {
                    {
                        "begining", new Dictionary<string, string>() {
                            { "played_begining_cutscene", "false" }
                        }
                    }
                }
            },
            progression = new StoryProgression() {
                currentChapterKey = "begining",
                currentQuestKey = "the_stop"
            },
            chapters = new Dictionary<string, StoryChapter>() {
                {
                    "begining", new StoryChapter() {
                        id = 0,
                        name = "Finding Reason",
                        synopsis = "You have landed in the earth solar system and are just awaken from stasis and need to find the reason for you being there.",
                        quests = new Dictionary<string, StoryQuest>() {
                            {
                                "the_stop", new StoryQuest() {
                                    id = 0,
                                    name = "The Stop",
                                    objective = "Sit back and enjoy the cutscene.",
                                    events = new Dictionary<string, string>() {
                                        { "played_begining_cutscene", "false" }
                                    }
                                }
                            },
                            {
                                "reach_infirmary", new StoryQuest() {
                                    id = 1,
                                    name = "Reach the infirmary",
                                    objective = "Make your way through the ship towards the infirmary.",
                                    events = new Dictionary<string, string>() {
                                        { "reached_the_elevator", "false" },
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        public StoryJsonLink(string path) : base(path) {
        }

        public override void Recreate(string path = null, object obj = null) {
            GSJsonService.WriteJsonToFile(base._path, DEFAULT_STORY);
        }

        //public override JSONNode GetJson() => base.GetJson();

        //public void UpdatePlayer(PlayerSettings PlayerSettings)
        //{
        //    var playerJsonObj = base.GetJsonObj<DeviceJson>();
        //    playerJsonObj.playerSettings = PlayerSettings;
        //    GSJsonService.WriteJsonToFile(base._path, playerJsonObj);
        //}

        //public void UpdateGameSettings(GameSettings gameSettings)
        //{
        //    var playerJsonObj = base.GetJsonObj<DeviceJson>();
        //    playerJsonObj.gameSettings = gameSettings;
        //    GSJsonService.WriteJsonToFile(base._path, playerJsonObj);
        //}

        public StoryData GetStoryData() {
            var playerJsonObj = base.GetJsonObj<StoryData>();
            if (playerJsonObj == null) {
                Debug.LogWarning("No PlayerJson");
                Recreate();
                playerJsonObj = base.GetJsonObj<StoryData>();
            }
            return playerJsonObj;
        }

        //public GameSettings GetGameSettings()
        //{
        //    var playerJsonObj = base.GetJsonObj<DeviceJson>();
        //    if (playerJsonObj == null)
        //    {
        //        Debug.LogWarning("No PlayerJson");
        //        Recreate();
        //        playerJsonObj = base.GetJsonObj<DeviceJson>();
        //    }
        //    return playerJsonObj.gameSettings;
        //}
    }
}