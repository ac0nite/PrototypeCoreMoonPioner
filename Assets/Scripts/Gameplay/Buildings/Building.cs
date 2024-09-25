using System;
using Gameplay.Locations;
using Gameplay.Warehouses;
using UnityEngine;
using Zenject;

namespace Gameplay.Buildings
{
    public class Building : IDisposable
    {
        private readonly BuildingView _view;
        private readonly InfoBoard _infoBoard;
        
        private IManufacture _manufacture;
        private IManufactureWarehouse _warehouse;

        public Building(
            BuildingView.Factory viewFactory,
            InfoBoard.Factory infoBoardFactory,
            [Inject (Id = Constants.Constants.ID.GeneralCamera)] Camera generalCamera
            )
        {
            _view = viewFactory.Create();
            _view.SetRenderCamera(generalCamera);
            _infoBoard = infoBoardFactory.Create(_view.InfoBoardPoint);
        }

        private void Initialise(Settings buildSettings, Manufacture.Settings manufactureSettings)
        {
            buildSettings.Point.ApplyTo(_view.transform);
            
            //TODO предполагается инициализация внешнего вида в зависимости от типа здания
            _view.Type = buildSettings.Type;
            
            _warehouse = new ManufactureWarehouse(manufactureSettings.Warehouse);
            _manufacture = new Manufacture(manufactureSettings.ProductionRequirement, _warehouse);
            
            _infoBoard.Initialize(_warehouse);
            
            _manufacture.Run();
        }
        
        public void Dispose()
        {
            _infoBoard.Dispose();
        }
        
        private void Enabled(bool enabled)
        {
            _view.gameObject.SetActive(enabled);
        }

        #region Settings

        [Serializable]
        public class Settings
        {
            public BuildType Type;
            public LocationSettings.Point Point;
        }

        #endregion
        
        #region Pool

        public class Pool : MemoryPool<Settings, Manufacture.Settings, Building>
        {
            protected override void OnCreated(Building item)
            {
                item.Enabled(false);
            }

            protected override void Reinitialize(Settings buildSettings, Manufacture.Settings manufactureSettings,Building item)
            {
                item.Initialise(buildSettings, manufactureSettings);
                item.Enabled(true);
            }

            protected override void OnDespawned(Building item)
            {
                item.Dispose();
                item.Enabled(false);
            }
        }

        #endregion
    }
}