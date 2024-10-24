using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Gameplay.Warehouses
{
    public interface IWarehouse
    {
        IStorage GetStorage(ResourceType type);
        bool IsFull { get; }
        bool IsEmpty { get; }
        bool IsUnUsed { get; }
        bool Contains(ResourceType type, int quantity);
        int GetFreeSpaceAmount(ResourceType type);
    }
    public class Warehouse : IWarehouse
    {
        protected readonly Dictionary<ResourceType,IStorage> _stored;
        protected readonly int _capacity;

        public Warehouse([CanBeNull] IEnumerable<IStorage> storages)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            _stored = storages?.ToDictionary(s => s.Resource.ResourceType, s => s);
            // ReSharper disable once PossibleMultipleEnumeration
            _capacity = storages?.Sum(s => s.Capacity) ?? 0;
        }

        public bool IsFull => _capacity == _stored.Sum(s => s.Value.Resource.Amount);
        public bool IsEmpty => _stored.All(s => s.Value.Resource.Amount == 0);
        public bool IsUnUsed => _capacity == 0;

        public virtual bool Contains(ResourceType type, int quantity) => 
            GetStorage(type)?.Resource.Amount >= quantity;

        public virtual int GetFreeSpaceAmount(ResourceType resourceType)
        {
            var storage = GetStorage(resourceType);
            return storage?.Capacity - storage?.Resource.Amount ?? 0;
        }

        public virtual IStorage GetStorage(ResourceType resourceType)
        {
            return _stored.GetValueOrDefault(resourceType);
        }
    }
}