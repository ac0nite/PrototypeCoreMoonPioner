using System.IO;
using System.Linq;
using Gameplay.Buildings;
using Gameplay.Characters;
using Gameplay.Warehouses;
using UnityEngine;
using Zenject;

namespace Gameplay.Locations
{
    public class LoadLocations
    {
        private readonly Character.Pool _characterPool;
        private readonly Building.Pool _buildingPool;
        private readonly Camera _generalCamera;
        private readonly ILocationModelSetter _locationModel;
        private readonly Manufacture.Settings _manufactureSettings;

        private readonly string[] _locationsConfig = CreatorLocation.GetLocationNames();


        public LoadLocations(
            Character.Pool characterPool,
            Building.Pool buildingPool,
            [Inject (Id = Constants.Constants.ID.GeneralCamera)] Camera generalCamera,
            ILocationModelSetter locationModel,
            Manufacture.Settings manufactureSettings)
        {
            _characterPool = characterPool;
            _buildingPool = buildingPool;
            _generalCamera = generalCamera;
            _locationModel = locationModel;
            
            //TODO производство пока с одинаковыми настройками
            _manufactureSettings = manufactureSettings;
        }

        public void Load(int locationNumber)
        {
            var locations = _locationsConfig[Mathf.Clamp(locationNumber, 0, _locationsConfig.Length)];
            var settings = JsonUtility.FromJson<LocationSettings>(File.ReadAllText(locations));
            
            _locationModel.Character = _characterPool.Spawn(settings.Character);
            _locationModel.Buildings = settings.Buildings.Select(s => _buildingPool.Spawn(s, _manufactureSettings)).ToList();
            _locationModel.MovementBoundRadius = new LocationModel.BoundRadius
            {
                Value = settings.MovementBboundsRadius, 
                Double = settings.MovementBboundsRadius * settings.MovementBboundsRadius
            };

            settings.Camera.ApplyTo(_generalCamera.transform);
        }
        
        public void Clear()
        {
            _characterPool.Despawn(_locationModel.Character);
            _locationModel.Buildings.ForEach(b => _buildingPool.Despawn(b));
            _locationModel.Character = null;
            _locationModel.Buildings.Clear();
        }
    }
}