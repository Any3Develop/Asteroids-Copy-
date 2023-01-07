using System.Collections.Generic;
using UnityEngine;

namespace Services.AudioService
{
    public interface IClipStorage
    {
        AudioClip Get(string id);
        IEnumerable<AudioClip> GetAll();
        bool Contains(string id);
    }
}