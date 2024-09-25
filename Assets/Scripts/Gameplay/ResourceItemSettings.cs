using System;
using System.Linq;
using Gameplay.Warehouses;
using UnityEngine;

namespace Gameplay.Constants
{
    [CreateAssetMenu(fileName = "ResourceItemSettings", menuName = "Settings/ResourceItemSettings", order = 0)]
    public class ResourceItemSettings : ScriptableObject
    {
        [SerializeField] private ResourceSettings[] _resources;

        public ResourceItem.Settings GetResourceSettings(ResourceType type)
        {
            return _resources.First(r => r.Type == type).Settings;
        }
        
        [Serializable]
        private struct ResourceSettings
        {
            public ResourceType Type;
            public ResourceItem.Settings Settings;
        }
        
        [Serializable]
        public class AnimationTransferSettings
        {
            
        }
    }
}