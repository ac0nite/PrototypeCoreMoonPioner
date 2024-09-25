using System;
using UnityEngine;

namespace Gameplay.Warehouses
{
    public interface IResource
    {
        ResourceType ResourceType { get; }
        int Amount { get; }
    }
    
    [Serializable]
    public class Resource : IResource
    {
        [SerializeField] private ResourceType _resourceType;
        [SerializeField] private int _amount;
        public ResourceType ResourceType => _resourceType;
        public int Amount => _amount;

        public Resource(ResourceType resourceType, int amount)
        {
            _resourceType = resourceType;
            _amount = amount;
        }
        
        public Resource(ResourceType resourceType)
        {
            _resourceType = resourceType;
            _amount = 1;
        }
        
        [Serializable]
        public struct ProductionRequirement
        {
            public Resource[] InputResources;
            public Resource OutputResource;
            public int ProductionTime;
        }
    }
}