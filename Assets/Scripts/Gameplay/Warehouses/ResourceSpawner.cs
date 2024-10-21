using Gameplay.Constants;

namespace Gameplay.Warehouses
{
    public interface IResourceSpawner
    {
        ResourceItem Spawn(ResourceType type);
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

        public void DeSpawn(params ResourceItem[] items)
        {
            foreach (var item in items)
                _resourceItemPool.Despawn(item);
        }
    }
}