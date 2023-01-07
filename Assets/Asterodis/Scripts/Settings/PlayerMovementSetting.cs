using System;
using UnityEngine;

namespace Asterodis.Settings
{
    [Serializable]
    public class PlayerMovementSetting
    {
        [Header("Movement")]
        public float AccelerationMovementMax;
        public float AccelerationMovementFactor;
        public float DecelerationMovementFactor;
        
        [Header("Rotation")]
        public float AccelerationRotationFactor;
        public float DecelerationRotationFactor;
    }
}