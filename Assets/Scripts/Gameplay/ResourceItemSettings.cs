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
        [SerializeField] private ResourceItemAnimation.Settings _animationSettings;

        public ResourceItem.Settings GetResourceSettings(ResourceType type) => _resources.First(r => r.Type == type).Settings;
        public ResourceItemAnimation.Settings AnimationSettings => _animationSettings;
        
        [Serializable]
        private struct ResourceSettings
        {
            public ResourceType Type;
            public ResourceItem.Settings Settings;
        }
    }
}