using UnityEngine;

namespace Asterodis.GameBuilder
{
    public class WhileTimer : ITask
    {
        private float timer;
        public bool IsEnded => timer < Time.time;
        public bool Disposed => IsEnded;
        public float Progress => IsEnded ? 1f : Time.time / timer;

        public WhileTimer(float duration)
        {
            timer = duration + Time.time;
        }
        
        public void Dispose()
        {
            timer = 0;
        }
    }
}