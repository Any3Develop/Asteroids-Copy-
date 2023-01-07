using System;
using System.Collections;
using UnityEngine;

namespace Services.AudioService
{
    public class AudioPlayerView : MonoBehaviour, IAudioPlayer
    {
        [SerializeField] private AudioSource player;
        public float Volume => player ? player.volume : 0f;
        public bool IsMute => player && player.mute;
        
        public event Action<IAudioPlayer> OnReleased;
        public event Action OnCompleted;
        private Coroutine playing;
        
        public void SetMute(bool value)
        {
            if (!player)
                return;
            
            player.mute = value;
        }

        public void SetVolume(float value)
        {
            if (!player)
                return;
            
            player.volume = value;
        }

        public void Play(AudioClip value, bool loop)
        {
            if (!player)
                return;
            
            Stop();
            gameObject.SetActive(true);
            player.clip = value;
            player.loop = loop;
            player.Play();
            playing = StartCoroutine(Playing());
        }

        public void PlayOneShot(AudioClip value, float volume)
        {
            gameObject.SetActive(true);
            player.PlayOneShot(value, volume);
        }

        public void Repeat()
        {
            if (!player)
                return;
            
            Stop();
            player.Play();
            playing = StartCoroutine(Playing());
        }

        public void Stop()
        {
            if (playing != null)
            {
                StopCoroutine(playing);
                playing = null;
            }
            
            if (!player)
                return;
            
            player.Stop();
        }

        public void Release()
        {
            Stop();
            if (player)
                player.clip = null;

            gameObject.SetActive(false);
            OnCompleted?.Invoke();
            OnReleased?.Invoke(this);
            OnReleased = null;
            OnCompleted = null;
            playing = null;
        }

        private IEnumerator Playing()
        {
            if (player != null && player.loop)
            {
                playing = null;
                yield break;
            }
            
            while (Application.isPlaying && player != null && player.isPlaying)
            {
                yield return null;
            }

            OnCompleted?.Invoke();
            playing = null;
        }

        public void Dispose()
        {
            OnReleased = null;
            OnCompleted = null;
            playing = null;
            Destroy(gameObject);
        }
    }
}