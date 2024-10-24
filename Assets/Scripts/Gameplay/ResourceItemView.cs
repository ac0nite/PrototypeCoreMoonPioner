using System;
using Core.PlacementsStorage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Gameplay.Constants
{
    public class ResourceItemView : MonoBehaviour
    {
        private ColorRenderer _colorRenderer;

        private void OnValidate()
        {
            ViewSize = GetComponent<Renderer>()?.bounds.size ?? Vector3.zero;
        }

        private void Awake()
        {
            _colorRenderer = new ColorRenderer(GetComponent<Renderer>());
        }

        public Vector3 ViewSize { get; private set; }

        public Color ViewColor
        {
            get => _colorRenderer.Color;
            set => _colorRenderer.Color = value;
        }
        
        public void SetColor(Color color)
        {
            _colorRenderer.Color = color;
        }

        #region Factory

        public class Factory : PlaceholderFactory<ResourceItemView>
        {
        }

        #endregion
    }
    
    public class ColorRenderer
    {
        private readonly Renderer _renderer;
        private MaterialPropertyBlock _propertyBlock;
        private Color _color;

        public ColorRenderer(Renderer renderer)
        {
            _renderer = renderer;
            _propertyBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_propertyBlock);
        }

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            _propertyBlock.SetColor(Constants.Renderer.ResourceColorName, _color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }

    public class ResourceItem
    {
        private readonly ResourceItemView _view;

        public ResourceItem(ResourceItemView.Factory viewFactory, ResourceItemSettings resourceItemSettings)
        {
            _view = viewFactory.Create();
            Animation = new ResourceItemAnimation(_view, resourceItemSettings.AnimationSettings);
        }
        
        private void Initialise(Settings settings)
        {
            _view.ViewColor = settings.Color;
        }

        public ResourceItemAnimation Animation { get; }

        private void Enable(bool enabled)
        {
            _view.gameObject.SetActive(enabled);
        }

        #region Settings

        [Serializable]
        public struct Settings
        {
            public Color Color;
        }

        #endregion

        #region Pool

        public class Pool : MemoryPool<Settings, ResourceItem>
        {
            protected override void OnCreated(ResourceItem item)
            {
                base.OnCreated(item);
                item.Enable(false);
            }

            protected override void Reinitialize(Settings settings, ResourceItem item)
            {
                item.Initialise(settings);
                item.Enable(true);
            }

            protected override void OnDespawned(ResourceItem item)
            {
                base.OnDespawned(item);
                item.Enable(false);
            }
        }

        #endregion

        public void SetPoint(Placement.Point point, bool isParent)
        {
            Debug.Log($"SetPoint: {point.Parent.name} -> {point.Position}");
            if (isParent) _view.transform.SetParent(point.Parent, true);
            
            _view.transform.position = point.Position;
            _view.transform.rotation = point.Parent.rotation;
        }
    }
    

    public class ResourceItemAnimation
    {
        private readonly ResourceItemView _view;
        private readonly Settings _settings;
        private readonly Transform _transform;

        public ResourceItemAnimation(ResourceItemView view, Settings settings)
        {
            _view = view;
            _settings = settings;
            _transform = _view.transform;
        }

        public void PlayAnimationManufacture(Transform from, Transform to, Color toColor, float duration, Action callback = null)
        {
            DOTween.Sequence()
                .Append(CreateMovementTween(from, to, duration * _settings.MoveRatio))
                .Append(CreateProgressManufacturingSequence(toColor, duration * (1f - _settings.MoveRatio)))
                .OnComplete(() => callback?.Invoke());
        }
        
        public void PlayAnimationManufacture(Transform to, Color toColor, float duration, Action callback = null)
        {
            _view.transform.position = to.position;
            _view.transform.rotation = to.rotation;
            
            CreateProgressManufacturingSequence(toColor, duration).OnComplete(() => callback?.Invoke());
        }

        public UniTask PlayMoveTask(Placement.Point target)
        {
            return DOTween.Sequence()
                .Append(_transform.DOMove(target.Position, _settings.MoveDuration))
                .Join(_transform.DORotateQuaternion(target.Parent.rotation, _settings.MoveDuration))
                .AsyncWaitForCompletion()
                .AsUniTask();
        }

        public UniTask PlayJumpTask(Placement.Point target)
        {
            return DOTween.Sequence()
                .Append(_transform.DOJump(target.Position, _settings.JumpPower, _settings.NumJumps,_settings.MoveDuration))
                .Join(_transform.DORotateQuaternion(target.Parent.rotation, _settings.MoveDuration))
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
        
        public UniTask PlayProgressTask(Placement.Point point, Color toColor, float duration)
        {
            _transform.position = point.Position;
            _transform.rotation = point.Parent.rotation;
            
            return CreateProgressManufacturingSequence(toColor, duration)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }

        public UniTask PlayAnimationMovementTask(Transform from, Transform to, bool setToParent = false)
        {
            return CreateMovementTween(from, to, _settings.MoveDuration).OnComplete(() =>
            {
                if (setToParent) _view.transform.SetParent(to, true);
            })
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
        private Sequence CreateMovementTween(Transform from, Transform to, float duration)
        {
            var transform = _view.transform;
            return DOTween.Sequence()
                .Append(transform.DOJump(to.position, _settings.JumpPower, _settings.NumJumps, duration).OnStart(() => transform.position = from.position))
                .Join(transform.DORotateQuaternion(to.rotation, duration).OnStart(() => transform.rotation = from.rotation));

            //TODO: реализовать анимацию если персонаж в движении
        }

        private Sequence CreateJumpTween(Vector3 toPosition, float duration)
        {
            return _transform.DOJump(toPosition, _settings.JumpPower, _settings.NumJumps, duration);
        }

        private Tween CreateRotationTween(Quaternion toRotation, float duration)
        {
            return _transform.DORotateQuaternion(toRotation, duration);
        }

        private Tween CreateColorTween(Color toColor, float duration)
        {
            return DOVirtual.Color(_view.ViewColor, toColor, duration, value => _view.SetColor(value));
        }
        
        private Tween CreateShakeScaleTween(float duration)
        {
            return _transform.DOShakeScale(duration, 0.05f, 10, 90);
        }

        private Sequence CreateProgressManufacturingSequence(Color toColor, float duration)
        {
            return DOTween.Sequence()
                .Append(CreateColorTween(toColor, duration))
                .Join(CreateShakeScaleTween(duration));
        }

        #region Settings

        [Serializable]
        public struct Settings
        {
            [Range(0, 1)] public float MoveRatio;
            public float MoveDuration;
            public float JumpPower;
            public int NumJumps;
        }

        #endregion
    }
}