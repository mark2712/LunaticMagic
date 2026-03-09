using System;
using System.Collections.Generic;

namespace Story
{
    public class StoryState
    {
        public bool MainMenuEntitySpawn = false;
        public bool GameEntitySpawn = false;

        public void Load(StoryStateData data)
        {
            MainMenuEntitySpawn = data.MainMenuEntitySpawn;
            GameEntitySpawn = data.GameEntitySpawn;
        }

        public StoryStateData Save()
        {
            return new StoryStateData()
            {
                MainMenuEntitySpawn = MainMenuEntitySpawn,
                GameEntitySpawn = GameEntitySpawn,
            };
        }
    }

    [Serializable]
    public class StoryStateData
    {
        public bool MainMenuEntitySpawn = false;
        public bool GameEntitySpawn = false;
    }
}


// public List<IStoryScript> InitExecuteScripts = new();
// public void RunInitExecuteScripts()
// {
//     foreach (var script in InitExecuteScripts)
//     {
//         script.Execute();
//     }
// }