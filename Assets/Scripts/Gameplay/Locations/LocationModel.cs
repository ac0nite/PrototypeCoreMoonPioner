using System.Collections.Generic;
using Gameplay.Buildings;
using Gameplay.Characters;
using UnityEngine;

namespace Gameplay.Locations
{
    
    public interface ILocationModelGetter
    { 
        IReadOnlyList<Building> Buildings { get; }
        Character Character { get; }
        Vector3 ClampMovement(Vector3 point);
    }
    public interface ILocationModelSetter
    {
        List<Building> Buildings { get; set; }
        Character Character { get; set; }
        LocationModel.BoundRadius MovementBoundRadius  { get; set; }
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
        
        public struct BoundRadius
        {
            public float Value;
            public float Double;
        }
    }
}