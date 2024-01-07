using Game.Shared.Interfaces;
using System.Collections.Generic;

namespace Game.Shared.DataModels {

    public class StoryData : IStoryData {
        public StoryDecisions decisions;
        public StoryProgression progression;
        public Dictionary<string, StoryChapter> chapters;
    }

    public class StoryDecisions {
        public Dictionary<string, Dictionary<string, string>> chapters;
    }

    public class StoryProgression {
        public string currentChapterKey;
        public string currentQuestKey;
    }

    public class StoryChapter {
        public int id;
        public string name;
        public string synopsis;
        public Dictionary<string, StoryQuest> quests;
    }

    public class StoryQuest {
        public int id;
        public string name;
        public string synopsis;
        public string objective;
        public Dictionary<string, string> events;
    }
}
