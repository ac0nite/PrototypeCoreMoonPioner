using System;
using System.Collections;
using System.Linq;
using Common;
using Core.PlacementsStorage;
using Cysharp.Threading.Tasks;
using Gameplay.Locations;
using Gameplay.Warehouses;
using UnityEngine;
using Zenject;

namespace Gameplay.Buildings
{
    public class Building : IDisposable
    {
        private readonly IResourceSpawner _resourceItemSpawner;
        private readonly BuildingView _view;
        private readonly InfoBoard _infoBoard;
        
        private IManufacture _manufacture;
        private IManufactureWarehouse _warehouse;
        private readonly ILocationModelGetter _locationModel;

        public Building(
            BuildingView.Factory viewFactory,
            InfoBoard.Factory infoBoardFactory,
            [Inject (Id = Constants.Constants.ID.GeneralCamera)] Camera generalCamera,
            IResourceSpawner resourceItemSpawner,
            ILocationModelGetter locationModel
            )
        {
            _resourceItemSpawner = resourceItemSpawner;
            _view = viewFactory.Create();
            _view.SetRenderCamera(generalCamera);
            _infoBoard = infoBoardFactory.Create(_view.InfoBoardPoint);
            _locationModel = locationModel;
        }

        private void Initialise(Settings buildSettings, Manufacture.Settings manufactureSettings)
        {
            buildSettings.Point.ApplyTo(_view.transform);

            //TODO предполагается инициализация внешнего вида в зависимости от типа здания
            _view.Type = buildSettings.Type;

            var inputStorages = manufactureSettings.Input.Select((s, index) =>
            {
                var resource = new Resource(s.ResourceType);
                var capacity = manufactureSettings.Warehouse.InputCapacity;
                Placement.Size size = _locationModel.ResourceItemSize;
                var placement = new Placement(_view.InputStorageWarehouseConfig, size,
                    resource.Collections as ICollection, index + 1);

                return new Storage(resource, capacity, placement);
            }).ToArray();

            var outputStorages = manufactureSettings.Output.Select((s, index) =>
            {
                var resource = new Resource(s.ResourceType);
                var capacity = manufactureSettings.Warehouse.OutputCapacity;
                Placement.Size size = _locationModel.ResourceItemSize;
                var placement = new Placement(_view.OutputWarehousePointConfig, size,
                    resource.Collections as ICollection, index + 1);

                return new Storage(resource, capacity, placement);
            }).ToArray();

            var manufactureStorages = manufactureSettings.Output.Select((s, index) =>
            {
                var resource = new Resource(s.ResourceType);
                var capacity = s.Amount;
                Placement.Size size = _locationModel.ResourceItemSize;
                var placement = new Placement(_view.ManufacturePointConfig, size, resource.Collections as ICollection,
                    index + 1);

                return new Storage(resource, capacity, placement);
            }).ToArray();

            _warehouse = new ManufactureWarehouse(inputStorages, outputStorages, manufactureStorages);
            _manufacture = new Manufacture(manufactureSettings, _warehouse, _resourceItemSpawner);

            _infoBoard.Initialize(_manufacture, _warehouse);

            _manufacture.ProgressChangedEvent += ProgressChangedHandler;

            _manufacture.Run();
        }

        private void ProgressChangedHandler(bool status)
        {
            if (!status) return;

            if (_warehouse.IsUnusedInput())
            {
                //var item = _resourcePool.Spawn();
            }
        }

        public void Dispose()
        {
            _infoBoard.Dispose();
            _manufacture.Dispose();
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