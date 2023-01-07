using System;

namespace Asterodis.Settings
{
    [Serializable]
    public class PlayerAiSetting
    {
        public bool AIEnvironmentCollision;
        public int Health;
        public int VariationsCount;
        public int PerLevelAppearances;
        public int SpawnPointCount;
        public float AimRotationSpeed;
    }
}