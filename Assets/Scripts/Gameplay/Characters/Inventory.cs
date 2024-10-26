using System.Collections.Generic;
using System.Linq;
using Core.PlacementsStorage;
using Gameplay.Warehouses;
using UnityEngine;

namespace Gameplay.Characters
{
    public interface IInventory : IStorage
    { }
    
    public class Inventory : IInventory
    {
        private readonly int _capacity;
        private readonly Stack<IResource> _storages = new();

        public Inventory(int capacity, Placement.PointConfig placementConfig, Placement.Size size)
        {
            Capacity = capacity;
            Placement = new Placement(placementConfig, size, () => _storages.Sum(s => s.Amount));
        }
        public IResource Resource => _storages.TryPeek(out IResource result) ? result : null;
        public int Capacity { get; }
        public IPlacement Placement { get; }

        public void AddResource(IResource resource)
        {
            if(Resource?.ResourceType == resource.ResourceType)
                Resource.Add(resource.Collections.ToArray());
            else
                _storages.Push(resource);
        }

        public IResource RemoveResource(int quantity)
        {
            var topStack = Resource;
            
            if (topStack.Amount == quantity)
                return _storages.Pop();
            
            return new Resource(
                topStack.ResourceType,
                Enumerable.Range(0, Mathf.Min(quantity, topStack.Amount)).Select(_ => topStack.Remove()).ToArray());
        }
    }
}