using System;
using System.Collections.Generic;
using Gameplay.Buildings;
using Gameplay.Locations;
using Gameplay.Warehouses;
using UnityEngine;

namespace Gameplay.Areas
{
    public interface IAreaTriggerHandler
    {
        void AddTrigger(IAreaTrigger trigger, (IWarehouse input, IWarehouse output) warehouses);
        void RemoveTriggers(IAreaTrigger[] triggers);
        (IWarehouse input, IWarehouse output) GetWarehouses(IAreaTrigger trigger);
    }
    
    public class AreaTriggerHandler : IAreaTriggerHandler
    {
        private readonly ILocationModelGetter _locationModel;
        private readonly Dictionary<IAreaTrigger, (IWarehouse, IWarehouse)> _triggers = new();

        public AreaTriggerHandler(ILocationModelGetter locationModel)
        {
            _locationModel = locationModel;
        }

        public void AddTrigger(IAreaTrigger trigger, (IWarehouse input, IWarehouse output) warehouses)
        {
            _triggers.Add(trigger, warehouses);
        }
        
        public void RemoveTriggers(IAreaTrigger[] triggers)
        {
            foreach (var trigger in triggers)
                _triggers.Remove(trigger);
        }

        public (IWarehouse input, IWarehouse output) GetWarehouses(IAreaTrigger trigger)
        {
            if (_triggers.TryGetValue(trigger, out var value))
                return value;

            return (default, default);
        }
    }
}