using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Gameplay.Buildings;
using UnityEngine;

namespace Gameplay.Warehouses
{
    public interface IManufactureWarehouse
    {
        IWarehouse Input { get; }
        IWarehouse Output { get; }
        IWarehouse Progress { get; }
        bool IsUnusedInput();
        bool CanResourceProduce(Manufacture.ResourceSettings[] requirement);
    }
    public class ManufactureWarehouse : IManufactureWarehouse
    {
        public IWarehouse Input { get; }
        public IWarehouse Output { get; }
        public IWarehouse Progress { get; }
        
        public ManufactureWarehouse(IEnumerable<Storage> inputStorages, IEnumerable<Storage> outputStorages, IEnumerable<Storage> progressStorages)
        {
            Input = new Warehouse(inputStorages);
            Output = new Warehouse(outputStorages);
            Progress = new Warehouse(progressStorages);
        }
        
        public bool CanResourceProduce(Manufacture.ResourceSettings[] requirement)
        {
            return (IsUnusedInput() || CanInputResources(requirement)) && !IsFullOutput();
        }

        public bool IsUnusedInput() => Input.IsUnUsed;

        private bool CanInputResources(Manufacture.ResourceSettings[] requirement)
        {
            return requirement.All(r => Input.Contains(r.ResourceType, r.Amount));
        }

        private bool IsFullOutput() => Output.IsFull;
        
        [Serializable]
        public struct Settings
        {
            public int InputCapacity;
            public int OutputCapacity;
        }
    }

    public interface IManufacture
    {
        void Run();
        void Stop();
        void Dispose();
        public event Action<bool> ProgressChangedEvent; 
    }
    public class Manufacture : IManufacture, IDisposable
    {
        private readonly Settings _settings;
        private readonly IManufactureWarehouse _warehouse;
        private readonly IResourceSpawner _resourceItemSpawner;
        private readonly CustomTimer _timer;
        private readonly ResourceType _outputResourceType;

        public event Action<bool> ProgressChangedEvent;

        public Manufacture(Settings settings, IManufactureWarehouse warehouse, IResourceSpawner resourceItemSpawner)
        {
            _settings = settings;
            _warehouse = warehouse;
            _resourceItemSpawner = resourceItemSpawner;

            // _outputResourceType = settings.Output.ResourceType;
            _timer = new CustomTimer(settings.ProductionTime);
            
            // _warehouse.Input.ResourceAddedEvent += ResourceAddedHandler;
        }

        public void Run()
        {
            if (_warehouse.CanResourceProduce(_settings.Input))
            {
                UseForProduce();
                Produce();
                ProgressChangedEvent?.Invoke(true);
                _timer.StartAsync(ReleaseAndTryToRunProduction).Forget();
            } 
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void ResourceAddedHandler(IResource _) => Run();

        private void ReleaseAndTryToRunProduction()
        {
            Release();
            ProgressChangedEvent?.Invoke(false);
            Run();
        }

        private void UseForProduce()
        {
            foreach (var resource in _settings.Input)
            {
                var input = _warehouse.Input.GetStorage(resource.ResourceType).RemoveResource(resource.Amount);
               _resourceItemSpawner.DeSpawn(input.Collections.ToArray());
            }
        }

        private void Produce()
        {
            var item = _resourceItemSpawner.Spawn(_outputResourceType);
            _warehouse.Progress.GetStorage(_outputResourceType).AddResource(new Resource(_outputResourceType, new []{item}));
            var point = _warehouse.Progress.GetStorage(_outputResourceType).GeеFreePointPlacement();
            item.Animation.PlayJumpTask(point).Forget();
        }

        private void Release()
        {
            TransferResources
                .TransferTo(_warehouse.Progress, _outputResourceType, 1, _warehouse.Output)
                .Forget();
        }
        
        public void Dispose()
        {
            _timer?.Dispose();
            // _warehouse.Input.ResourceAddedEvent -= ResourceAddedHandler;
        }

        [Serializable]
        public struct Settings
        {
            public ResourceSettings[] Input;
            public ResourceSettings[] Output;
            public int ProductionTime;

            public ManufactureWarehouse.Settings Warehouse;
        }
        
        [Serializable]
        public struct ResourceSettings
        {
            public ResourceType ResourceType;
            public int Amount;
        }
    }
}