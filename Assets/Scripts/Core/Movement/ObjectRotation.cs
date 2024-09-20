using System;
using UnityEngine;

namespace Core.Movement
{
    public class ObjectRotation
    {
        private readonly Transform _owner;
        private readonly Settings _settings;
        private readonly Func<Vector3> InputDirectionFunc;
        
        private Vector3 _direction;
        private Quaternion _targetRotation;

        public ObjectRotation(Transform owner, Settings settings, Func<Vector3> inputDirectionFunc)
        {
            _owner = owner;
            _settings = settings;
            InputDirectionFunc = inputDirectionFunc;
        }

        public void Update(float deltaTime)
        {
            _direction = InputDirectionFunc();
            if (_direction != Vector3.zero)
            {
                _targetRotation = Quaternion.LookRotation(_direction);
                _owner.rotation = Quaternion.Slerp(_owner.rotation, _targetRotation, _settings.Speed * deltaTime);
            }
        }
        
        [Serializable]
        public class Settings
        {
            public float Speed = 5f;
        }
    }
}