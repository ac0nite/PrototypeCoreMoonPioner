using System.Collections.Generic;
using Gameplay.Buildings;
using Gameplay.Characters;
using Gameplay.Constants;
using UnityEngine;

namespace Gameplay.Locations
{
    
    public interface ILocationModelGetter
    { 
        IReadOnlyList<Building> Buildings { get; }
        Character Character { get; }
        Vector3 ClampMovement(Vector3 point);
        Vector3 ResourceItemSize { get; }
    }
    public interface ILocationModelSetter
    {
        List<Building> Buildings { get; set; }
        Character Character { get; set; }
        LocationModel.BoundRadius MovementBoundRadius  { get; set; }
        Vector3 ResourceItemSize { get; set; }
    }
    
    public class LocationModel : ILocationModelGetter, ILocationModelSetter
    {
        IReadOnlyList<Building> ILocationModelGetter.Buildings { get; }
        public List<Building> Buildings { get; set; }
        public Character Character { get; set; }
        public BoundRadius MovementBoundRadius { get; set; }
        
        public Vector3 ClampMovement(Vector3 point)
        {
            return point.sqrMagnitude > MovementBoundRadius.Double ? point.normalized * MovementBoundRadius.Value : point;
        }

        public Vector3 ResourceItemSize { get; set; }

        public struct BoundRadius
        {
            public float Value;
            public float Double;
        }
    }

    public class AnimationDebug
    {
        private readonly ILocationModelGetter _locationModel;
        private readonly ResourceItem.Pool _resourcePool;

        public AnimationDebug(
            ILocationModelGetter locationModel,
            ResourceItem.Pool resourcePool)
        {
            _locationModel = locationModel;
            _resourcePool = resourcePool;
        }
    }
}