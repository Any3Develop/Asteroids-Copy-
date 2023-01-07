using System;

namespace Asterodis.Settings
{
    [Serializable]
    public class WeaponSetting
    {
        public bool FriendlyCollision;
        public float ProjectileSize;
        public float ProjectileSpeed;
        public float AttackSpeed;
        public float Range;
        public int Damage;
    }
}