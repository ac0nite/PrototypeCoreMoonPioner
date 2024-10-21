using UnityEngine;

namespace Core.PlacementsStorage
{
    [CreateAssetMenu(fileName = "PlacementSettings", menuName = "Placements/PlacementSettings")]
    public class PlacementSettings : ScriptableObject
    {
        [Range(1,10)] public int RowMax;
        [Range(1,10)] public int ColMax;
        [Min(1)] public int StageAmount;
        [Range(0,1)] public float SpacingHeight;
        [Range(0,1)] public float SpacingWidth;
        [Range(0,1)] public float SpacingStage;
        [Range(0,1)] public float SpacingStorage;

        public int LayerSize => ColMax * RowMax;
        public float StageSideOffset(float widthSize) => SpacingStage + (SpacingWidth + widthSize) * RowMax;   
        public float StageForwardOffset(float depthSize) => SpacingStage + (SpacingWidth + depthSize) * ColMax;
        public float GetUpOffset(float heightSize) => SpacingHeight + heightSize;
        public float GetSideOffset(float widthSize) => SpacingWidth + widthSize;
        public float GetForwardOffset(float depthSize) => SpacingWidth + depthSize;
    }
}