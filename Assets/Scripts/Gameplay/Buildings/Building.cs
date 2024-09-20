using System;
using Gameplay.Locations;
using UnityEngine;
using Zenject;

namespace Gameplay.Buildings
{
    public class Building
    {
        private readonly BuildingView _view;

        public Building(BuildingView.Factory viewFactory)
        {
            _view = viewFactory.Create();
        }
        
        private void SetPoint(Settings point)
        {
            _view.transform.position = point.Point.Position;
            _view.transform.rotation = Quaternion.Euler(point.Point.Rotation); 
            _view.Type = point.Type;
        }

        #region Settings

        [Serializable]
        public class Settings
        {
            public BuildType Type;
            public LocationSettings.Point Point;
        }

        #endregion
        
        #region Pool

        public class Pool : MemoryPool<Settings, Building>
        {
            protected override void OnCreated(Building item)
            {
                base.OnCreated(item);
                item._view.gameObject.SetActive(false);
            }

            protected override void OnSpawned(Building item)
            {
                base.OnSpawned(item);
                item._view.gameObject.SetActive(true);
            }

            protected override void Reinitialize(Settings point, Building item)
            {
                base.Reinitialize(point, item);
                item.SetPoint(point);
            }
        }

        #endregion
    }
}