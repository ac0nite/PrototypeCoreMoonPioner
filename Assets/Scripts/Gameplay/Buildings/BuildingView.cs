using Gameplay.Locations;
using UnityEngine;
using Zenject;

namespace Gameplay.Buildings
{
    public class BuildingView : MonoBehaviour
    {
        public BuildType Type;
        
        [SerializeField] private Transform _inputWarehousePoint;
        [SerializeField] private Transform _outputWarehousePoint;
        [SerializeField] private Canvas _canvas;

        public Transform InputWarehousePoint => _inputWarehousePoint;
        public Transform OutputWarehousePoint => _outputWarehousePoint;
        public Transform InfoBoardPoint => _canvas.transform;
        
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
#endif
        #endregion
    }
}