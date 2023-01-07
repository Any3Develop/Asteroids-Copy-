using System;
using UnityEngine;

namespace Asterodis.Entities.Movements
{
    public class LinearMovement : IMovement
    {
        protected float Speed;
        protected Vector2 Direction;
        protected float PassedDistance;
        protected Transform Target;
        
        public event Action<float> OnMove;
        public string Id { get; }
        
        public LinearMovement(string id, Transform target, float speed)
        {
            Id = id;
            SetDirection(Vector2.up);
            Target = target;
            Speed = speed;
        }

        public void SetDirection(Vector3 value)
        {
            Direction = value;
        }

        public void SetSpeed(float value)
        {
            Speed = value;
        }

        public void SetPosition(Vector3 value)
        {
            Target.position = value;
        }

        public virtual void Update()
        {
            if (!Target)
                return;

            var translation = Direction * Speed * Time.deltaTime;
            PassedDistance += translation.y;
            Target.Translate(translation);
            OnMove?.Invoke(PassedDistance);
        }

        public virtual void Dispose()
        {
            PassedDistance = 0;
            OnMove = null;
        }
    }
}