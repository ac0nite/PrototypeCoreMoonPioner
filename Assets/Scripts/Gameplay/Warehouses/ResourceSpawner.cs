using Gameplay.Constants;
using UnityEngine;

namespace Gameplay.Warehouses
{
    public interface IResourceSpawner
    {
        ResourceItem Spawn(ResourceType type);
        
        //TODO Color resourceColor вместо Color в послкдствии нужно возвратить настройки для анимации или при создании учесть
        (ResourceItem item, Color targetColor) Spawn(ResourceType createType, ResourceType resourceType);
        void DeSpawn(params ResourceItem[] items);
    }
    public class ResourceSpawner : IResourceSpawner
    {
        private readonly ResourceItem.Pool _resourceItemPool;
        private readonly ResourceItemSettings _settings;

        public ResourceSpawner(
            ResourceItem.Pool resourceItemPool,
            ResourceItemSettings settings)
        {
            _resourceItemPool = resourceItemPool;
            _settings = settings;
        }
        
        public ResourceItem Spawn(ResourceType type)
        {
            return _resourceItemPool.Spawn(_settings.GetResourceSettings(type));
        }

        public (ResourceItem item, Color targetColor) Spawn(ResourceType createType, ResourceType resourceType)
        {
            return (
                _resourceItemPool.Spawn(_settings.GetResourceSettings(createType)), 
                _settings.GetResourceSettings(resourceType).Color);
        }

        public void DeSpawn(params ResourceItem[] items)
        {
            foreach (var item in items)
                _resourceItemPool.Despawn(item);
        }
    }
}