using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Core.PlacementsStorage
{
    public interface IPlacement
    {
        Placement.Point GetNextPoint();
    }
    public class Placement : IPlacement
    {
        private readonly Transform _basePoint;
        private readonly ICollection _collection;
        
        private readonly float _layerSize;
        private readonly Vector3 _sideDirectionVector;
        private readonly Vector3 _forwardDirectionVector;
        
        private readonly int _stageAmount;
        private readonly Vector3 _nextStageStepOffset;
        private readonly FillMode _fillMode;
        private readonly int _rowMax;
        private readonly int _colMax;
        private readonly float _upOffset;
        private readonly float _rightOffset;
        private readonly float _forwardOffset;
        private readonly Vector3 _halfSizeOffset;
        private readonly Vector3 _storageOffset;
        private readonly Size _viewSize;
        
        private Vector3 _stageOffset = Vector3.zero;

        public Placement([NotNull] PointConfig config, Size size, [NotNull] ICollection collection, int storage = 1)
        {
            _basePoint = config.BasePoint;
            _viewSize = size;
            _collection = collection;

            var template = config.Template;
            _sideDirectionVector = template.GetSideDirectionVector();
            _forwardDirectionVector = template.GetForwardDirectionVector();
            _fillMode = template.FillMode;

            var settings = config.Settings;
            _layerSize = settings.LayerSize;

            _nextStageStepOffset = CalculateNextStageStepOffset(template, settings);
            _stageAmount = settings.StageAmount;

            _rowMax = settings.RowMax;
            _colMax = settings.ColMax;

            _upOffset = settings.GetUpOffset(_viewSize.Height);
            _rightOffset = settings.GetSideOffset(_viewSize.Width);
            _forwardOffset = settings.GetForwardOffset(_viewSize.Depth);

            _halfSizeOffset = CalculateHalfSizeOffset(settings);

            _storageOffset = CalculateStorageOffset(storage, settings, template);
        }
        public Point GetNextPoint()
        {
            var total = CalculateStageAmount(_collection.Count);

            var (row, col) = CalculateRowAndCol(total);

            var up = Vector3.up * (_upOffset * Mathf.CeilToInt(total / _layerSize));
            var right = _sideDirectionVector * (_rightOffset * col);
            var forward = _forwardDirectionVector * (_forwardOffset * row);

            var position = CalculatePosition(up, right, forward);

            return new Point()
            {
                Parent = _basePoint,
                Position = position,
                LocalPosition = _basePoint.InverseTransformPoint(position)
            };
        }
        
        private Vector3 CalculatePosition(Vector3 up, Vector3 right, Vector3 forward)
        {
            return _basePoint.position + 
                   _basePoint.TransformDirection(_halfSizeOffset + up + right + forward + _stageOffset + _storageOffset);
        }
        
        private (int row, int col) CalculateRowAndCol(int total)
        {
            int row, col;
            switch (_fillMode)
            {
                case FillMode.ROW_FIRST:
                    row = ((total - 1) / _rowMax) % _colMax;
                    col = ((total - 1) % _rowMax) % _rowMax;
                    break;
                case FillMode.COLUMN_FIRST:
                    row = ((total - 1) % _colMax) % _colMax;
                    col = ((total - 1) / _colMax) % _rowMax;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (row, col);
        }
        
        private int CalculateStageAmount(int amount)
        {
            var quantity = Mathf.CeilToInt(amount / (float)_stageAmount);
            _stageOffset = quantity > 1 ? _nextStageStepOffset * (quantity - 1) : Vector3.zero;
            return amount - _stageAmount * (quantity - 1);
        }
        
        private Vector3 CalculateHalfSizeOffset(PlacementSettings settings)
        {
            return new Vector3(
                -_viewSize.Width * 0.5f,
                -(_viewSize.Height * 0.5f + settings.SpacingHeight),
                _viewSize.Depth * 0.5f
            );
        }
        
        private Vector3 CalculateNextStageStepOffset(PlacementTemplate template, PlacementSettings settings)
        {
            return template.NextStageStepOffset(
                settings.StageSideOffset(_viewSize.Width),
                settings.StageForwardOffset(_viewSize.Depth)
            );
        }
        
        private Vector3 CalculateStorageOffset(int storage, PlacementSettings settings, PlacementTemplate template)
        {
            return storage <= 1
                ? Vector3.zero
                : template.NextStorageOffset(
                    (settings.StageSideOffset(_viewSize.Width) + settings.SpacingStorage) * (storage - 1),
                    (settings.StageForwardOffset(_viewSize.Depth) + settings.SpacingStorage) * (storage - 1)
                );
        }

        #region TEMPLATE TYPES
        
        public enum FillMode {ROW_FIRST, COLUMN_FIRST}
        public enum SideDirection {LEFT, RIGHT}
        public enum ForwardDirection {FORWARD, BACK}
        public enum NextStageDirection {LEFT, RIGHT, FORWARD, BACK}
        
        #endregion
        
        #region OUT OF PLACEMENT
        public record Point
        {
            public Transform Parent;
            public Vector3 Position;
            public Vector3 LocalPosition;
        }
        #endregion

        #region CONFIG

        [Serializable]
        public class PointConfig
        {
            public Transform BasePoint;
            public PlacementSettings Settings;
            public PlacementTemplate Template;
            
            #region EDITOR

            #if UNITY_EDITOR
            [Space]
            [SerializeField] private bool IsDebug;
            [SerializeField] private Size ViewSize;
            [SerializeField, Range(1,100)] private int _amount;
            [SerializeField, Range(1,5)] private int _storage;

            public void EditorGizmoDraw()
            {
                if(!IsDebug) return;
                
                Color[] colors = new[] {Color.red, Color.green, Color.yellow, Color.magenta};
                for (int i = 1; i <= _storage; i++)
                {
                    var collection = new Stack<int>();
                    var placement = new Placement(this, ViewSize, collection, i);
                    var points = Enumerable.Range(0, _amount).Select(_ =>
                    {
                        collection.Push(0);
                        return placement.GetNextPoint();
                    }).ToList();
                    
                    Gizmos.color = colors[(i-1) % colors.Length];
                    
                    Matrix4x4 oldMatrix = Gizmos.matrix;
                    foreach (var point in points)
                    {
                        Gizmos.matrix = Matrix4x4.TRS(point.Position, point.Parent.rotation, Vector3.one);
                        Gizmos.DrawWireCube(Vector3.zero, ViewSize.ToVector3());
                    }
                    Gizmos.matrix = oldMatrix;  
                }
            }      
            #endif
            #endregion
        }
        
        [Serializable]
        public struct Size
        {
            public float Width;
            public float Height;
            public float Depth;

            public Vector3 ToVector3() => new Vector3(Width, Height, Depth);
            public static implicit operator Vector3(Size size) => size.ToVector3();
            public static implicit operator Size(Vector3 vector) => new Size
            {
                Width = vector.x,
                Height = vector.y,
                Depth = vector.z
            };
        }

        #endregion
    }
}