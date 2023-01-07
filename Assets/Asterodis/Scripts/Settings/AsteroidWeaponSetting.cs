using System;

namespace Asterodis.Settings
{
    [Serializable]
    public class AsteroidWeaponSetting : WeaponSetting
    {
        public float ShardsSpeedMultiplier;
        public int ShardsSteps;
        public int DevideFactor;
    }
}