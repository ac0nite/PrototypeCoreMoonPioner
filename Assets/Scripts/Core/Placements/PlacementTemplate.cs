using System;
using UnityEngine;

namespace Core.PlacementsStorage
{
    [CreateAssetMenu(fileName = "PlacementTemplate", menuName = "Placements/PlacementTemplate", order = 0)]
    public class PlacementTemplate : ScriptableObject
    {
        public Placement.FillMode FillMode;
        [SerializeField] private Placement.SideDirection sideDirection;
        [SerializeField] private Placement.ForwardDirection forwardDirection;
        [SerializeField] private Placement.NextStageDirection nextStageDirection;
        [SerializeField] private Placement.NextStageDirection nextStorageDirection;

        public Vector3 GetSideDirectionVector() => sideDirection switch
        {
            Placement.SideDirection.LEFT => Vector3.left,
            Placement.SideDirection.RIGHT => Vector3.right,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        public Vector3 GetForwardDirectionVector() => forwardDirection switch
        {
            Placement.ForwardDirection.FORWARD => Vector3.forward,
            Placement.ForwardDirection.BACK => Vector3.back,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        public Vector3 NextStageDirectionVector() => nextStageDirection switch
        {
            Placement.NextStageDirection.LEFT => Vector3.left,
            Placement.NextStageDirection.RIGHT => Vector3.right,
            Placement.NextStageDirection.FORWARD => Vector3.forward,
            Placement.NextStageDirection.BACK => Vector3.back,
            _ => throw new ArgumentOutOfRangeException()
        };

        public Vector3 NextStageStepOffset(float verticalSpacing, float horizontalSpacing) => nextStageDirection switch
        {
            Placement.NextStageDirection.LEFT => Vector3.left * verticalSpacing,
            Placement.NextStageDirection.RIGHT => Vector3.right * verticalSpacing,
            Placement.NextStageDirection.FORWARD => Vector3.forward * horizontalSpacing,
            Placement.NextStageDirection.BACK => Vector3.back * horizontalSpacing,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        public Vector3 NextStorageOffset(float verticalSpacing, float horizontalSpacing) => nextStorageDirection switch
        {
            Placement.NextStageDirection.LEFT => Vector3.left * verticalSpacing,
            Placement.NextStageDirection.RIGHT => Vector3.right * verticalSpacing,
            Placement.NextStageDirection.FORWARD => Vector3.forward * horizontalSpacing,
            Placement.NextStageDirection.BACK => Vector3.back * horizontalSpacing,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}