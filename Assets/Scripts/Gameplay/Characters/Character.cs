using System;
using System.Collections;
using System.Linq;
using Core.Input;
using Core.Movement;
using Gameplay.Locations;
using Gameplay.Warehouses;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Gameplay.Characters
{
    public class Character : IInitializable, IDisposable
    {
        private readonly Settings _settings;
        private readonly IInputHandler _inputHandler;
        private readonly ILocationModelGetter _locationModel;
        private readonly IResourceSpawner _resourceItemSpawner;
        private readonly CharacterView _view;
        private Inventory _inventory;

        public Character(
            Settings settings,
            IInputHandler inputHandler,
            CharacterView.Factory viewFactory,
            ILocationModelGetter locationModel,
            IResourceSpawner resourceItemSpawner)
        {
            _settings = settings;
            _inputHandler = inputHandler;
            _locationModel = locationModel;
            _resourceItemSpawner = resourceItemSpawner;
            _view = viewFactory.Create();
        }
        
        private IEnumerator Tick()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    var types = Enum.GetValues(typeof(ResourceType));
                    var type = (ResourceType)Random.Range(0, types.Length);
                    var item = _resourceItemSpawner.Spawn(type);
                    _inventory.AddResource(new Resource(type, item));
                    item.SetPoint(_inventory.GeеFreePointPlacement(), true);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    var top = _inventory.Resource;
                    if (top != null)
                    {
                        var r = _inventory.RemoveResource(top.Amount);
                        _resourceItemSpawner.DeSpawn(r.Collections.ToArray());
                    }
                }
            
                yield return null;   
            }
        }

        public void Initialize()
        {
            _view.Movement = new ObjectMovement(_view.transform, _settings.MovementSettings, () => _inputHandler.Direction, _locationModel.ClampMovement);
            _view.Rotation = new ObjectRotation(_view.transform, _settings.RotationSettings, () => _inputHandler.Direction);
            
            _inventory = new Inventory(20, _view.InventoryPlacementPointConfig, _locationModel.ResourceItemSize);
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
            if(enabled) _view.StartCoroutine(Tick());
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