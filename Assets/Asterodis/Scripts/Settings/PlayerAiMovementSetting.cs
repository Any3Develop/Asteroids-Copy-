using System;

namespace Asterodis.Settings
{
    [Serializable]
    public class PlayerAiMovementSetting
    {
        public int ManeuverCountMin;
        public int ManeuverCountMax;
        public float ReachDistance;
        public PlayerAiMovementVariation[] Variations;
    }
    
    [Serializable]
    public class PlayerAiMovementVariation
    {
        public float LinearSpeed;
    }
}