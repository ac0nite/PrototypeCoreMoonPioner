using Gameplay.Locations;
using UnityEngine;
using Zenject;

namespace Gameplay.Buildings
{
    public class BuildingView : MonoBehaviour
    {
        public BuildType Type;

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