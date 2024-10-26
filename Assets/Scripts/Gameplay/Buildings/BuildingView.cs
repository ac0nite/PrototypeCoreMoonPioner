using System;
using Core.PlacementsStorage;
using Gameplay.Areas;
using Gameplay.Locations;
using UnityEngine;
using Zenject;

namespace Gameplay.Buildings
{
    public class BuildingView : MonoBehaviour
    {
        public BuildType Type;
        
        [SerializeField] private Placement.PointConfig inputStorageWarehouseConfig;
        [SerializeField] private Placement.PointConfig _outputWarehousePointConfig;
        [SerializeField] private Placement.PointConfig _manufacturePointConfig;
        
        [SerializeField] private Canvas _canvas;
        
        private IAreaTrigger[] _areaTriggers;

        public Placement.PointConfig InputStorageWarehouseConfig => inputStorageWarehouseConfig;
        public Placement.PointConfig OutputWarehousePointConfig => _outputWarehousePointConfig;
        public Placement.PointConfig ManufacturePointConfig => _manufacturePointConfig;
        public Transform InfoBoardPoint => _canvas.transform;

        public IAreaTrigger[] AreaTriggers { get; private set; }

        private void Awake()
        {
            AreaTriggers = GetComponentsInChildren<IAreaTrigger>() ?? throw new Exception("No area triggers found");
        }

        public void SetRenderCamera(Camera camera)
        {
            _canvas.worldCamera = camera;
        }

        #region Factory

        public class Factory : PlaceholderFactory<BuildingView>
        {
        }

        #endregion
        
        #region Editor

#if UNITY_EDITOR
        public Building.Settings GetLocationSettingsData()
        {
            return new Building.Settings
            { 
                Type = Type, 
                Point = new LocationSettings.Point 
                { 
                    Position = transform.position, 
                    Rotation = transform.rotation.eulerAngles 
                }
            };
        }

        private void OnDrawGizmos()
        {
            inputStorageWarehouseConfig.EditorGizmoDraw();
            _outputWarehousePointConfig.EditorGizmoDraw();
            _manufacturePointConfig.EditorGizmoDraw();
        }
#endif
        #endregion
    }
}