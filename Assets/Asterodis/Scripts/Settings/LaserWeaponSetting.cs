using System;

namespace Asterodis.Settings
{
    [Serializable]
    public class LaserWeaponSetting : WeaponSetting
    {
        public int MaxAmmo;
        public float CountDown;
    }
}