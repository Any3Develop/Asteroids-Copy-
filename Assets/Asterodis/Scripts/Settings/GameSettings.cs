using System;
using System.Collections.Generic;

namespace Asterodis.Settings
{
    [Serializable]
    public class GameSettings
    {
        public bool FriendlyFire;
        public float ScoreMultiplier;
        public float DelayLevelTimer;
        public float SceneDepth;
        public List<Score> Scores;
    }

    [Serializable]
    public struct Score
    {
        public string Tag;
        public int Quantity;
    }
}