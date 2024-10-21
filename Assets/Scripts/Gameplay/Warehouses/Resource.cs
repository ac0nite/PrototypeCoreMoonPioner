using System;
using System.Collections.Generic;
using Gameplay.Constants;

namespace Gameplay.Warehouses
{
    public interface IResource
    {
        ResourceType ResourceType { get; }
        int Amount { get; }
        IReadOnlyCollection<ResourceItem> Collections { get; }
        void Add(params ResourceItem[] items);
        ResourceItem Remove();
    }
    
    public class Resource : IResource
    {
        private readonly Stack<ResourceItem> _items = new();
        public ResourceType ResourceType { get; }
        public int Amount => _items.Count;
        public Resource(ResourceType resourceType)
        {
            ResourceType = resourceType;
        }
        
        public Resource(ResourceType resourceType, params ResourceItem[] items)
        {
            ResourceType = resourceType;
            Add(items);
        }

        public void Add(params ResourceItem[] items)
        {
            foreach (var item in items)
            {
                _items.Push(item);
            }
        }

        public ResourceItem Remove()
        {
            if (_items.TryPop(out var item))
                return item;

            throw new InvalidOperationException("No items left.");
        }

        public IReadOnlyCollection<ResourceItem> Collections => _items;
    }
}