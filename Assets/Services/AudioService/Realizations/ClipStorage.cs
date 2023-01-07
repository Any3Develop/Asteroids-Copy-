using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Services.AudioService
{
    public class ClipStorage : IClipStorage, IInitializable, IDisposable
    {
        private readonly string clipsPath;
        private Dictionary<string, AudioClip> storage;

        public ClipStorage(string clipsPath)
        {
            this.clipsPath = clipsPath;
        }

        public void Initialize()
        {
            var clips = Resources.LoadAll<AudioClip>(clipsPath);
            storage = clips.ToDictionary(x => x.name);
        }

        public void Dispose()
        {
            foreach (var audioClip in storage.Values.ToArray())
            {
                Resources.UnloadAsset(audioClip);
            }

            storage.Clear();
        }

        public AudioClip Get(string id)
        {
            return !Contains(id) ? default : storage[id];
        }

        public IEnumerable<AudioClip> GetAll()
        {
            return storage.Values;
        }

        public bool Contains(string id)
        {
            return storage.ContainsKey(id);
        }
    }
}