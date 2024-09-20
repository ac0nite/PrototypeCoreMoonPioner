using System;
using Gameplay.Buildings;
using UnityEngine;

namespace Gameplay.Locations
{
    [Serializable]
    public class LocationSettings
    {
        public Building.Settings[] Buildings;
        public Point Character;
        public Point Camera;
        public float MovementBboundsRadius;
        
        [Serializable]
        public struct Point
        {
            public Vector3 Position;
            public Vector3 Rotation;

            public void ApplyTo(Transform target)
            {
                target.position = Position;
                target.rotation = Quaternion.Euler(Rotation);
            }
        }
    }
}