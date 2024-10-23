using System;
using UnityEngine;

namespace Core.PlacementsStorage
{
    public class PlacementDebugExample : MonoBehaviour
    {
        [SerializeField] private Placement.PointConfig[] placementConfigs;

#if UNITY_EDITOR
      private void OnDrawGizmos()
        {
            foreach (var config in placementConfigs)
            {
                config.EditorGizmoDraw();
            }
        }  
#endif
    }
}