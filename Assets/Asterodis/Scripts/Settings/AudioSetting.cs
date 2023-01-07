using System;
using System.Collections.Generic;

namespace Asterodis.Settings
{
    [Serializable]
    public class AudioSetting
    {
        public float Volume;
        public bool Mute;
        public float BitLevelTime;
        public float BitTempMax;
        public List<AudioPresset> Pressets;
    }

    [Serializable]
    public class AudioPresset
    {
        public string AudioName;
        public float ScaleVolume;
    }
}