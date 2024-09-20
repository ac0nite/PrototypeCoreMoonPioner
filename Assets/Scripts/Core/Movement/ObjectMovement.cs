using System;
using UnityEngine;

namespace Core.Movement
{
    public class ObjectMovement
    {
        private readonly Transform _owner;
        private readonly Settings _settings;
        
        private readonly Func<Vector3> InputDirectionFunc;
        private readonly Func<Vector3, Vector3> ClampFunc;
        
        private Vector3 _direction;
        private Vector3 _velocity;
        private Vector3 _target;
        private Vector3 _nextPoint;

        public ObjectMovement(Transform owner, Settings settings, Func<Vector3> inputDirectionFunc, Func<Vector3, Vector3> clampFunc = null)
        {
            _owner = owner;
            _settings = settings;
            InputDirectionFunc = inputDirectionFunc;
            ClampFunc = clampFunc ?? ((v) => v);
        }
        

        public void Update(float deltaTime)
        {
            _direction = InputDirectionFunc();
            if (_direction != Vector3.zero)
            {
                _velocity = _direction * _settings.Speed;
                _target = _owner.position + _velocity * deltaTime;
                _nextPoint = Vector3.MoveTowards(_owner.position, _target, _settings.Speed * deltaTime);
            }
            else
            {
                _velocity = Vector3.Lerp(_velocity, Vector3.zero, _settings.Deceleration * deltaTime);
                _nextPoint = _owner.position + _velocity * deltaTime;
            }

            _owner.position = ClampFunc(_nextPoint);
        }
        
        [Serializable]
        public class Settings
        {
            public float Speed = 5f;
            public float Deceleration = 0.1f;
        }
    }
}