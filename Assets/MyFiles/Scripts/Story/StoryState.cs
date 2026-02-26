using System;
using System.Collections.Generic;

namespace Story
{
    public class StoryState
    {
        public List<IStoryScript> InitExecuteScripts = new();

        public bool MainMenuOpen = false;
        public bool IsTimerRunning = false;

        public void RunInitExecuteScripts()
        {
            foreach (var script in InitExecuteScripts)
            {
                script.Execute();
            }
        }

        public void Load(StoryStateData data)
        {
            MainMenuOpen = data.MainMenuOpen;
            IsTimerRunning = data.IsTimerRunning;
        }

        public StoryStateData Save()
        {
            return new StoryStateData()
            {
                MainMenuOpen = MainMenuOpen,
                IsTimerRunning = IsTimerRunning,
            };
        }
    }

    [Serializable]
    public class StoryStateData
    {
        public bool MainMenuOpen = false;
        public bool IsTimerRunning = false;
    }
}