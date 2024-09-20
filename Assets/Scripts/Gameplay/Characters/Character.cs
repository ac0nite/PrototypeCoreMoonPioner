using System;
using Core.Input;
using Core.Movement;
using Gameplay.Locations;
using Zenject;

namespace Gameplay.Characters
{
    public class Character : IInitializable, IDisposable
    {
        private readonly Settings _settings;
        private readonly IInputHandler _inputHandler;
        private readonly ILocationModelGetter _locationModel;
        private readonly CharacterView _view;

        public Character(
            Settings settings,
            IInputHandler inputHandler,
            CharacterView.Factory viewFactory,
            ILocationModelGetter locationModel)
        {
            _settings = settings;
            _inputHandler = inputHandler;
            _locationModel = locationModel;
            _view = viewFactory.Create();
        }
        
        public void Initialize()
        {
            _view.Movement = new ObjectMovement(_view.transform, _settings.MovementSettings, () => _inputHandler.Direction, _locationModel.ClampMovement);
            _view.Rotation = new ObjectRotation(_view.transform, _settings.RotationSettings, () => _inputHandler.Direction);
        }
        
        public void Dispose()
        {
            _view.Movement = null;
            _view.Rotation = null;
        }
        
        private void SetPoint(LocationSettings.Point point)
        {
            point.ApplyTo(_view.transform);
        }
        
        private void SetEnable(bool enabled)
        {
            _view.gameObject.SetActive(enabled);
        }

        #region Settings

        [Serializable]
        public class Settings
        {
            public ObjectMovement.Settings MovementSettings;
            public ObjectRotation.Settings RotationSettings;
        }

        #endregion

        #region Pool

        public sealed class Pool : MemoryPool<LocationSettings.Point, Character>
        {
            protected override void OnCreated(Character item)
            {
                base.OnCreated(item);
                item.SetEnable(false);
            }

            protected override void Reinitialize(LocationSettings.Point point, Character item)
            {
                item.Initialize();
                item.SetPoint(point);
                item.SetEnable(true);
            }

            protected override void OnDespawned(Character item)
            {
                item.Dispose();
                base.OnDespawned(item);
            }
        }

        #endregion
    }
}