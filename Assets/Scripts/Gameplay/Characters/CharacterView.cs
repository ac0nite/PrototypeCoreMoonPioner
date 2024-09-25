using System;
using Core.Movement;
using UnityEngine;
using Zenject;

namespace Gameplay.Characters
{
    public class CharacterView : MonoBehaviour
    {
        public ObjectMovement Movement { get; set; }
        public ObjectRotation Rotation { get; set; }
        
        public event Action<Collider> OnTriggerEnterEvent;
        public event Action<Collider> OnTriggerExitEvent;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"OnTriggerEnter: {other.gameObject.name}");
            OnTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log($"OnTriggerExit: {other.gameObject.name}");
            OnTriggerExitEvent?.Invoke(other);
        }

        private void Update()
        {
            Movement?.Update(Time.deltaTime);
            Rotation?.Update(Time.deltaTime);
        }

        #region Factory

        public class Factory : PlaceholderFactory<CharacterView>
        {
        }

        #endregion
    }
}