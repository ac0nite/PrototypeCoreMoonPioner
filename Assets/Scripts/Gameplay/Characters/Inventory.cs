using System.Collections.Generic;
using System.Linq;
using Core.PlacementsStorage;
using Gameplay.Warehouses;
using UnityEngine;

namespace Gameplay.Characters
{
    public interface IInventory : IStorage
    {
    }
    
    public class Inventory : IInventory
    {
        private readonly int _capacity;
        private readonly Stack<IResource> _storages = new();
        private readonly Placement _placement;

        public Inventory(int capacity, Placement.PointConfig placementConfig, Placement.Size size)
        {
            Capacity = capacity;
            _placement = new Placement(placementConfig, size, () => _storages.Sum(s => s.Amount));
        }
        public IResource Resource => _storages.TryPeek(out IResource result) ? result : null;
        public int Capacity { get; }
        public Placement.Point GeеFreePointPlacement() => _placement.GetNextPoint();

        public void AddResource(IResource resource)
        {
            if(Resource?.ResourceType == resource.ResourceType)
                Resource.Add(resource.Collections.ToArray());
            else
                _storages.Push(resource);

            var sum = _storages.Sum(s => s.Amount);
            var pos = _placement.GetNextPoint().Position;
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