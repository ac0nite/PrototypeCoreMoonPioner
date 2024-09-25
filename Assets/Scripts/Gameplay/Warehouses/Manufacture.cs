using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.Warehouses
{
    public interface IWarehouseObserver
    {
        void OnWarehouseUpdate(Resource resource, Transform from, Transform to);
    }
    
    
    
    /*
       private delegate void FromPlayerToWarehouseDelegate(Resources resource, Character character, Building building);
       private delegate void FromInputWarehouseToManufactureDelegate(Resources resource, Building building);
       private delegate void FromManufactureToWarehouseDelegate(Resources resource, Building building);
       private delegate void FromWarehouseToCharacterDelegate(Resources resource, Building building, Character character);
     */

    public interface IManufactureWarehouse
    {
        IWarehouse Input { get; }
        IWarehouse Output { get; }
        bool CanResourceProduce(params IResource[] resources);
    }
    public class ManufactureWarehouse : IManufactureWarehouse
    {
        public IWarehouse Input { get; }
        public IWarehouse Output { get; }
        
        private readonly IResource _verificationResource = new Resource(ResourceType.Resource1);
        public ManufactureWarehouse(Settings settings)
        {
            Input = new Warehouse(settings.InputCapacity);
            Output = new Warehouse(settings.OutputCapacity);
        }
        
        public bool CanResourceProduce(params IResource[] resources)
        {
            var b0 = IsUnusedInput();
            var b1 = CanInputResources(resources);
            var b2 = IsFullOutput();
            
            return (IsUnusedInput() || CanInputResources(resources)) && !IsFullOutput();
        }

        private bool IsUnusedInput() => Input.StoredResources.Count == 0 && !Input.CanAddResource(_verificationResource);

        private bool CanInputResources(IResource[] resources)
        {
            return resources.All(r => Input.StoredResources.ContainsKey(r.ResourceType) && Input.StoredResources[r.ResourceType] >= r.Amount);
        }

        private bool IsFullOutput()
        {
            return !Output.CanAddResource(_verificationResource);
        }
        
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
    }
    public class Manufacture : IManufacture, IDisposable
    {
        private readonly Resource.ProductionRequirement _requirement;
        private readonly IManufactureWarehouse _warehouse;
        private readonly CustomTimer _timer;
        private readonly IResource[] _requirementResource;

        public Manufacture(Resource.ProductionRequirement requirement, IManufactureWarehouse warehouse)
        {
            _requirement = requirement;
            _warehouse = warehouse;

            _requirementResource = _requirement.InputResources.Select(r => (IResource)r).ToArray();
            _timer = new CustomTimer(_requirement.ProductionTime);
            
            _warehouse.Input.ResourceAddedEvent += ResourceAddedHandler;
        }

        public void Run()
        {
            var е0 = _warehouse.CanResourceProduce(_requirementResource);
            if (_warehouse.CanResourceProduce(_requirementResource))
            {
                UseForProduce();
                _timer.StartAsync(ProduceAndTryToRunProduction).Forget();
            } 
        }

        public void Stop()
        {
            _timer.Stop();
        }
        
        private void ResourceAddedHandler(IResource _) => Run();

        private void ProduceAndTryToRunProduction()
        {
            Produce();
            Run();
        }

        private void UseForProduce()
        {
            foreach (var resource in _requirementResource)
                _warehouse.Input.RemoveResource(resource.ResourceType, resource.Amount);
        }

        private void Produce()
        {
            _warehouse.Output.AddResource(_requirement.OutputResource);
        }
        
        public void Dispose()
        {
            _timer?.Dispose();
            _warehouse.Input.ResourceAddedEvent -= ResourceAddedHandler;
        }

        [Serializable]
        public struct Settings
        {
            public Resource.ProductionRequirement ProductionRequirement;
            public ManufactureWarehouse.Settings Warehouse;
        }
    }

    public interface IWarehouse
    {
        event Action<IResource> ResourceAddedEvent;
        event Action<IResource> ResourceRemovedEvent;
        
        IReadOnlyDictionary<ResourceType, int> StoredResources { get; }
        bool CanAddResource(IResource resource);
        IResource RemoveResource(ResourceType resourceType, int amount);
        void AddResource(IResource resource);
    }
    public class Warehouse : IWarehouse
    {
        private Dictionary<ResourceType, int> _storedResources = new();
        private readonly int _capacity;
        
        public event Action<IResource> ResourceAddedEvent;
        public event Action<IResource> ResourceRemovedEvent;

        public Warehouse(int capacity)
        {
            _capacity = capacity;
        }
        
        public IReadOnlyDictionary<ResourceType, int> StoredResources => _storedResources;

        public bool CanAddResource(IResource resource)
        {
            return CurrentCapacity + resource.Amount <= _capacity;
        }

        public void AddResource(IResource resource)
        {
            if (CanAddResource(resource))
            {
                if(_storedResources.ContainsKey(resource.ResourceType))
                    _storedResources[resource.ResourceType] += resource.Amount;
                else
                    _storedResources.Add(resource.ResourceType, resource.Amount);
                
                ResourceAddedEvent?.Invoke(resource);
            }
        }

        public IResource RemoveResource(ResourceType resourceType, int amount)
        {
            if (_storedResources.ContainsKey(resourceType) && _storedResources[resourceType] >= amount)
            {
                _storedResources[resourceType] -= amount;
                var resource = new Resource(resourceType, amount);
                ResourceRemovedEvent?.Invoke(resource);
                return resource;
            }
            return null;
        }

        private int CurrentCapacity => _storedResources.Sum(x => x.Value);
    }
}