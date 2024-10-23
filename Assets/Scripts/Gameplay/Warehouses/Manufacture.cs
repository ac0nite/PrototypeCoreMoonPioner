using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;

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
        UniTask RunAsync();
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

        public async UniTask RunAsync()
        {
            if (_warehouse.CanResourceProduce(_settings.Input))
            {
                await UseForProduceAsync();
                await ProduceAsync();
                ProgressChangedEvent?.Invoke(true);
                _timer.StartAsync(ReleaseAndTryToRunProduction).Forget();
            } 
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void ResourceAddedHandler(IResource _) => RunAsync();

        private void ReleaseAndTryToRunProduction()
        {
            ReleaseAsync().Forget();
            ProgressChangedEvent?.Invoke(false);
            RunAsync();
        }

        private async UniTask UseForProduceAsync()
        {
            await UniTask.Yield();
            for (int i = 0; i < _settings.Input.Length; i++)
            {
                var settings = _settings.Input[i];
                for (int j = 0; j < settings.Amount; j++)
                {
                    var input = _warehouse.Input.GetStorage(settings.ResourceType).RemoveResource(1);
                    _resourceItemSpawner.DeSpawn(input.Collections.ToArray());
                }
            }
            
            // foreach (var resource in _settings.Input)
            // {
            //     var input = _warehouse.Input.GetStorage(resource.ResourceType).RemoveResource(resource.Amount);
            //    _resourceItemSpawner.DeSpawn(input.Collections.ToArray());
            // }
        }

        private async UniTask ProduceAsync()
        {
            await UniTask.Yield();
            for (int i = 0; i < _settings.Output.Length; i++)
            {
                var settings = _settings.Output[i];
                for (int j = 0; j < settings.Amount; j++)
                {
                    var param = _resourceItemSpawner.Spawn(ResourceType.Progress, settings.ResourceType);
                    var storage = _warehouse.Progress.GetStorage(settings.ResourceType);
                    storage.AddResource(new Resource(settings.ResourceType, param.item));
                    param.item.Animation.PlayProgressTask(storage.GeеFreePointPlacement(), param.targetColor, _settings.ProductionTime).Forget();
                }
            }
        }

        private async UniTask ReleaseAsync()
        {
            await UniTask.Yield();
            for (int i = 0; i < _settings.Output.Length; i++)
            {
                var settings = _settings.Output[i];
                TransferResources.TransferTo(_warehouse.Progress, settings.ResourceType, 1, _warehouse.Output).Forget();
            }
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