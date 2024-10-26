using System.Linq;
using Core.PlacementsStorage;

namespace Gameplay.Warehouses
{
    public interface IStorage
    {
        IResource Resource { get; }
        int Capacity { get; }
        IPlacement Placement { get; }
        void AddResource(IResource resource);
        IResource RemoveResource(int quantity);
    }
    public class Storage : IStorage
    {
        private readonly IPlacement _placement;
        private readonly IResource _resource;
        
        public Storage(IResource resource, int capacity, IPlacement placement)
        {
            _resource = resource;
            
            Placement = placement;
            Capacity = capacity;
        }
        
        public IResource Resource => _resource;
        public int Capacity { get; }
        public IPlacement Placement { get; }
        public void AddResource(IResource resource)
        {
            if(_resource.ResourceType != resource.ResourceType) 
                throw new System.Exception("Wrong resource type");
            
            foreach (var resourceItem in resource.Collections)
                _resource.Add(resourceItem);
        }
        public IResource RemoveResource(int quantity)
        {
            return new Resource(
                _resource.ResourceType,
                Enumerable.Range(0, quantity).Select(_ => _resource.Remove()).ToArray());
        }
    }
}