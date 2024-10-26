using System;
using Gameplay.Warehouses;
using UnityEngine;

namespace Gameplay.Areas
{
    public interface IAreaTrigger
    {
        public AreaTrigger.Type TriggerType { get; }
    }
    public class AreaTrigger : MonoBehaviour, IAreaTrigger
    {
        [Flags]
        public enum Type { INPUT = 1, OUTPUT = 2 }
        
        [SerializeField] private Type _triggerType;
        
        public Type TriggerType => _triggerType;
    }

    public static class TriggerTypeExtensions
    {
        public static bool HasFlagFast(this AreaTrigger.Type value, AreaTrigger.Type flag)
        {
            return (value & flag) != 0;
        }
    }
}